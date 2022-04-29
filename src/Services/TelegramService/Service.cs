using Logger;
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

        #endregion

        #region .ctor

        /// <inheritdoc cref="Service"/>
        public Service(ITelegramClient client, ILoggerDecorator logger)
        {
            _client = client;
            _log = logger;
        }

        #endregion
    }
}
