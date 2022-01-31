namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Типы исключений возникающий при работе с Binance
    /// </summary>
    public enum BinanceExceptionType
    {
        /// <summary>
        ///     Неверно сформированный запрос на стороне клиента
        /// </summary>
        /// <remarks>
        ///     StatusCode: 4XX, кроме 403, 429, 418 - они определяются другим типом
        /// </remarks>
        InvalidRequest,

        /// <summary>
        ///     Отказано в доступе брандмауэром
        /// </summary>
        /// <remarks>
        ///     StatusCode: 403, возникает при ограничениях брандмауэра
        /// </remarks>
        WAFLimit,

        /// <summary>
        ///     Превышена скорость запроса
        /// </summary>
        /// <remarks>
        ///     StatusCode: 429
        /// </remarks>
        RateLimit,

        /// <summary>
        ///     IP адрес заблокирован после многократного получения <see cref="RateLimit"/>
        /// </summary>
        /// <remarks>
        ///     StatusCode: 418
        /// </remarks>
        Blocked,

        /// <summary>
        ///     Ошибка на стороне сервера. !Это НЕ говорит о неудачной операции!
        /// </summary>
        /// <remarks>
        ///     Состояние неизвестно и могло быть выполнено успешно
        /// </remarks>
        ServerException
    }
}
