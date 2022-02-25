using ExchangeLibrary.Binance.Enums;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель данных с потока необработанной торговой информации; у каждой сделки есть уникальный покупатель и продавец
    /// </summary>
    internal class SymbolTradeStreamModel : MarketdataStreamModelBase, IMarketdataStreamModel
    {
        /// <inheritdoc />
        public MarketdataStreamType StreamType => MarketdataStreamType.TradeStream;

        /// <summary>
        ///     Идентификатор заказа продавца
        /// </summary>
        [JsonPropertyName("a")]
        public long SellerOrderId { get; set; }

        /// <summary>
        ///     Идентификатор заказа покупателя
        /// </summary>
        [JsonPropertyName("b")]
        public long BuyerOrderId { get; set; }

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
        ///     Id сделки
        /// </summary>
        [JsonPropertyName("t")]
        public long TradeId { get; set; }

        /// <summary>
        ///     Время сделки
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
