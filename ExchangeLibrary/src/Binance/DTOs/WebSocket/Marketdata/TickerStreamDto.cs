using ExchangeLibrary.Binance.Enums;
using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    /// <summary>
    ///     Модель статистики бегущего окна за 24 часа для одного символа
    /// </summary>
    internal class TickerStreamDto : MarketdataStreamDtoBase
    {
        /// <inheritdoc />
        public override MarketdataStreamType StreamType => MarketdataStreamType.IndividualSymbolTickerStream;

        /// <summary>
        ///    Цена
        /// </summary>
        [JsonProperty("p")]
        public double Price { get; set; }

        /// <summary>
        ///    Изменение цены в процентах
        /// </summary>
        [JsonProperty("P")]
        public double PricePercentChange { get; set; }

        /// <summary>
        ///    Средневзвешенная цена
        /// </summary>
        [JsonProperty("w")]
        public double WeightedAveragePrice { get; set; }

        /// <summary>
        ///    Цена самой первой сделки до 24-х часового скользящего окна
        /// </summary>
        [JsonProperty("x")]
        public double FirstPrice { get; set; }

        /// <summary>
        ///    Последняя цена
        /// </summary>
        [JsonProperty("c")]
        public double LastPrice { get; set; }

        /// <summary>
        ///    Последнее     кол-во
        /// </summary>
        [JsonProperty("Q")]
        public double LastQuantity { get; set; }

        /// <summary>
        ///    Лучшая цена спроса
        /// </summary>
        [JsonProperty("b")]
        public double BestBidPrice { get; set; }

        /// <summary>
        ///    Лучшая объем спроса
        /// </summary>
        [JsonProperty("B")]
        public double BestBidQuantity { get; set; }

        /// <summary>
        ///    Лучшая цена предложения
        /// </summary>
        [JsonProperty("a")]
        public double BestAskPrice { get; set; }

        /// <summary>
        ///    Лучшая объем предложения
        /// </summary>
        [JsonProperty("A")]
        public double BestAskQuantity { get; set; }

        /// <summary>
        ///    Цена открытия
        /// </summary>
        [JsonProperty("o")]
        public double OpenPrice { get; set; }

        /// <summary>
        ///    Максимальная цена
        /// </summary>
        [JsonProperty("h")]
        public double MaxPrice { get; set; }

        /// <summary>
        ///    Минимальная цена
        /// </summary>
        [JsonProperty("l")]
        public double MinPrice { get; set; }

        /// <summary>
        ///    Общий торгуемый объем базовых активов
        /// </summary>
        [JsonProperty("v")]
        public double AllBaseVolume { get; set; }

        /// <summary>
        ///    Общий торгуемый объем котировочного актива
        /// </summary>
        [JsonProperty("q")]
        public double AllQuoteVolume { get; set; }

        /// <summary>
        ///    Время открытия статистики
        /// </summary>
        [JsonProperty("O")]
        public long StatisticOpenTimeUnix{ get; set; }

        /// <summary>
        ///    Время закрытия статистики
        /// </summary>
        [JsonProperty("C")]
        public long StatisticCloseTimeUnix { get; set; }

        /// <summary>
        ///    Id первой сделки
        /// </summary>
        [JsonProperty("F")]
        public long FirstTradeId { get; set; }

        /// <summary>
        ///    Id последней сделки
        /// </summary>
        [JsonProperty("L")]
        public long LastTradeId { get; set; }

        /// <summary>
        ///    Число сделок
        /// </summary>
        [JsonProperty("n")]
        public long TradeNumber { get; set; }
    }
}
