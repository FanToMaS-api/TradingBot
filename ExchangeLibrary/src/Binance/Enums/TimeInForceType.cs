namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Сколько ордер будет активен пока не истечет
    /// </summary>
    public enum TimeInForceType
    {
        /// <summary>
        ///     Good Til Canceled - будет в книге пока не будет отменен
        /// </summary>
        GTC,

        /// <summary>
        ///     Immediate Or Cancel - Ордер будет пытаться выполнить ордер настолько, насколько это возможно, до истечения срока действия заказа.
        /// </summary>
        IOC,

        /// <summary>
        /// 	Fill or Kill - ордер истекает, если полный ордер не может быть исполнен
        /// </summary>
        FOK,
    }
}
