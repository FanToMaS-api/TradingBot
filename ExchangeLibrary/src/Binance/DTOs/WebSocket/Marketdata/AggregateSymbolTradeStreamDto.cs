using ExchangeLibrary.Binance.Enums;
using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    /// <summary>
    ///     Модель данных с потока торговой информации, которая агрегируется для одного ордера тейкера
    /// </summary>
    internal class AggregateSymbolTradeStreamDto : MarketdataStreamDtoBase
    {
        /// <inheritdoc />
        public override MarketdataStreamType StreamType => MarketdataStreamType.AggregateTradeStream;

        /// <summary>
        ///     Совокупное Id сделки
        /// </summary>
        [JsonProperty("a")]
        public long AggregateTradeId { get; set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        [JsonProperty("p")]
        public double Price { get; set; }

        /// <summary>
        ///     Объем сделки
        /// </summary>
        [JsonProperty("q")]
        public double Quantity { get; set; }

        /// <summary>
        ///     Первое Id сделки
        /// </summary>
        [JsonProperty("f")]
        public long FirstTradeId { get; set; }

        /// <summary>
        ///     Последнее Id сделки
        /// </summary>
        [JsonProperty("l")]
        public long LastTradeId { get; set; }

        /// <summary>
        ///     Время Id сделки
        /// </summary>
        [JsonProperty("T")]
        public long TradeTimeUnix { get; set; }

        /// <summary>
        ///     Является ли покупатель маркет-мейкером?
        /// </summary>
        [JsonProperty("m")]
        public bool IsMarketMaker { get; set; }
    }
}
