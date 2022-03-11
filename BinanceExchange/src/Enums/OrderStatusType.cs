namespace BinanceExchange.Enums
{
    /// <summary>
    ///     Статус ордера
    /// </summary>
    internal enum OrderStatusType
    {
        /// <summary>
        ///     Ордер был принят
        /// </summary>
        New,

        /// <summary>
        ///     Часть ордера выполнена
        /// </summary>
        PartiallyFilled,

        /// <summary>
        ///     Ордер выполнен
        /// </summary>
        Filled,

        /// <summary>
        ///     Ордер был отменен пользователем
        /// </summary>
        Canceled,

        /// <summary>
        ///     Ордер не принят
        /// </summary>
        Rejected,

        /// <summary>
        ///     Ордер был отменен
        /// </summary>
        Expired
    }
}
