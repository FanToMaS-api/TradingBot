using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель лучшей цены/кол-ва из стакана для пары
    /// </summary>
    public class SymbolOrderBookTickerModel
    {
        /// <summary>
        ///     Название пары
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Лучшая цена спроса
        /// </summary>
        [JsonPropertyName("bidPrice")]
        public double BidPrice { get; set; }

        /// <summary>
        ///     Лучшее кол-во спроса
        /// </summary>
        [JsonPropertyName("bidQty")]
        public double BidQty { get; set; }

        /// <summary>
        ///     Лучшая цена предложения
        /// </summary>
        [JsonPropertyName("askPrice")]
        public double AskPrice { get; set; }

        /// <summary>
        ///     Лучшее кол-во предложения
        /// </summary>
        [JsonPropertyName("askQty")]
        public double AskQty { get; set; }
    }
}
