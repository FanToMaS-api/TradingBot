namespace ExchangeLibrary.Binance.Enums.Helper
{
    /// <summary>
    ///     Расширяет enum'ы
    /// </summary>
    internal static class EnumHelper
    {
        /// <summary>
        ///     Переводит период свечи в формате бинанса в удобный для работы <see cref="CandlestickIntervalType"/>
        /// </summary>
        public static CandlestickIntervalType ConvertToCandleStickIntervalType(this string intervalType) =>
            intervalType switch
            {
                "1m" => CandlestickIntervalType.OneMinute,
                "3m" => CandlestickIntervalType.ThreeMinutes,
                "5m" => CandlestickIntervalType.FiveMinutes,
                "15m" => CandlestickIntervalType.FifteenMinutes,
                "30m" => CandlestickIntervalType.ThirtyMinutes,
                "1h" => CandlestickIntervalType.OneHour,
                "2h" => CandlestickIntervalType.TwoHour,
                "4h" => CandlestickIntervalType.FourHours,
                "6h" => CandlestickIntervalType.SixHours,
                "8h" => CandlestickIntervalType.EightHours,
                "12h" => CandlestickIntervalType.TwelveHours,
                "1d" => CandlestickIntervalType.OneDay,
                "3d" => CandlestickIntervalType.ThreeDays,
                "1w" => CandlestickIntervalType.OneWeek,
                "1M" => CandlestickIntervalType.OneMonth,
                _ => throw new System.Exception($"Failed to convert '{intervalType}' to {nameof(CandlestickIntervalType)}"),
            };

        /// <summary>
        ///     Переводит тип стрима в формате бинанса в удобный для работы <see cref="MarketdataStreamType"/>
        /// </summary>
        public static MarketdataStreamType ConvertToMarketdataStreamType(this string streamType)
        {
            return streamType switch
            {
                "@trade" => MarketdataStreamType.TradeStream,
                "@kline_" => MarketdataStreamType.CandlestickStream,
                "@miniTicker" => MarketdataStreamType.IndividualSymbolMiniTickerStream,
                "@aggTrade" => MarketdataStreamType.AggregateTradeStream,
                "!miniTicker@arr" => MarketdataStreamType.AllMarketMiniTickersStream,
                "@ticker" => MarketdataStreamType.IndividualSymbolTickerStream,
                "!ticker@arr" => MarketdataStreamType.AllMarketTickersStream,
                "!bookTicker" => MarketdataStreamType.AllBookTickersStream,
                "@depth" => MarketdataStreamType.PartialBookDepthStream,
                _ => throw new System.Exception($"Failed to convert '{streamType}' to {nameof(MarketdataStreamType)}"),
            };
        }

        /// <summary>
        ///     Переводит статус ордера в формате бинанса в удобный для работы <see cref="OrderStatusType"/>
        /// </summary>
        public static OrderStatusType ConvertToOrderStatusType(this string statusType) =>
            statusType switch
            {
                "NEW" => OrderStatusType.New,
                "FILLED" => OrderStatusType.Filled,
                "PARTIALLY_FILLED" => OrderStatusType.PartiallyFilled,
                "EXPIRED" => OrderStatusType.Expired,
                "CANCELED" => OrderStatusType.Canceled,
                "REJECTED" => OrderStatusType.Rejected,
                _ => throw new System.Exception($"Failed to convert '{statusType}' to {nameof(OrderStatusType)}"),
            };

        /// <summary>
        ///     Переводит время жизни ордера в формате бинанса в удобный для работы <see cref="TimeInForceType"/>
        /// </summary>
        public static TimeInForceType ConvertToTimeInForceType(this string type) =>
            type switch
            {
                "GTC" => TimeInForceType.GTC,
                "FOK" => TimeInForceType.FOK,
                "IOC" => TimeInForceType.IOC,
                _ => throw new System.Exception($"Failed to convert '{type}' to {nameof(TimeInForceType)}"),
            };

        /// <summary>
        ///     Переводит тип ордера в формате бинанса в удобный для работы <see cref="OrderType"/>
        /// </summary>
        public static OrderType ConvertToOrderType(this string type) =>
            type switch
            {
                "LIMIT" => OrderType.Limit,
                "LIMIT_MAKER" => OrderType.LimitMaker,
                "MARKET" => OrderType.Market,
                "STOP_LOSS" => OrderType.StopLoss,
                "STOP_LOSS_LIMIT" => OrderType.StopLossLimit,
                "TAKE_PROFIT" => OrderType.TakeProfit,
                "TAKE_PROFIT_LIMIT" => OrderType.TakeProfitLimit,
                _ => throw new System.Exception($"Failed to convert '{type}' to {nameof(OrderType)}"),
            };

        /// <summary>
        ///     Переводит тип ордера в формате бинанса в удобный для работы <see cref="OrderSideType"/>
        /// </summary>
        public static OrderSideType ConvertToOrderSideType(this string type) =>
            type switch
            {
                "BUY" => OrderSideType.Buy,
                "SELL" => OrderSideType.Sell,
                _ => throw new System.Exception($"Failed to convert '{type}' to {nameof(OrderSideType)}"),
            };

        /// <summary>
        ///     Переводит тип ордера в формате бинанса в удобный для работы <see cref="SymbolStatusType"/>
        /// </summary>
        public static SymbolStatusType ConvertToSymbolStatusType(this string type) =>
            type switch
            {
                "AUCTION_MATCH" => SymbolStatusType.AuctionMatch,
                "BREAK" => SymbolStatusType.Break,
                "END_OF_DAY" => SymbolStatusType.EndOfDay,
                "HALT" => SymbolStatusType.Halt,
                "POST_TRADING" => SymbolStatusType.PostTrading,
                "PRE_TRADING" => SymbolStatusType.PreTrading,
                "TRADING" => SymbolStatusType.Trading,
                _ => throw new System.Exception($"Failed to convert '{type}' to {nameof(SymbolStatusType)}"),
            };

        /// <summary>
        ///     Возвращает конечную точку для указанной версии API
        /// </summary>
        public static string GetEndpoint(this ApiVersionType type)
        {
            return type switch
            {
                ApiVersionType.Default => "https://api.binance.com",
                ApiVersionType.ApiV1 => "https://api1.binance.com",
                ApiVersionType.ApiV2 => "https://api2.binance.com",
                ApiVersionType.ApiV3 => "https://api3.binance.com",
                _ => type.ToString(),
            };
        }

        /// <summary>
        ///     Отобразить
        /// </summary>
        public static string Display(this BinanceExceptionType? exceptionType)
        {
            return exceptionType switch
            {
                BinanceExceptionType.InvalidRequest => "Неверно сформированный запрос на стороне клиента",
                BinanceExceptionType.WAFLimit => " Отказано в доступе брандмауэром",
                BinanceExceptionType.RateLimit => "Превышена скорость запроса",
                BinanceExceptionType.Blocked => "IP адрес заблокирован",
                BinanceExceptionType.ServerException => "Ошибка на стороне сервера (это НЕ говорит о неудачной операции)",
                null => "",
                _ => exceptionType.ToString(),
            };
        }

        /// <summary>
        ///     Получить строку из типа для отправки запроса бинансу
        /// </summary>
        public static string ToUrl(this MarketdataStreamType streamType)
        {
            return streamType switch
            {
                MarketdataStreamType.AggregateTradeStream => "@aggTrade",
                MarketdataStreamType.TradeStream => "@trade",
                MarketdataStreamType.CandlestickStream => "@kline_",
                MarketdataStreamType.IndividualSymbolMiniTickerStream => "@miniTicker",
                MarketdataStreamType.AllMarketMiniTickersStream => "!miniTicker@arr",
                MarketdataStreamType.IndividualSymbolTickerStream => "@ticker",
                MarketdataStreamType.AllMarketTickersStream => "!ticker@arr",
                MarketdataStreamType.AllBookTickersStream => "!bookTicker",
                MarketdataStreamType.PartialBookDepthStream => "@depth",
                _ => streamType.ToString(),
            };
        }

        /// <summary>
        ///     Возвращает период свечи в формате необходимом бинансу
        /// </summary>
        public static string ToUrl(this CandlestickIntervalType intervalType) =>
            intervalType switch
            {
                CandlestickIntervalType.OneMinute => "1m",
                CandlestickIntervalType.ThreeMinutes => "3m",
                CandlestickIntervalType.FiveMinutes => "5m",
                CandlestickIntervalType.FifteenMinutes => "15m",
                CandlestickIntervalType.ThirtyMinutes => "30m",
                CandlestickIntervalType.OneHour => "1h",
                CandlestickIntervalType.TwoHour => "2h",
                CandlestickIntervalType.FourHours => "4h",
                CandlestickIntervalType.SixHours => "6h",
                CandlestickIntervalType.EightHours => "8h",
                CandlestickIntervalType.TwelveHours => "12h",
                CandlestickIntervalType.OneDay => "1d",
                CandlestickIntervalType.ThreeDays => "3d",
                CandlestickIntervalType.OneWeek => "1w",
                CandlestickIntervalType.OneMonth => "1M",
                _ => intervalType.ToString(),
            };

        /// <summary>
        ///     Получить строку из типа для отправки запроса бинансу
        /// </summary>
        public static string ToUrl(this OrderSideType type)
        {
            return type switch
            {
                OrderSideType.Sell => "SELL",
                OrderSideType.Buy => "BUY",
                _ => type.ToString(),
            };
        }

        /// <summary>
        ///     Получить строку из типа для отправки запроса бинансу
        /// </summary>
        public static string ToUrl(this OrderType type)
        {
            return type switch
            {
                OrderType.Limit => "LIMIT",
                OrderType.LimitMaker => "LIMIT_MAKER",
                OrderType.Market => "MARKET",
                OrderType.StopLoss => "STOP_LOSS",
                OrderType.StopLossLimit => "STOP_LOSS_LIMIT",
                OrderType.TakeProfit => "TAKE_PROFIT",
                OrderType.TakeProfitLimit => "TAKE_PROFIT_LIMIT",
                _ => type.ToString(),
            };
        }

        /// <summary>
        ///     Получить строку из типа для отправки запроса бинансу
        /// </summary>
        public static string ToUrl(this TimeInForceType type)
        {
            return type switch
            {
                TimeInForceType.GTC => "GTC",
                TimeInForceType.FOK => "FOK",
                TimeInForceType.IOC => "IOC",
                _ => type.ToString(),
            };
        }

        /// <summary>
        ///     Получить строку из типа для отправки запроса бинансу
        /// </summary>
        public static string ToUrl(this OrderResponseType type)
        {
            return type switch
            {
                OrderResponseType.Ack => "ACK",
                OrderResponseType.Result => "RESULT",
                OrderResponseType.Full => "FULL",
                _ => type.ToString(),
            };
        }
    }
}
