using Common.Enums;

namespace Common.Models
{
    /// <summary>
    ///     Содержит общие св-ва ответа на запросы связанные с ордерами
    /// </summary>
    public class OrderResponseModelBase
    {
        /// <summary>
        ///     Пара
        /// </summary>
        public string Symbol { get; internal set; }

        /// <summary>
        ///     Id ордера
        /// </summary>
        public long OrderId { get; internal set; }

        /// <summary>
        ///     Id клиентского ордера
        /// </summary>
        public string ClientOrderId { get; internal set; }

        /// <summary>
        ///     Если не OCO значение будет -1 всегда
        /// </summary>
        public long OrderListId { get; internal set; }

        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        ///     Запрошенное кол-во
        /// </summary>
        public double OrigQty { get; internal set; }

        /// <summary>
        ///     Исполненное кол-во
        /// </summary>
        public double ExecutedQty { get; internal set; }

        /// <summary>
        ///     Кол-во совокупной котировки
        /// </summary>
        public double CumulativeQuoteQty { get; internal set; }

        /// <summary>
        ///     Статус выполнения ордера
        /// </summary>
        public string Status { get; internal set; }

        /// <summary>
        ///     Время жизни ордера
        /// </summary>
        public string TimeInForce { get; internal set; }

        /// <summary>
        ///     Тип ордера
        /// </summary>
        public string OrderType { get; internal set; }

        /// <summary>
        ///     Тип ордера (покупка, продажа)
        /// </summary>
        public OrderSideType OrderSide { get; internal set; }
    }
}
