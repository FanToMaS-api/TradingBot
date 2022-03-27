namespace TradingBot.Configuration
{
    /// <summary>
    ///     Содержит ключи для получения настроек из файла конфигурации
    /// </summary>
    internal class ConfigKeys
    {
        /// <summary>
        ///     Ключ подключения к Binance
        /// </summary>
        public const string API_KEY = nameof(API_KEY);

        /// <summary>
        ///     Секретный ключ подключения к Binance
        /// </summary>
        public const string SECRET_KEY = nameof(SECRET_KEY);

        /// <summary>
        ///     Токен подключения к Telegram боту
        /// </summary>
        public const string TELEGRAM_TOKEN = nameof(TELEGRAM_TOKEN);

        /// <summary>
        ///     Id telegram канала для отправки
        /// </summary>
        public const string TELEGRAM_CHANNEL_ID = nameof(TELEGRAM_CHANNEL_ID);

        /// <summary>
        ///     Строка подключения к Redis
        /// </summary>
        public const string REDIS_CONNECTION_STRING = nameof(REDIS_CONNECTION_STRING);
    }
}
