using BinanceDatabase.Enums;

namespace BinanceDataService.Configuration.AggregatorConfigs
{
    /// <summary>
    ///     Класс с настройками агрегации данных по однпятнадцать минут
    /// </summary>
    internal sealed class FifteenMinutesAggregatorConfig : AggregatorConfigBase
    {
        /// <summary>
        ///     Период агрегирования данных <see cref="AggregateDataIntervalType.FifteenMinutes"/>
        /// </summary>
        public override AggregateDataIntervalType AggregateDataInterval => AggregateDataIntervalType.FifteenMinutes;
    }
}
