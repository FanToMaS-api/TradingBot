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
        public static IExchange CreateExchange(ExchangeType exchangeType, string apiKey, string secretKey)
        {
            return exchangeType switch
            {
                ExchangeType.Binance => new BinanceExchange(apiKey, secretKey),
                _ => throw new System.Exception("Unexpected Type")
            };
        }
    }
}
