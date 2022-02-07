using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.Wallet
{
    /// <summary>
    ///     Модель ответа на запрос таксы по коину
    /// </summary>
    public class TradeFeeDto
    {
        #region Properties

        /// <summary>
        ///     Обозначение монеты
        /// </summary>
        [JsonProperty("symbol")]
        public string Coin { get; set; }

        /// <summary>
        ///     Коммисия мейкера
        /// </summary>
        [JsonProperty("makerCommission")]
        public double MakerCommission { get; set; }

        /// <summary>
        ///     Коммисия тейкера
        /// </summary>
        [JsonProperty("takerCommission")]
        public double TakerCommission { get; set; }

        #endregion
    }
}
