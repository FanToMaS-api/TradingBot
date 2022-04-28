using Common.Models;

namespace BinanceExchange
{
    /// <summary>
    ///     Содержит настройки Binance Exchange
    /// </summary>
    internal class BinanceExchangeOptions
    {
        /// <inheritdoc />
        public static string Name => "Binance";

        /// <summary>
        ///     Ключ подключения к Binance
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        ///     Секретный ключ подключения к Binance
        /// </summary>
        public string SecretKey { get; set; }
    }
}
