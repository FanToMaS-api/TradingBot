namespace BinanceExchange.Enums
{
    /// <summary>
    ///     Состояния символа
    /// </summary>
    internal enum SymbolStatusType
    {
        /// <summary>
        ///     Предварительная торговля
        /// </summary>
        PreTrading,

        /// <summary>
        ///     Разрешена торговля
        /// </summary>
        Trading,

        /// <summary>
        ///     Завершающая торговля
        /// </summary>
        PostTrading,

        /// <summary>
        ///     Конец дня
        /// </summary>
        EndOfDay,

        /// <summary>
        ///     Остановлена торговля
        /// </summary>
        Halt,

        /// <summary>
        ///     Аукцион
        /// </summary>
        AuctionMatch,

        /// <summary>
        ///     Перерыв
        /// </summary>
        Break,
    }
}
