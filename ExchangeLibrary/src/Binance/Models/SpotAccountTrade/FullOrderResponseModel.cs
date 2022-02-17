using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель ответа на отправку нового ордера (содержит полную информацию)
    /// </summary>
    internal class FullOrderResponseModel
    {
        /// <summary>
        ///     Пара
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Id ордера
        /// </summary>
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }

        /// <summary>
        ///     Id клиентского ордера
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public long ClientOrderId { get; set; }

        /// <summary>
        ///     Время исполнения транзакции
        /// </summary>
        [JsonPropertyName("transactTime")]
        public long TransactTimeUnix { get; set; }
    }
}
