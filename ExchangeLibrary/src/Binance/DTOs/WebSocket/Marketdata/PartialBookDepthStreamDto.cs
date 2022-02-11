using ExchangeLibrary.Binance.DTOs.Marketdata;
using ExchangeLibrary.Binance.Enums;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    internal class PartialBookDepthStreamDto
    {
        /// <summary>
        ///     Тип стрима с которого получаем данные
        /// </summary>
        public MarketdataStreamType StreamType => MarketdataStreamType.PartialBookDepthStream;

        /// <inheritdoc cref="OrderBookDto"/>
        public OrderBookDto OrderBook { get; set; }
    }
}
