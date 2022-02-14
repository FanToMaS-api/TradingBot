using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models.Wallet
{
    /// <summary>
    ///     Модель монеты
    /// </summary>
    public class CoinModel
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
