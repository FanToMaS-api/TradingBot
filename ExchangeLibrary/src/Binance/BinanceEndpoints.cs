namespace ExchangeLibrary.Binance
{
    /// <summary>
    ///     Сожаржит все конечные точки Binance
    /// </summary>
    public static class BinanceEndpoints
    {
        #region Wallet

        /// <summary>
        ///     Статус системы
        /// </summary>
        public const string SYSTEM_STATUS = "/sapi/v1/system/status";

        /// <summary>
        ///     Информация обо всех монетах
        /// </summary>
        public const string ALL_COINS_INFORMATION = "/sapi/v1/capital/config/getall";

        /// <summary>
        ///     Ежедневный отчет об аккаунте
        /// </summary>
        public const string DAILY_ACCOUNT_SNAPSHOT = "/sapi/v1/accountSnapshot";

        /// <summary>
        ///     Статус аккаунта (трейдинг)
        /// </summary>
        public const string ACCOUNT_API_TRADING_STATUS = "/sapi/v1/account/apiTradingStatus";

        /// <summary>
        ///     Такса за торговлю
        /// </summary>
        public const string TRADE_FEE = "/sapi/v1/asset/tradeFee";

        #endregion

        #region Marketdata

        /// <summary>
        ///     Книга ордеров
        /// </summary>
        public const string ORDER_BOOK = "/api/v3/depth";

        /// <summary>
        ///     Недавние сделки
        /// </summary>
        public const string RECENT_TRADES = "/api/v3/trades";

        /// <summary>
        ///     Исторические сделки
        /// </summary>
        public const string OLD_TRADES = "/api/v3/historicalTrades";

        #endregion
    }
}
