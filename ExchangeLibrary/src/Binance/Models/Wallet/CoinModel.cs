using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
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
