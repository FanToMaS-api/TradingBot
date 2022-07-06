using Analytic.AnalyticUnits;
using Analytic.AnalyticUnits.Profiles.ML;
using Analytic.AnalyticUnits.Profiles.ML.DataLoaders.Impl.Binance;
using Analytic.Models;
using AutoMapper;
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
using TelegramServiceWeb.Configuration;

namespace TelegramServiceWeb
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
        private readonly IAnalyticProfile _analyticProfile;
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
            IMapper mapper,
            ILoggerDecorator logger)
        {
            _configuration = config;
            _serviceScopeFactory = serviceScopeFactory;
            _client = client;
            _log = logger;
            var dataLoader = new BinanceDataLoaderForSsa(_log, mapper);
            _analyticProfile = new MlAnalyticProfile(
                _log,
                MachineLearningModelType.SSA,
                dataLoader,
                "SsaAnalyticProfile");
        }

        #endregion

        #region Implementation of ITelegramService

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
                await _log.ErrorAsync(exception, "The service was stopped by token cancellation. Reason: ");
            }
            catch (Exception ex)
            {
                await _log.ErrorAsync(ex, cancellationToken: _cancellationTokenSource.Token);
            }
        }

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _log?.Dispose();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource.Dispose();
            _isDisposed = true;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обрабатывает новое сообщение
        /// </summary>
        /// <param name="message"> Модель сообщения из телеграмма </param>
        /// <param name="text"> текст сообщения </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        private async Task HandleUpdateAsync(Message message, string text, CancellationToken cancellationToken)
        {
            var messageBuilder = new TelegramMessageBuilder();
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
                messageBuilder.SetChatId(chatId);

                if (!await database.Users.IsExist(userId, cancellationToken))
                {
                    await SendMessageAsync(messageBuilder, DefaultText.WelcomeText, cancellationToken);

                    var newUser = await database.Users.CreateAsync(CreateUserEntity, cancellationToken);
                    await database.SaveChangesAsync(cancellationToken);

                    return;
                }

                await HandleUserMessageAsync(database, message, messageBuilder, cancellationToken);
            }
            catch (ApiRequestException apiEx)
            {
                await HandleApiRequestExceptionAsync(apiEx, database, message.From.Id, cancellationToken);
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
                    Status = UserStatusType.Active,
                };
            }
        }

        /// <summary>
        ///     Проверяет валидность входящих агрументов
        /// </summary>
        internal async Task<bool> IsArgumentsValidAsync(Message message, string text, CancellationToken cancellationToken)
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
            TelegramMessageBuilder messageBuilder,
            CancellationToken cancellationToken)
        {
            if (!await CanUserGetForecastAsync(database, message, messageBuilder, cancellationToken))
            {
                return;
            }

            var pairName = ConvertUserMessageToPairName(message.Text);
            var infoModel = new InfoModel(pairName);
            var (isSuccessfulAnalyze, resultModel) =
                await _analyticProfile.TryAnalyzeAsync(_serviceScopeFactory, infoModel, cancellationToken);
            if (!isSuccessfulAnalyze)
            {
                await SendMessageAsync(messageBuilder, DefaultText.UnsuccessfulAnalyzeText, cancellationToken);

                return;
            }

            await SendForecastAsync(pairName, resultModel, messageBuilder, cancellationToken);
            messageBuilder.Reset().SetChatId(message.Chat.Id);
        }

        /// <summary>
        ///     Проверят может ли пользователь получать прогнозы
        /// </summary>
        /// <param name="database"> База данных </param>
        /// <param name="message"> Модель сообщения телеграмма </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        internal async Task<bool> CanUserGetForecastAsync(
            ITelegramDbUnitOfWork database,
            Message message,
            TelegramMessageBuilder messageBuilder,
            CancellationToken cancellationToken)
        {
            var userId = message.From.Id;
            var user = await database.Users.GetAsync(userId, cancellationToken);
            if (user.UserState.Status == UserStatusType.Banned)
            {
                await SendMessageAsync(messageBuilder, DefaultText.BannedAccountText, cancellationToken);

                return false;
            }

            if (IsSpammer(user))
            {
                await WarnAboutSpamAsync(user, messageBuilder, cancellationToken);
                if (LimitWarningNumber - user.UserState.WarningNumber <= 0)
                {
                    await BanUserForSpamAsync(user, messageBuilder, cancellationToken);
                }

                user.UpdateLastAction();
                await database.SaveChangesAsync(cancellationToken);
                return false;
            }
            else
            {
                user.UserState.ResetWarningNumbers();
                user.UpdateLastAction();
                await database.SaveChangesAsync(cancellationToken);
            }

            if (!await _client.IsInChannelAsync(_configuration.ChannelId, userId, cancellationToken))
            {
                await SendMessageAsync(messageBuilder, DefaultText.RequiredSubscriptionText, cancellationToken);

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Отправляет сообщение пользователю
        /// </summary>
        /// <param name="messageBuilder"> Строитель с указанным Id чата </param>
        /// <param name="text"> Текст сообщения </param>
        public async Task SendMessageAsync(
            TelegramMessageBuilder messageBuilder,
            string text,
            CancellationToken cancellationToken)
        {
            var messageModel = messageBuilder
                   .SetMessageText(text)
                   .GetResult();
            await _client.SendMessageAsync(messageModel, cancellationToken);
        }

        /// <summary>
        ///     Преобразует сообщение пользователя в название пары
        /// </summary>
        /// <param name="text"> Сообщение пользователя  </param>
        /// <returns>
        ///     Название пары
        /// </returns>
        internal static string ConvertUserMessageToPairName(string text)
        {
            var pairName = text
                .Replace(" ", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace("-", "")
                .ToUpper();

            return pairName;
        }

        /// <summary>
        ///     Проверка является ли пользователь спаммером
        /// </summary>
        internal static bool IsSpammer(UserEntity user)
            => DateTime.Now - user.LastAction < TimeToDefineSpam;

        /// <summary>
        ///     Предупреждает пользователя о спаме
        /// </summary>
        private async Task WarnAboutSpamAsync(
            UserEntity userEntity,
            TelegramMessageBuilder messageBuilder,
            CancellationToken cancellationToken)
        {
            userEntity.UserState.IncrementWarningNumbers();
            var remainingWarnings = LimitWarningNumber - userEntity.UserState.WarningNumber;
            try
            {
                var warningText = $"{DefaultText.WarnAboutSpamText}\n" +
                    $"Осталось предупреждений до бана: *{remainingWarnings}*";
                await SendMessageAsync(messageBuilder, warningText, cancellationToken);
            }
            catch (Exception ex)
            {
                await _log.ErrorAsync(ex, "Failed to send warning about spam", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        ///     Банит пользователя за спам
        /// </summary>
        /// <param name="user"> Пользователь, который будет забанен </param>
        private async Task BanUserForSpamAsync(
            UserEntity user,
            TelegramMessageBuilder messageBuilder,
            CancellationToken cancellationToken)
        {
            user.Ban(BanReasonType.Spam);
            try
            {
                var banTextMessage = $"{DefaultText.BannedAccountText}. Причина бана - спам";
                await SendMessageAsync(messageBuilder, banTextMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                await _log.ErrorAsync(ex, "Failed to send message about ban", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        ///     Обрабатывает ошибку апи
        /// </summary>
        internal async Task HandleApiRequestExceptionAsync(
            ApiRequestException apiEx,
            ITelegramDbUnitOfWork database,
            long userId,
            CancellationToken cancellationToken)
        {
            if (!IsUserBlockedBot(apiEx))
            {
                await _log.ErrorAsync(apiEx, cancellationToken: cancellationToken);
                return;
            }

            var user = await database.Users.GetAsync(userId, cancellationToken);
            if (user is null)
            {
                return;
            }

            await database.UserStates.UpdateAsync(
                user.UserState.Id,
                _ => { _.Status = UserStatusType.BlockedBot; },
                cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        ///     Проверяет заблокировал ли пользователь бота
        /// </summary>
        internal static bool IsUserBlockedBot(ApiRequestException ex) => ex.Message == "Forbidden: bot was blocked by the user";

        /// <summary>
        ///     Отправляет прогноз пользователю
        /// </summary>
        /// <param name="pairName"> Название пары </param>
        /// <param name="resultModel"> Модель прогноза </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        private async Task SendForecastAsync(
            string pairName,
            AnalyticResultModel resultModel,
            TelegramMessageBuilder messageBuilder,
            CancellationToken cancellationToken)
        {
            var forecastingMessage = $"*{pairName}*\n*Минимальная цена прогноза: {resultModel.RecommendedPurchasePrice:0.00000}*";
            if (resultModel.RecommendedSellingPrice is not null)
            {
                forecastingMessage += $"\n*Максимальная цена прогноза: {resultModel.RecommendedSellingPrice.Value:0.00000}*";
            }

            if (resultModel.HasPredictionImage)
            {
                messageBuilder.SetImage(resultModel.ImagePath);
            }

            await SendMessageAsync(messageBuilder, forecastingMessage, cancellationToken);
        }

        #endregion
    }
}
