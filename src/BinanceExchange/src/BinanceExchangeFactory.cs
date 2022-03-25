using ExchangeLibrary;
using Redis;

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
        public static IExchange CreateExchange(OptionsBase options, IRedisDatabase redisDatabase) =>
            new BinanceExchange((BinanceExchangeOptions)options, redisDatabase);
    }
}
