using System.Net.Http;

namespace ExchangeLibrary.Binance
{
    /// <summary>
    ///     Содержит все конечные точки Binance
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
        ///     Ордера из стакана
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

        /// <summary>
        ///     Свечи для монеты
        /// </summary>
        public const string CANDLESTICK_DATA = "/api/v3/klines";

        /// <summary>
        ///     Текущая средняя цена монеты
        /// </summary>
        public const string AVERAGE_PRICE = "/api/v3/avgPrice";

        /// <summary>
        ///     Изменение цены пары за 24 часа
        /// </summary>
        public const string DAY_PRICE_CHANGE = "/api/v3/ticker/24hr";

        /// <summary>
        ///     Последняя цена пары или пар
        /// </summary>
        public const string SYMBOL_PRICE_TICKER = "/api/v3/ticker/price";

        /// <summary>
        ///     Лучшая цена/количество в стакане для символа или символов
        /// </summary>
        public const string SYMBOL_ORDER_BOOK_TICKER = "/api/v3/ticker/bookTicker";

        #endregion

        #region Spot Account/Trade

        /// <summary>
        ///     Отправить новый ТЕСТОВЫЙ ордер <see cref="HttpMethod.Post"/>
        /// </summary>
        public const string NEW_TEST_ORDER = "/api/v3/order/test";

        /// <summary>
        ///     Отправить новый ордер <see cref="HttpMethod.Post"/>
        /// </summary>
        public const string NEW_ORDER = "/api/v3/order";

        /// <summary>
        ///     Отменить ордер по опред паре <see cref="HttpMethod.Delete"/>
        /// </summary>
        public const string CANCEL_ORDER = "/api/v3/order";

        /// <summary>
        ///     Отменить все ордера по опред паре <see cref="HttpMethod.Delete"/>
        /// </summary>
        public const string CANCEL_All_ORDERS = "/api/v3/openOrders";

        #endregion
    }
}
