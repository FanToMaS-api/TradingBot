using ExchangeLibrary.Binance.Enums;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("p")]
        public double Price { get; set; }

        /// <summary>
        ///    Изменение цены в процентах
        /// </summary>
        [JsonPropertyName("P")]
        public double PricePercentChange { get; set; }

        /// <summary>
        ///    Средневзвешенная цена
        /// </summary>
        [JsonPropertyName("w")]
        public double WeightedAveragePrice { get; set; }

        /// <summary>
        ///    Цена самой первой сделки до 24-х часового скользящего окна
        /// </summary>
        [JsonPropertyName("x")]
        public double FirstPrice { get; set; }

        /// <summary>
        ///    Последняя цена
        /// </summary>
        [JsonPropertyName("c")]
        public double LastPrice { get; set; }

        /// <summary>
        ///    Последнее     кол-во
        /// </summary>
        [JsonPropertyName("Q")]
        public double LastQuantity { get; set; }

        /// <summary>
        ///    Лучшая цена спроса
        /// </summary>
        [JsonPropertyName("b")]
        public double BestBidPrice { get; set; }

        /// <summary>
        ///    Лучшая объем спроса
        /// </summary>
        [JsonPropertyName("B")]
        public double BestBidQuantity { get; set; }

        /// <summary>
        ///    Лучшая цена предложения
        /// </summary>
        [JsonPropertyName("a")]
        public double BestAskPrice { get; set; }

        /// <summary>
        ///    Лучшая объем предложения
        /// </summary>
        [JsonPropertyName("A")]
        public double BestAskQuantity { get; set; }

        /// <summary>
        ///    Цена открытия
        /// </summary>
        [JsonPropertyName("o")]
        public double OpenPrice { get; set; }

        /// <summary>
        ///    Максимальная цена
        /// </summary>
        [JsonPropertyName("h")]
        public double MaxPrice { get; set; }

        /// <summary>
        ///    Минимальная цена
        /// </summary>
        [JsonPropertyName("l")]
        public double MinPrice { get; set; }

        /// <summary>
        ///    Общий торгуемый объем базовых активов
        /// </summary>
        [JsonPropertyName("v")]
        public double AllBaseVolume { get; set; }

        /// <summary>
        ///    Общий торгуемый объем котировочного актива
        /// </summary>
        [JsonPropertyName("q")]
        public double AllQuoteVolume { get; set; }

        /// <summary>
        ///    Время открытия статистики
        /// </summary>
        [JsonPropertyName("O")]
        public long StatisticOpenTimeUnix{ get; set; }

        /// <summary>
        ///    Время закрытия статистики
        /// </summary>
        [JsonPropertyName("C")]
        public long StatisticCloseTimeUnix { get; set; }

        /// <summary>
        ///    Id первой сделки
        /// </summary>
        [JsonPropertyName("F")]
        public long FirstTradeId { get; set; }

        /// <summary>
        ///    Id последней сделки
        /// </summary>
        [JsonPropertyName("L")]
        public long LastTradeId { get; set; }

        /// <summary>
        ///    Число сделок
        /// </summary>
        [JsonPropertyName("n")]
        public long TradeNumber { get; set; }
    }
}
