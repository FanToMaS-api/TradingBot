namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///    Тип ответа на ордер 
    /// </summary>
    public enum OrderResponseType
    {
        /// <summary>
        ///     Подтверждение
        /// </summary>
        ACK,

        /// <summary>
        ///     Результат
        /// </summary>
        RESULT,

        /// <summary>
        ///     Полный
        /// </summary>
        FULL
    }
}
