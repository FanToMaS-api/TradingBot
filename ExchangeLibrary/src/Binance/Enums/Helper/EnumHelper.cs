using Common.Enums;
using ExchangeLibrary.Binance.Enums;

namespace ExchangeLibrary.src.Binance.Enums.Helper
{
    /// <summary>
    ///     Расширяет enum'ы
    /// </summary>
    internal static class EnumHelper
    {
        /// <summary>
        ///     Возвращает период свечи в формате необходимом бинансу
        /// </summary>
        public static string GetInterval(this CandleStickIntervalType intervalType) =>
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
        ///     Возвращает конечную точку для указанной версии API
        /// </summary>
        public static string GetEndPoint(this ApiVersionType type)
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
    }
}
