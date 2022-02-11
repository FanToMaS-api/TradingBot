using ExchangeLibrary.Binance.Enums;
using Newtonsoft.Json;

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
        [JsonProperty("u")]
        public long OrderBookUpdatedId { get; set; }

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
    }
}
