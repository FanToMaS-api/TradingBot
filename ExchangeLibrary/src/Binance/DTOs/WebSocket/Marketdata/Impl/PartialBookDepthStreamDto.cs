using ExchangeLibrary.Binance.DTOs.Marketdata;
using ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata;
using ExchangeLibrary.Binance.Enums;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata.Impl
{
    internal class PartialBookDepthStreamDto : IMarketdataStreamDto
    {
        /// <summary>
        ///     Тип стрима с которого получаем данные
        /// </summary>
        public MarketdataStreamType StreamType => MarketdataStreamType.PartialBookDepthStream;

        /// <inheritdoc cref="OrderBookDto"/>
        public OrderBookDto OrderBook { get; set; }
    }
}
