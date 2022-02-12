using System.Text.Json.Serialization;

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
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Изменение цены
        /// </summary>
        [JsonPropertyName("priceChange")]
        public double PriceChange { get; set; }

        /// <summary>
        ///     Изменение цены в процентах
        /// </summary>
        [JsonPropertyName("priceChangePercent")]
        public double PriceChangePercent { get; set; }

        /// <summary>
        ///     Взвешенная средняя цена
        /// </summary>
        [JsonPropertyName("weightedAvgPrice")]
        public double WeightedAvgPrice { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        [JsonPropertyName("prevClosePrice")]
        public double PrevClosePrice { get; set; }

        /// <summary>
        ///     Последняя цена
        /// </summary>
        [JsonPropertyName("lastPrice")]
        public double LastPrice { get; set; }

        /// <summary>
        ///     Последний объем
        /// </summary>
        [JsonPropertyName("lastQty")]
        public double LastQty { get; set; }

        /// <summary>
        ///     Цена спроса
        /// </summary>
        [JsonPropertyName("bidPrice")]
        public double BidPrice { get; set; }

        /// <summary>
        ///     Объем спроса
        /// </summary>
        [JsonPropertyName("bidQty")]
        public double BidQty { get; set; }

        /// <summary>
        ///     Цена предложения
        /// </summary>
        [JsonPropertyName("askPrice")]
        public double AskPrice { get; set; }

        /// <summary>
        ///     Объем предложения
        /// </summary>
        [JsonPropertyName("askQty")]
        public double AskQty { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        [JsonPropertyName("openPrice")]
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        [JsonPropertyName("highPrice")]
        public double HighPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        [JsonPropertyName("lowPrice")]
        public double LowPrice { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        [JsonPropertyName("volume")]
        public double Volume { get; set; }

        /// <summary>
        ///     Объем котировки
        /// </summary>
        [JsonPropertyName("quoteVolume")]
        public double QuoteVolume { get; set; }

        /// <summary>
        ///     Время открытия
        /// </summary>
        [JsonPropertyName("openTime")]
        public long OpenTimeUnix { get; set; }

        /// <summary>
        ///     Время закрытия
        /// </summary>
        [JsonPropertyName("closeTime")]
        public long CloseTimeUnix { get; set; }

        /// <summary>
        ///     Id первой сделки
        /// </summary>
        [JsonPropertyName("firstId")]
        public long FirstId { get; set; }

        /// <summary>
        ///     Id последеней сделки
        /// </summary>
        [JsonPropertyName("LastId")]
        public long LastId { get; set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        [JsonPropertyName("count")]
        public long Count { get; set; }
    }
}
