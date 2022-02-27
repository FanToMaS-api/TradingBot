namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Сколько ордер будет активен пока не истечет
    /// </summary>
    internal enum TimeInForceType
    {
        /// <summary>
        ///     Good Til Canceled - ордер будет висеть до тех пор, пока его не отменят (по-умолчанию)
        /// </summary>
        GTC,

        /// <summary>
        ///     Immediate Or Cancel - будет куплено то количество, которое можно купить немедленно. Все, что не удалось купить, будет отменено
        /// </summary>
        IOC,

        /// <summary>
        /// 	Fill or Kill - либо будет куплено все указанное количество немедленно, либо не будет куплено вообще ничего, ордер отменится
        /// </summary>
        FOK,
    }
}
