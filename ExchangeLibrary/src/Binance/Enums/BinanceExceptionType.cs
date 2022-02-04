namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Типы исключений возникающий при работе с Binance
    /// </summary>
    public enum BinanceExceptionType
    {
        /// <summary>
        ///     Неверно сформированный запрос на стороне клиента. StatusCode: 4XX, кроме 403, 429, 418 - они определяются другим типом
        /// </summary>
        InvalidRequest,

        /// <summary>
        ///     Отказано в доступе брандмауэром. StatusCode: 403, возникает при ограничениях брандмауэра
        /// </summary>
        WAFLimit,

        /// <summary>
        ///     Превышена скорость запроса. StatusCode: 429
        /// </summary>
        RateLimit,

        /// <summary>
        ///     IP адрес заблокирован после многократного получения <see cref="RateLimit"/>. StatusCode: 418
        /// </summary>
        Blocked,

        /// <summary>
        ///     Ошибка на стороне сервера. !Это НЕ говорит о неудачной операции!. Состояние неизвестно и могло быть выполнено успешно
        /// </summary>
        ServerException
    }
}
