using ExchangeLibrary.Binance.Enums;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    /// <summary>
    ///     Представляет собой объект данных со стримов маркетдаты
    /// </summary>
    public interface IMarketdataStreamDto
    {
        /// <summary>
        ///     Тип стрима с которого получаем данные
        /// </summary>
        public MarketdataStreamType StreamType { get; }
    }
}
