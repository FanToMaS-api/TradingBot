using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models.SpotAccountTrade
{
    /// <summary>
    ///     Базовый класс ответа на запрос о создание нового ордера
    /// </summary>
    public abstract class OrderResponseModelBase
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
