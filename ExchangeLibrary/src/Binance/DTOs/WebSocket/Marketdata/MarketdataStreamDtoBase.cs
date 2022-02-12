using ExchangeLibrary.Binance.Enums;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("E")]
        public long EventTimeUnix { get; set; }

        /// <summary>
        ///     Пара тикеров
        /// </summary>
        [JsonPropertyName("s")]
        public string Symbol { get; set; }

        ///// <summary>
        /////     Имя стрима (можно игнорировать)
        ///// </summary>
        //[JsonIgnore]
        //private string StreamName { get; set; }
    }
}
