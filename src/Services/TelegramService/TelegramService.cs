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
using TelegramService.Configuration;
using TelegramServiceDatabase;
using TelegramServiceDatabase.Entities;
using TelegramServiceDatabase.Repositories;
using TelegramServiceDatabase.Types;

namespace TelegramService
{
    /// <inheritdoc cref="ITelegramService"/>
    internal class TelegramService : ITelegramService
    {
        #region Fields

        /// <summary>
        ///     Считать спамом все от времени последнего сообщения пользователя, что пришло быстрее чем
        /// </summary>
        private static readonly TimeSpan TimeToDefineSpam = TimeSpan.FromMinutes(1);

        /// <summary>
        ///     Кол-во предупреждений пользователю, после которых он получит бан
        /// </summary>
        public const int LimitWarningNumber = 10;

        private readonly ILoggerDecorator _log;
        private readonly ITelegramClient _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TelegramMessageBuilder _messageBuilder = new();
        private readonly SsaAnalyticPofile _ssaAnalyticPofile;
        private readonly TelegramServiceConfig _configuration;
        private bool _isDisposed;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramService"/>
        public TelegramService(
            TelegramServiceConfig config,
            IServiceScopeFactory serviceScopeFactory,
            ITelegramClient client,
            ILoggerDecorator logger)
        {
            _configuration = config;
            _serviceScopeFactory = serviceScopeFactory;
            _client = client;
            _log = logger;
            _ssaAnalyticPofile = new(_log, "TelegramService_SsaAnalyticUnit");
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
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
            if (!await IsArgumentsValidAsync(message, text, cancellationToken))
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
                    await SendMessageAsync(DefaultText.WelcomeText, cancellationToken);
                    var newUser = await database.Users.CreateAsync(CreateUserEntity, cancellationToken);
                    await database.SaveChangesAsync(cancellationToken);

                    return;
                }

                await HandleUserMessageAsync(database, message, cancellationToken);
            }
            catch (ApiRequestException apiEx)
            {
                if (!IsUserBlockedBot(apiEx))
                {
                    await _log.ErrorAsync(apiEx, cancellationToken: cancellationToken);
                    return;
                }

                var user = await database.Users.GetAsync(message.From.Id, cancellationToken);
                if (user is null)
                {
                    return;
                }

                await database.UserStates.UpdateAsync(
                    user.UserState.Id,
                    _ => { _.UserStateType = UserStateType.BlockedBot; },
                    cancellationToken);
                await database.SaveChangesAsync(cancellationToken);

                return;
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
        private async Task<bool> IsArgumentsValidAsync(Message message, string text, CancellationToken cancellationToken)
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
        /// <param name="database"> База данных </param>
        /// <param name="message"> Модель сообщения телеграмма </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        private async Task HandleUserMessageAsync(
            ITelegramDbUnitOfWork database,
            Message message,
            CancellationToken cancellationToken)
        {
            if (!await IsUserValidAsync(database, message, cancellationToken))
            {
                return;
            }

            var pairName = message.Text
                .Replace(" ", "")
                .Replace("/", "")
                .Replace("\\", "")
                .ToUpper();
            var infoModel = new InfoModel(pairName);
            var (isSuccessfulAnalyze, resultModel) =
                await _ssaAnalyticPofile.TryAnalyzeAsync(_serviceScopeFactory, infoModel, cancellationToken);
            if (!isSuccessfulAnalyze)
            {
                await SendMessageAsync(DefaultText.UnsuccessfulAnalyzeText, cancellationToken);
                return;
            }

            await SendForecastAsync(pairName, resultModel, cancellationToken);
        }

        /// <summary>
        ///     Проверят может ли пользователь получать прогнозы
        /// </summary>
        /// <param name="database"> База данных </param>
        /// <param name="message"> Модель сообщения телеграмма </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        private async Task<bool> IsUserValidAsync(ITelegramDbUnitOfWork database, Message message, CancellationToken cancellationToken)
        {
            var userId = message.From.Id;
            var user = await database.Users.GetAsync(userId, cancellationToken);
            if (user.UserState.UserStateType == UserStateType.Banned)
            {
                await SendMessageAsync(DefaultText.BannedAccountText, cancellationToken);
                return false;
            }

            if (IsSpammer(user))
            {
                await WarnAboutSpamAsync(user, cancellationToken);
                if (LimitWarningNumber - user.UserState.WarningNumber <= 0)
                {
                    await BanUserAsync(user, cancellationToken);
                }

                UpdateUserLastAction(user);
                await database.SaveChangesAsync(cancellationToken);
                return false;
            }
            else
            {
                UnbanUser(user);
                UpdateUserLastAction(user);
                await database.SaveChangesAsync(cancellationToken);
            }

            if (!await _client.IsInChannelAsync(_configuration.ChannelId, userId, cancellationToken))
            {
                await SendMessageAsync(DefaultText.RequiredSubscriptionText, cancellationToken);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Проверка является ли пользователь спаммером
        /// </summary>
        private static bool IsSpammer(UserEntity user)
            => DateTime.Now - user.LastAction < TimeToDefineSpam;

        /// <summary>
        ///     Обновляет дату последнего действия пользователя
        /// </summary>
        /// <param name="user"> Пользователь </param>
        private static void UpdateUserLastAction(UserEntity user)
            => user.LastAction = DateTime.Now;

        /// <summary>
        ///     Предупреждает пользователя о спаме
        /// </summary>
        private async Task WarnAboutSpamAsync(UserEntity userEntity, CancellationToken cancellationToken)
        {
            userEntity.UserState.WarningNumber++;
            var remainingWarnings = LimitWarningNumber - userEntity.UserState.WarningNumber;
            try
            {
                await SendMessageAsync(
                    $"{DefaultText.WarnAboutSpamText}\nОсталось предупреждений до бана: *{remainingWarnings}*",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                await _log.ErrorAsync(ex, "Failed to send warning about spam", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        ///     Банит пользователя
        /// </summary>
        /// <param name="user"> Пользователь, который будет забанен </param>
        private async Task BanUserAsync(UserEntity user, CancellationToken cancellationToken)
        {
            user.UserState.UserStateType = UserStateType.Banned;
            user.UserState.BanReason = BanReasonType.Spam;
            try
            {
                await SendMessageAsync($"{DefaultText.BannedAccountText}. Причина бана: спам", cancellationToken);
            }
            catch (Exception ex)
            {
                await _log.ErrorAsync(ex, "Failed to send message about ban", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        ///     Убирает все предупреждения с пользователя
        /// </summary>
        /// <param name="user"> Пользователь </param>
        private void UnbanUser(UserEntity user)
        {
            user.UserState.UserStateType = UserStateType.Active;
            user.UserState.BanReason = BanReasonType.NotBanned;
            user.UserState.WarningNumber = 0;
        }

        /// <summary>
        ///     Отправляет сообщение пользователю
        /// </summary>
        private async Task SendMessageAsync(string text, CancellationToken cancellationToken)
        {
            _messageBuilder.SetMessageText(text);
            await _client.SendMessageAsync(_messageBuilder.GetResult(), cancellationToken);
        }

        /// <summary>
        ///     Проверяет заблокировал ли пользователь бота
        /// </summary>
        private static bool IsUserBlockedBot(ApiRequestException ex) => ex.Message == "Forbidden: bot was blocked by the user";

        /// <summary>
        ///     Отправляет прогноз пользователю
        /// </summary>
        /// <param name="pairName"> Название пары </param>
        /// <param name="resultModel"> Модель прогноза </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        private async Task SendForecastAsync(string pairName, AnalyticResultModel resultModel, CancellationToken cancellationToken)
        {
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
