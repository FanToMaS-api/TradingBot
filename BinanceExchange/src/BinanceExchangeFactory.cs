using ExchangeLibrary;

namespace BinanceExchange
{
    /// <summary>
    ///     Фабрика для Binance биржи
    /// </summary>
    public static class BinanceExchangeFactory
    {
        /// <summary>
        ///     Создать Binance биржу
        /// </summary>
        public static IExchange CreateExchange(OptionsBase options) => new BinanceExchange((BinanceExchangeOptions)options);
    }
}
