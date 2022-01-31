namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Тип ограничения скорости
    /// </summary>
    public enum RateLimiterType
    {
        /// <summary>
        ///     Запрос
        /// </summary>
        REQUEST_WEIGHT,

        /// <summary>
        ///     Заказы
        /// </summary>
        ORDERS,

        /// <summary>
        ///     Необработанные запросы
        /// </summary>
        RAW_REQUESTS,
    }
}
