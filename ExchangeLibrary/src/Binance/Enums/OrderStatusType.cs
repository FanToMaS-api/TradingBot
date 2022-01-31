namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Статус заказа
    /// </summary>
    public enum OrderStatusType
    {
        /// <summary>
        ///     Заказ был принят
        /// </summary>
        NEW,

        /// <summary>
        ///     Часть заказа выполнена
        /// </summary>
        PARTIALLY_FILLED,

        /// <summary>
        ///     Заказ выполнен
        /// </summary>
        FILLED,

        /// <summary>
        ///     Заказ был отменен пользователем
        /// </summary>
        CANCELED,

        /// <summary>
        ///     Заказ не принят
        /// </summary>
        REJECTED,

        /// <summary>
        ///     Заказ был отменен
        /// </summary>
        EXPIRED
    }
}
