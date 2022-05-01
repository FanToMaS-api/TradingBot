using Logger;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Client;

namespace TelegramService
{
    /// <summary>
    ///     Сервис по получению и обработке сообщений от пользователей
    /// </summary>
    public class Service
    {
        #region Fields

        private readonly ILoggerDecorator _log;
        private readonly ITelegramClient _client;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Service"/>
        public Service(ITelegramClient client, ILoggerDecorator logger)
        {
            _client = client;
            _log = logger;
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
                    await OnMessageReceived?.Invoke(update.Message, update.Message.Text, _cancellationTokenSource.Token);
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
    }
}
