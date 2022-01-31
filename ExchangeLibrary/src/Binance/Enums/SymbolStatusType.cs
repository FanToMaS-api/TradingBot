namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Состояния символа
    /// </summary>
    public enum SymbolStatusType
    {
        /// <summary>
        ///     TODO: найти документацию
        /// </summary>
        PRE_TRADING,

        TRADING,

        POST_TRADING,

        END_OF_DAY,

        HALT,

        AUCTION_MATCH,

        BREAK,
    }
}
