using ExchangeLibrary.Binance.Enums;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель данных с потока торговой информации, которая агрегируется для одного ордера тейкера
    /// </summary>
    public class AggregateSymbolTradeStreamModel : MarketdataStreamModelBase, IMarketdataStreamModel
    {
        /// <inheritdoc />
        public MarketdataStreamType StreamType => MarketdataStreamType.AggregateTradeStream;

        /// <summary>
        ///     Совокупное Id сделки
        /// </summary>
        [JsonPropertyName("a")]
        public long AggregateTradeId { get; set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        [JsonPropertyName("p")]
        public double Price { get; set; }

        /// <summary>
        ///     Объем сделки
        /// </summary>
        [JsonPropertyName("q")]
        public double Quantity { get; set; }

        /// <summary>
        ///     Первое Id сделки
        /// </summary>
        [JsonPropertyName("f")]
        public long FirstTradeId { get; set; }

        /// <summary>
        ///     Последнее Id сделки
        /// </summary>
        [JsonPropertyName("l")]
        public long LastTradeId { get; set; }

        /// <summary>
        ///     Время Id сделки
        /// </summary>
        [JsonPropertyName("T")]
        public long TradeTimeUnix { get; set; }

        /// <summary>
        ///     Является ли покупатель маркет-мейкером?
        /// </summary>
        [JsonPropertyName("m")]
        public bool IsMarketMaker { get; set; }
    }
}
