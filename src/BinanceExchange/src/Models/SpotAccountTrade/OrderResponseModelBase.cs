using BinanceExchange.Enums;
using BinanceExchange.Enums.Helper;
using System.Text.Json;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Содержит общие св-ва ответа на запросы связанные с ордерами
    /// </summary>
    internal class OrderResponseModelBase
    {
        /// <summary>
        ///     Пара
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     Id ордера
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        ///     Id клиентского ордера
        /// </summary>
        public string ClientOrderId { get; set; }

        /// <summary>
        ///     Если не OCO значение будет -1 всегда
        /// </summary>
        public long OrderListId { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Запрошенное кол-во
        /// </summary>
        public double OrigQty { get; set; }

        /// <summary>
        ///     Исполненное кол-во
        /// </summary>
        public double ExecutedQty { get; set; }

        /// <summary>
        ///     Кол-во совокупной котировки
        /// </summary>
        public double CumulativeQuoteQty { get; set; }

        /// <summary>
        ///     Статус выполнения ордера
        /// </summary>
        public OrderStatusType Status { get; set; }

        /// <summary>
        ///     Время жизни ордера
        /// </summary>
        public TimeInForceType TimeInForce { get; set; }

        /// <summary>
        ///     Тип ордера
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        ///     Тип ордера (покупка, продажа)
        /// </summary>
        public OrderSideType OrderSide { get; set; }

        /// <summary>
        ///     Пробует установить св-во
        /// </summary>
        private protected void SetProperty(string propertyName, ref Utf8JsonReader reader)
        {
            switch (propertyName)
            {
                case "symbol":
                    Symbol = reader.GetString();
                    return;
                case "orderId":
                    OrderId = reader.GetInt64();
                    return;
                case "clientOrderId":
                    ClientOrderId = reader.GetString();
                    return;
                case "orderListId":
                    OrderListId = reader.GetInt64();
                    return;
                case "price":
                    Price = double.Parse(reader.GetString());
                    return;
                case "origQty":
                    OrigQty = double.Parse(reader.GetString());
                    return;
                case "executedQty":
                    ExecutedQty = double.Parse(reader.GetString());
                    return;
                case "cummulativeQuoteQty":
                    CumulativeQuoteQty = double.Parse(reader.GetString());
                    return;
                case "status":
                    Status = reader.GetString().ConvertToOrderStatusType();
                    return;
                case "timeInForce":
                    TimeInForce = reader.GetString().ConvertToTimeInForceType();
                    return;
                case "type":
                    OrderType = reader.GetString().ConvertToOrderType();
                    return;
                case "side":
                    OrderSide = reader.GetString().ConvertToOrderSideType();
                    return;
                default:
                    throw new System.Exception($"Unknown property name '{propertyName}'");
            }
        }
    }
}
