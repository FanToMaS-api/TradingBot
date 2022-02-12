using ExchangeLibrary.Binance.Enums;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    /// <summary>
    ///     
    /// </summary>
    internal class BookTickerStreamDto
    {
        /// <summary>
        ///     Тип стрима с которого получаем данные
        /// </summary>
        public MarketdataStreamType StreamType => MarketdataStreamType.IndividualSymbolBookTickerStream;

        /// <summary>
        ///     Идентификатор обновления книги заказов
        /// </summary>
        [JsonPropertyName("u")]
        public long OrderBookUpdatedId { get; set; }

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
    }
}
