using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///    Модель трейдинг статуса аккаунта 
    /// </summary>
    internal class AccountTradingStatusModel
    {
        #region Properties

        /// <summary>
        ///     Содержит инфу об аккаунте
        /// </summary>
        [JsonPropertyName("data")]
        public DataDto Data { get; set; }

        #endregion
    }
}
