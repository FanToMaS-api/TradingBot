using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Базовый класс моделей получаемых со стримов маркетдаты
    /// </summary>
    internal class MarketdataStreamModelBase
    {
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
    }
}
