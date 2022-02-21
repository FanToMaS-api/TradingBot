using ExchangeLibrary.Binance;
using TraidingBot.Exchanges.Binance;

namespace ExchangeLibrary
{
    /// <summary>
    ///     Фабрика для бирж
    /// </summary>
    public static class ExchangeFactory
    {
        /// <summary>
        ///     Создать биржу по типу
        /// </summary>
        public static IExchange CreateExchange(ExchangeType exchangeType, OptionsBase options)
        {
            return exchangeType switch
            {
                ExchangeType.Binance => new BinanceExchange((BinanceExchangeOptions)options, null), // TODO Mapper
                _ => throw new System.Exception("Unexpected Type")
            };
        }
    }
}
