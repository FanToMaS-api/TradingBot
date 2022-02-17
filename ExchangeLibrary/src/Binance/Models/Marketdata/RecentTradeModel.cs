using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель недавней сделки
    /// </summary>
    public class RecentTradeModel
    {
        /// <summary>
        ///     Уникальный идентификатор
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        [JsonPropertyName("price")]
        public double Price { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        [JsonPropertyName("qty")]
        public double Qty { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        [JsonPropertyName("quoteQty")]
        public double QuoteQty { get; set; }

        /// <summary>
        ///     Время сделки
        /// </summary>
        [JsonPropertyName("time")]
        public long TimeUnix { get; set; }

        /// <summary>
        ///     Была ли покупка по указанной покупателем цене
        /// </summary>
        [JsonPropertyName("isBuyerMaker")]
        public bool IsBuyerMaker { get; set; }

        /// <summary>
        ///     Была ли встречная сделка
        /// </summary>
        [JsonPropertyName("isBestMatch")]
        public bool IsBestMatch { get; set; }
    }
}
