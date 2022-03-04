using ExchangeLibrary;

namespace BinanceExchange
{
    /// <summary>
    ///     Фабрика для бирж
    /// </summary>
    public static class BinanceExchangeFactory
    {
        /// <summary>
        ///     Создать биржу по типу
        /// </summary>
        public static IExchange CreateExchange(ExchangeType exchangeType, OptionsBase options)
        {
            return exchangeType switch
            {
                ExchangeType => new BinanceExchange((BinanceExchangeOptions)options)
            };
        }
    }
}
