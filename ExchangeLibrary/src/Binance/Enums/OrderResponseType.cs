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
        Ack,

        /// <summary>
        ///     Результат
        /// </summary>
        Result,

        /// <summary>
        ///     Полный
        /// </summary>
        Full
    }
}
