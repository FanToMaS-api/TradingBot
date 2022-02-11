using ExchangeLibrary.Binance.Enums;
using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    /// <summary>
    ///     Модель данных с потока необработанной торговой информации; у каждой сделки есть уникальный покупатель и продавец
    /// </summary>
    internal class SymbolTradeStreamDto : MarketdataStreamDtoBase
    {
        /// <inheritdoc />
        public override MarketdataStreamType StreamType => MarketdataStreamType.TradeStream;

        /// <summary>
        ///     Идентификатор заказа продавца
        /// </summary>
        [JsonProperty("a")]
        public long SellerOrderId { get; set; }

        /// <summary>
        ///     Идентификатор заказа покупателя
        /// </summary>
        [JsonProperty("b")]
        public long BuyerOrderId { get; set; }

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
