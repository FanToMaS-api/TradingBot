using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.DTOs.Wallet
{
    /// <summary>
    ///     Модель монеты
    /// </summary>
    public class CoinDto
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
