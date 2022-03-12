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
    }
}
