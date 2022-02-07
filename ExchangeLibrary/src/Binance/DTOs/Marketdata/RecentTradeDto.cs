using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.Marketdata
{
    /// <summary>
    ///     Модель недавней сделки
    /// </summary>
    public class RecentTradeDto
    {
        /// <summary>
        ///     Уникальный идентификатор
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        [JsonProperty("price")]
        public double Price { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        [JsonProperty("qty")]
        public double Qty { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        [JsonProperty("quoteQty")]
        public double QuoteQty { get; set; }

        /// <summary>
        ///     Время сделки
        /// </summary>
        [JsonProperty("time")]
        public long TimeUnix { get; set; }

        /// <summary>
        ///     Была ли покупка по указанной покупателем цене
        /// </summary>
        [JsonProperty("isBuyerMaker")]
        public bool IsBuyerMaker { get; set; }

        /// <summary>
        ///     Была ли встречная сделка
        /// </summary>
        [JsonProperty("isBestMatch")]
        public bool IsBestMatch { get; set; }
    }
}
