using ExchangeLibrary.Binance.Enums;
using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    /// <summary>
    ///     Базовый класс моделей получаемых со стримов маркетдаты
    /// </summary>
    internal class MarketdataStreamDtoBase
    {
        /// <summary>
        ///     Тип стрима с которого получаем данные
        /// </summary>
        public virtual MarketdataStreamType StreamType { get; }

        /// <summary>
        ///     Время события
        /// </summary>
        [JsonProperty("E")]
        public long EventTimeUnix { get; set; }

        /// <summary>
        ///     Пара тикеров
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; }
    }
}
