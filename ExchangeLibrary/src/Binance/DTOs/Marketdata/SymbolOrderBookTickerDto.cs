using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.Marketdata
{
    /// <summary>
    ///     Модель лучшей цены/кол-ва из стакана для пары
    /// </summary>
    public class SymbolOrderBookTickerDto
    {
        /// <summary>
        ///     Название пары
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Лучшая цена спроса
        /// </summary>
        [JsonProperty("bidPrice")]
        public double BidPrice { get; set; }

        /// <summary>
        ///     Лучшее кол-во спроса
        /// </summary>
        [JsonProperty("bidQty")]
        public double BidQty { get; set; }

        /// <summary>
        ///     Лучшая цена предложения
        /// </summary>
        [JsonProperty("askPrice")]
        public double AskPrice { get; set; }

        /// <summary>
        ///     Лучшее кол-во предложения
        /// </summary>
        [JsonProperty("askQty")]
        public double AskQty { get; set; }
    }
}
