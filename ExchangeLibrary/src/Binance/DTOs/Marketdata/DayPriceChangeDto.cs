using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.Marketdata
{
    /// <summary>
    ///     Модель изменения цены за 1 по паре
    /// </summary>
    public class DayPriceChangeDto
    {
        /// <summary>
        ///     Наименование пары
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Изменение цены
        /// </summary>
        [JsonProperty("priceChange")]
        public double PriceChange { get; set; }

        /// <summary>
        ///     Изменение цены в процентах
        /// </summary>
        [JsonProperty("priceChangePercent")]
        public double PriceChangePercent { get; set; }

        /// <summary>
        ///     Взвешенная средняя цена
        /// </summary>
        [JsonProperty("weightedAvgPrice")]
        public double WeightedAvgPrice { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        [JsonProperty("prevClosePrice")]
        public double PrevClosePrice { get; set; }

        /// <summary>
        ///     Последняя цена
        /// </summary>
        [JsonProperty("lastPrice")]
        public double LastPrice { get; set; }

        /// <summary>
        ///     Последний объем
        /// </summary>
        [JsonProperty("lastQty")]
        public double LastQty { get; set; }

        /// <summary>
        ///     Цена спроса
        /// </summary>
        [JsonProperty("bidPrice")]
        public double BidPrice { get; set; }

        /// <summary>
        ///     Объем спроса
        /// </summary>
        [JsonProperty("bidQty")]
        public double BidQty { get; set; }

        /// <summary>
        ///     Цена предложения
        /// </summary>
        [JsonProperty("askPrice")]
        public double AskPrice { get; set; }

        /// <summary>
        ///     Объем предложения
        /// </summary>
        [JsonProperty("askQty")]
        public double AskQty { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        [JsonProperty("openPrice")]
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        [JsonProperty("highPrice")]
        public double HighPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        [JsonProperty("lowPrice")]
        public double LowPrice { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        [JsonProperty("volume")]
        public double Volume { get; set; }

        /// <summary>
        ///     Объем котировки
        /// </summary>
        [JsonProperty("quoteVolume")]
        public double QuoteVolume { get; set; }

        /// <summary>
        ///     Время открытия
        /// </summary>
        [JsonProperty("openTime")]
        public long OpenTimeUnix { get; set; }

        /// <summary>
        ///     Время закрытия
        /// </summary>
        [JsonProperty("closeTime")]
        public long CloseTimeUnix { get; set; }

        /// <summary>
        ///     Id первой сделки
        /// </summary>
        [JsonProperty("firstId")]
        public long FirstId { get; set; }

        /// <summary>
        ///     Id последеней сделки
        /// </summary>
        [JsonProperty("LastId")]
        public long LastId { get; set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        [JsonProperty("count")]
        public long Count { get; set; }
    }
}
