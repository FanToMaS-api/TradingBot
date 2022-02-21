using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using System.Collections.Generic;
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

        /// <summary>
        ///     Если не OCO значение будет -1 всегда
        /// </summary>
        [JsonPropertyName("orderListId")]
        public long OrderListId { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        [JsonPropertyName("price")]
        public double Price { get; set; }

        /// <summary>
        ///     Запрошенное кол-во
        /// </summary>
        [JsonPropertyName("origQty")]
        public double OrigQty { get; set; }

        /// <summary>
        ///     Исполненное кол-во
        /// </summary>
        [JsonPropertyName("executedQty")]
        public double ExecutedQty { get; set; }

        /// <summary>
        ///     Кол-во совокупной котировки
        /// </summary>
        [JsonPropertyName("cummulativeQuoteQty")]
        public double СumulativeQuoteQty { get; set; }

        /// <summary>
        ///     Статус выполнения ордера
        /// </summary>
        public OrderStatusType Status => status.ConvertToOrderStatusType();

        /// <summary>
        ///     Статус выполнения ордера (нужен для парса)
        /// </summary>
        [JsonPropertyName("status")]
        public string status { get; set; }

        /// <summary>
        ///     Время жизни ордера
        /// </summary>
        public TimeInForceType TimeInForce => timeInForce.ConvertToTimeInForceType();

        /// <summary>
        ///     Время жизни ордера
        /// </summary>
        [JsonPropertyName("timeInForce")]
        public string timeInForce { get; set; }

        /// <summary>
        ///     Тип ордера
        /// </summary>
        public OrderType OrderType => orderType.ConvertToOrderType();

        /// <summary>
        ///     Тип ордера
        /// </summary>
        [JsonPropertyName("type")]
        public string orderType { get; set; }

        /// <summary>
        ///     Тип ордера (покупка, продажа)
        /// </summary>
        public OrderSideType OrderSide => side.ConvertToOrderSideType();

        /// <summary>
        ///     Тип ордера (покупка, продажа)
        /// </summary>
        [JsonPropertyName("side")]
        public string side { get; set; }

        [JsonPropertyName("fills")]
        public IEnumerable<FillModel> Fills { get; set; }
    }

    /// <summary>
    ///     Содержит информацию о частях заполнения ордера
    /// </summary>
    public class FillModel
    {
        /// <summary>
        ///     Цена
        /// </summary>
        [JsonPropertyName("price")]
        public double Price { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        [JsonPropertyName("qty")]
        public double Quantity { get; set; }

        /// <summary>
        ///     Коммиссия
        /// </summary>
        [JsonPropertyName("commission")]
        public double Commission { get; set; }

        /// <summary>
        ///     Актив комиссии
        /// </summary>
        [JsonPropertyName("commissionAsset")]
        public string CommissionAsset { get; set; }

        /// <summary>
        ///     Id сделки
        /// </summary>
        [JsonPropertyName("tradeId")]
        public long TradeId { get; set; }
    }
}
