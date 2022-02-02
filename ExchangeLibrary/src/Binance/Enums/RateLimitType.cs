namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Тип ограничения скорости
    /// </summary>
    public enum RateLimitType
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

        /// <summary>
        ///     Информация обо всех монетах
        /// </summary>
        ALL_COINS_INFO,
    }
}
