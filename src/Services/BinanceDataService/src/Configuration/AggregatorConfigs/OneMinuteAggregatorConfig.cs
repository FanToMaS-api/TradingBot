using BinanceDatabase.Enums;

namespace BinanceDataService.Configuration.AggregatorConfigs
{
    /// <summary>
    ///     Класс с настройками агрегации данных по одной минуте
    /// </summary>
    internal sealed class OneMinuteAggregatorConfig : AggregatorConfigBase
    {
        /// <summary>
        ///     Период агрегирования данных <see cref="AggregateDataIntervalType.OneMinute"/>
        /// </summary>
        public override AggregateDataIntervalType AggregateDataInterval => AggregateDataIntervalType.OneMinute;
    }
}
