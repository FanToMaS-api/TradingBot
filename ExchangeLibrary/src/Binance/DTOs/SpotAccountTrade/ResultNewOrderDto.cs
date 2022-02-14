using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.DTOs.SpotAccountTrade
{
    /// <summary>
    ///     Модель ответа на отправку нового ордера (содержит основную информацию)
    /// </summary>
    internal class ResultNewOrderDto : NewOrderDtoBase
    {
        /// <summary>
        ///     Цена
        /// </summary>
        [JsonPropertyName("price")]
        public double Price { get; set; }

        /// <summary>
        ///     Заданное кол-во
        /// </summary>
        [JsonPropertyName("origQty")]
        public double OriginalQuantity { get; set; }

        /// <summary>
        ///     Выполненное кол-во
        /// </summary>
        [JsonPropertyName("executedQty")]
        public double ExecutedQuantity { get; set; }

        /// <summary>
        ///     Статус ордера
        /// </summary>
        public OrderStatusType Status => status.ConvertToOrderStatusType();

        /// <summary>
        ///     Время жизни ордера
        /// </summary>
        public TimeInForceType TimeInForce => timeInForce.ConvertToTimeInForceType();

        /// <summary>
        ///     Тип ордера
        /// </summary>
        public OrderType OrderType => type.ConvertToOrderType();

        /// <summary>
        ///     Тип ордера
        /// </summary>
        public OrderSideType OrderSideType => side.ConvertToOrderSideType();

        /// <summary>
        ///     Статус ордера (нужен для парса json)
        /// </summary>
        [JsonPropertyName("status")]
        public string status { get; set; }

        /// <summary>
        ///     Время жизни ордера
        /// </summary>
        [JsonPropertyName("timeInForce")]
        public string timeInForce { get; set; }

        /// <summary>
        ///     Тип ордера (нужен для парса json)
        /// </summary>
        [JsonPropertyName("type")]
        public string type { get; set; }

        /// <summary>
        ///    Тип ордера (нужен для парса json)
        /// </summary>
        [JsonPropertyName("side")]
        public string side { get; set; }

    }
}
