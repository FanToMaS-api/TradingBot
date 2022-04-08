using System;

namespace BinanceDatabase.Enums
{
    /// <summary>
    ///     Расширяет enum'ы
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        ///     Переводит интервал объединения в TimeSpan
        /// </summary>
        public static TimeSpan ConvertToTimeSpan(this AggregateDataIntervalType intervalType) =>
            intervalType switch
            {
                AggregateDataIntervalType.Default => TimeSpan.Zero,
                AggregateDataIntervalType.OneMinute => TimeSpan.FromMinutes(1),
                AggregateDataIntervalType.FiveMinutes => TimeSpan.FromMinutes(5),
                AggregateDataIntervalType.FifteenMinutes => TimeSpan.FromMinutes(15),
                AggregateDataIntervalType.OneHour => TimeSpan.FromHours(1),
                _ => throw new NotImplementedException($"Unknow interval type {intervalType}")
            };
    }
}
