using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.Wallet
{
    /// <summary>
    ///     Модель статуса системы
    /// </summary>
    public class SystemStatusDto
    {
        #region Properties

        /// <summary>
        ///     Код статуса системы
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        /// <summary>
        ///     Сообщение статуса системы
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        #endregion
    }
}
