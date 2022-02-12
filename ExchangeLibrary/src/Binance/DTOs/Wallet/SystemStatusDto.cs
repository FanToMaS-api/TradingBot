using System.Text.Json.Serialization;

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
        [JsonPropertyName("status")]
        public int Status { get; set; }

        /// <summary>
        ///     Сообщение статуса системы
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        #endregion
    }
}
