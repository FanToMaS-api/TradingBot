using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель монеты
    /// </summary>
    internal class CoinModel
    {
        #region Properties

        /// <summary>
        ///     Обозначение монеты
        /// </summary>
        [JsonPropertyName("coin")]
        public string Coin { get; set; }

        /// <summary>
        ///     Название валюты
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        #endregion
    }
}
