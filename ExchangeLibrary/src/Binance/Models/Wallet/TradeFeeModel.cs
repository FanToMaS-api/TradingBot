using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель ответа на запрос таксы по коину
    /// </summary>
    public class TradeFeeModel
    {
        #region Properties

        /// <summary>
        ///     Обозначение монеты
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Coin { get; set; }

        /// <summary>
        ///     Коммисия мейкера
        /// </summary>
        [JsonPropertyName("makerCommission")]
        public double MakerCommission { get; set; }

        /// <summary>
        ///     Коммисия тейкера
        /// </summary>
        [JsonPropertyName("takerCommission")]
        public double TakerCommission { get; set; }

        #endregion
    }
}
