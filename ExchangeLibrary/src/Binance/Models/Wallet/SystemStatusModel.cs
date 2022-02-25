using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель статуса системы
    /// </summary>
    internal class SystemStatusModel
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
