namespace ExchangeLibrary.Binance.Enums.Helper
{
    /// <summary>
    ///     Расширяет enum'ы
    /// </summary>
    internal static class EnumHelper
    {
        /// <summary>
        ///     Переводит период свечи в формате бинанса в удобный для работы <see cref="CandleStickIntervalType"/>
        /// </summary>
        public static CandleStickIntervalType ConvertToCandleStickIntervalType(this string intervalType) =>
            intervalType switch
            {
                "1m" => CandleStickIntervalType.OneMinute,
                "3m" => CandleStickIntervalType.ThreeMinutes,
                "5m" => CandleStickIntervalType.FiveMinutes,
                "15m" => CandleStickIntervalType.FifteenMinutes,
                "30m" => CandleStickIntervalType.ThirtyMinutes,
                "1h" => CandleStickIntervalType.OneHour,
                "2h" => CandleStickIntervalType.TwoHour,
                "4h" => CandleStickIntervalType.FourHours,
                "6h" => CandleStickIntervalType.SixHours,
                "8h" => CandleStickIntervalType.EightHours,
                "12h" => CandleStickIntervalType.TwelveHours,
                "1d" => CandleStickIntervalType.OneDay,
                "3d" => CandleStickIntervalType.ThreeDays,
                "1w" => CandleStickIntervalType.OneWeek,
                "1M" => CandleStickIntervalType.OneMonth,
                _ => throw new System.Exception($"Failed to convert '{intervalType}' to {nameof(CandleStickIntervalType)}"),
            };

        /// <summary>
        ///     Переводит статус ордера в формате бинанса в удобный для работы <see cref="OrderStatusType"/>
        /// </summary>
        public static OrderStatusType ConvertToOrderStatusType(this string statusType) =>
            statusType switch
            {
                "NEW" => OrderStatusType.NEW,
                "FILLED" => OrderStatusType.FILLED,
                "PARTIALLY_FILLED" => OrderStatusType.PARTIALLY_FILLED,
                "EXPIRED" => OrderStatusType.EXPIRED,
                "CANCELED" => OrderStatusType.CANCELED,
                "REJECTED" => OrderStatusType.REJECTED,
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
                "LIMIT" => OrderType.LIMIT,
                "LIMIT_MAKER" => OrderType.LIMIT_MAKER,
                "MARKET" => OrderType.MARKET,
                "STOP_LOSS" => OrderType.STOP_LOSS,
                "STOP_LOSS_LIMIT" => OrderType.STOP_LOSS_LIMIT,
                "TAKE_PROFIT" => OrderType.TAKE_PROFIT,
                "TAKE_PROFIT_LIMIT" => OrderType.TAKE_PROFIT_LIMIT,
                _ => throw new System.Exception($"Failed to convert '{type}' to {nameof(OrderType)}"),
            };

        /// <summary>
        ///     Переводит тип ордера в формате бинанса в удобный для работы <see cref="OrderSideType"/>
        /// </summary>
        public static OrderSideType ConvertToOrderSideType(this string type) =>
            type switch
            {
                "BUY" => OrderSideType.BUY,
                "SELL" => OrderSideType.SELL,
                _ => throw new System.Exception($"Failed to convert '{type}' to {nameof(OrderSideType)}"),
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
        public static string ToUrl(this CandleStickIntervalType intervalType) =>
            intervalType switch
            {
                CandleStickIntervalType.OneMinute => "1m",
                CandleStickIntervalType.ThreeMinutes => "3m",
                CandleStickIntervalType.FiveMinutes => "5m",
                CandleStickIntervalType.FifteenMinutes => "15m",
                CandleStickIntervalType.ThirtyMinutes => "30m",
                CandleStickIntervalType.OneHour => "1h",
                CandleStickIntervalType.TwoHour => "2h",
                CandleStickIntervalType.FourHours => "4h",
                CandleStickIntervalType.SixHours => "6h",
                CandleStickIntervalType.EightHours => "8h",
                CandleStickIntervalType.TwelveHours => "12h",
                CandleStickIntervalType.OneDay => "1d",
                CandleStickIntervalType.ThreeDays => "3d",
                CandleStickIntervalType.OneWeek => "1w",
                CandleStickIntervalType.OneMonth => "1M",
                _ => intervalType.ToString(),
            };

        /// <summary>
        ///     Получить строку из типа для отправки запроса бинансу
        /// </summary>
        public static string ToUrl(this OrderSideType type)
        {
            return type switch
            {
                OrderSideType.SELL => "SELL",
                OrderSideType.BUY => "BUY",
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
                OrderType.LIMIT => "LIMIT",
                OrderType.LIMIT_MAKER => "LIMIT_MAKER",
                OrderType.MARKET => "MARKET",
                OrderType.STOP_LOSS => "STOP_LOSS",
                OrderType.STOP_LOSS_LIMIT => "STOP_LOSS_LIMIT",
                OrderType.TAKE_PROFIT => "TAKE_PROFIT",
                OrderType.TAKE_PROFIT_LIMIT => "TAKE_PROFIT_LIMIT",
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
                OrderResponseType.ACK => "ACK",
                OrderResponseType.RESULT => "RESULT",
                OrderResponseType.FULL => "FULL",
                _ => type.ToString(),
            };
        }
    }
}
