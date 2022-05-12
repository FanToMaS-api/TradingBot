using BinanceDatabase.Enums;

namespace BinanceDataService.Configuration.AggregatorConfigs
{
    /// <summary>
    ///     Класс с настройками агрегации данных по пять минут
    /// </summary>
    internal sealed class FiveMinutesAggregatorConfig : AggregatorConfigBase
    {
        /// <summary>
        ///     Период агрегирования данных <see cref="AggregateDataIntervalType.FiveMinutes"/>
        /// </summary>
        public override AggregateDataIntervalType AggregateDataInterval => AggregateDataIntervalType.FiveMinutes;
    }
}
