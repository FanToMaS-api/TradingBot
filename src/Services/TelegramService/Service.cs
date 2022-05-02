﻿using Analytic.AnalyticUnits.Profiles.SSA;
using Analytic.Models;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Builder;
using Telegram.Client;
using TelegramServiceDatabase;
using TelegramServiceDatabase.Entities;
using TelegramServiceDatabase.Repositories;
using TelegramServiceDatabase.Types;

namespace TelegramService
{
    /// <summary>
    ///     Сервис по получению и обработке сообщений от пользователей
    /// </summary>
    public class Service : IDisposable
    {
        #region Fields

        private readonly ILoggerDecorator _log;
        private readonly ITelegramClient _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TelegramMessageBuilder _messageBuilder = new();
        private readonly SsaAnalyticPofile _ssaAnalyticPofile;
        private bool _isDisposed;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Service"/>
        public Service(IServiceScopeFactory serviceScopeFactory, ITelegramClient client, ILoggerDecorator logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _client = client;
            _log = logger;
            _ssaAnalyticPofile = new(_log, "TelegramService_SsaAnalyticUnit");
        }

        #endregion

        #region Public methods

        public async Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var updateReceiver = _client.GetUpdateReceiver(new[] { UpdateType.Message });
            try
            {
                await _log.InfoAsync("Telegram service successfully launched!");

                await foreach (var update in updateReceiver.WithCancellation(_cancellationTokenSource.Token))
                {
                    await HandleUpdateAsync(update.Message, update.Message.Text, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException exception)
            {
                await _log.WarnAsync(exception, "The service was stopped by token cancellation. Reason: ");
            }
            catch (Exception ex)
            {
                await _log.ErrorAsync(ex, cancellationToken: _cancellationTokenSource.Token);
            }
        }

        #endregion

        #region Private methods

        /// <inheritdoc />
        public async Task HandleUpdateAsync(Message message, string text, CancellationToken cancellationToken)
        {
            if (!await IsValidAsync(message, text, cancellationToken))
            {
                return;
            }

            using var scope = _serviceScopeFactory.CreateAsyncScope();
            var databaseFactory = scope.ServiceProvider.GetRequiredService<ITelegramDatabaseFactory>();
            using var database = databaseFactory.CreateScopeDatabase();

            try
            {
                var userId = message.From.Id;
                var chatId = message.Chat.Id;
                _messageBuilder.Reset();
                _messageBuilder.SetChatId(chatId);
                if (!await database.Users.IsExist(userId, cancellationToken))
                {
                    var newUser = await database.Users.CreateAsync(CreateUserEntity, cancellationToken);
                    await SenMessageAsync(DefaultText.WelcomeText, cancellationToken);

                    return;
                }

                await HandleUserMessageAsync(database, message, cancellationToken);
            }
            catch (ApiRequestException apiEx)
            {
                if (IsUserBlockedBot(apiEx))
                {
                    var user = await database.Users.GetAsync(message.From.Id, cancellationToken);
                    if (user is null)
                    {
                        return;
                    }

                    await database.UserStates.UpdateAsync(
                        user.UserState.Id,
                        _ => { _.UserStateType = UserStateType.BlockedBot; },
                        cancellationToken);

                    return;
                }

                await _log.ErrorAsync(apiEx, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await _log.ErrorAsync(ex, cancellationToken: cancellationToken);
            }

            void CreateUserEntity(UserEntity user)
            {
                user.TelegramId = message.From.Id;
                user.Nickname = message.From.Username;
                user.LastName = message.From.LastName;
                user.ChatId = message.Chat.Id;
                user.FirstName = message.From.FirstName;
                user.LastAction = DateTime.Now;
                user.UserState = new UserStateEntity
                {
                    UserId = user.TelegramId,
                    UserStateType = UserStateType.Active,
                };
            }
        }

        /// <summary>
        ///     Проверяет валидность входящих агрументов
        /// </summary>
        private async Task<bool> IsValidAsync(Message message, string text, CancellationToken cancellationToken)
        {
            if (message is null)
            {
                await _log.ErrorAsync(new NullReferenceException(nameof(message)), cancellationToken: cancellationToken);

                return false;
            }

            if (string.IsNullOrEmpty(text))
            {
                await _log.WarnAsync(new NullReferenceException(nameof(text)), cancellationToken: cancellationToken);

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Обрабатывает пользовательское сообщение
        /// </summary>
        /// <param name="database"> база данных </param>
        /// <param name="message"> Модель сообщения телеграмма </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        private async Task HandleUserMessageAsync(
            ITelegramDbUnitOfWork database,
            Message message,
            CancellationToken cancellationToken)
        {
            var userId = message.From.Id;
            var user = await database.Users.GetAsync(userId, cancellationToken);
            if (user.UserState.UserStateType == UserStateType.Banned)
            {
                await SenMessageAsync(DefaultText.BannedAccountText, cancellationToken);

                return;
            }

            var pairName = message.Text
                .Replace(" ", "")
                .Replace("/", "")
                .Replace("\\", "");
            var infoModel = new InfoModel(pairName);
            var(isSuccessfulAnalyze, resultModel) =
                await _ssaAnalyticPofile.TryAnalyzeAsync(_serviceScopeFactory, infoModel, cancellationToken);
            if (!isSuccessfulAnalyze)
            {
                await SenMessageAsync(DefaultText.UnsuccessfulAnalyze, cancellationToken);

                return;
            }

            var forecastingMessage = $"*{pairName}*\n*Минимальная цена прогноза: {resultModel.RecommendedPurchasePrice:0.00000}*";
            if (resultModel.RecommendedSellingPrice is not null)
            {
                forecastingMessage += $"\n*Максимальная цена прогноза: {resultModel.RecommendedSellingPrice.Value:0.00000}*";
            }

            _messageBuilder.SetMessageText(forecastingMessage);
            if (resultModel.HasPredictionImage)
            {
                _messageBuilder.SetImage(resultModel.ImagePath);
            }

            await _client.SendMessageAsync(_messageBuilder.GetResult(), cancellationToken);
        }

        /// <summary>
        ///     Отправляет сообщение пользователю
        /// </summary>
        private async Task SenMessageAsync(string text, CancellationToken cancellationToken)
        {
            _messageBuilder.SetMessageText(text);
            await _client.SendMessageAsync(_messageBuilder.GetResult(), cancellationToken);
        }

        /// <summary>
        ///     Проверяет заблокировал ли пользователь бота
        /// </summary>
        private static bool IsUserBlockedBot(ApiRequestException ex) => ex.Message == "Forbidden: bot was blocked by the user";

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource.Dispose();
            _isDisposed = true;
        }

        #endregion
    }
}
