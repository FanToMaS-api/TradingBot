using Newtonsoft.Json;

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
        [JsonProperty("coin")]
        public string Coin { get; set; }

        /// <summary>
        ///     Название валюты
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        #endregion
    }
}
