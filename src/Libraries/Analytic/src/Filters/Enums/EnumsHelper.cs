using System;

namespace Analytic.Filters.Enums
{
    /// <summary>
    ///     Помогает в работе с перечислениями
    /// </summary>
    internal static class EnumsHelper
    {
        /// <summary>
        ///     Кастит <see cref="AggregateDataIntervalType"/> к <see cref="BinanceDatabase.Enums.AggregateDataIntervalType"/>
        /// </summary>
        public static BinanceDatabase.Enums.AggregateDataIntervalType CastToBinanceDataAggregateType(
            this AggregateDataIntervalType aggregateDataIntervalType)
            => aggregateDataIntervalType switch
            {
                AggregateDataIntervalType.Default => BinanceDatabase.Enums.AggregateDataIntervalType.Default,
                AggregateDataIntervalType.OneMinute => BinanceDatabase.Enums.AggregateDataIntervalType.OneMinute,
                AggregateDataIntervalType.FiveMinutes => BinanceDatabase.Enums.AggregateDataIntervalType.FiveMinutes,
                AggregateDataIntervalType.FifteenMinutes => BinanceDatabase.Enums.AggregateDataIntervalType.FifteenMinutes,
                AggregateDataIntervalType.OneHour => BinanceDatabase.Enums.AggregateDataIntervalType.OneHour,
                _ => throw new NotImplementedException($"Unknown type of {nameof(aggregateDataIntervalType)}")
            };
    }
}
