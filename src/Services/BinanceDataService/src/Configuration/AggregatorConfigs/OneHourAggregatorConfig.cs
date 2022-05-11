using BinanceDatabase.Enums;

namespace BinanceDataService.Configuration.AggregatorConfigs
{
    /// <summary>
    ///     Класс с настройками агрегации данных по одному часу
    /// </summary>
    internal sealed class OneHourAggregatorConfig : AggregatorConfigBase
    {
        /// <summary>
        ///     Период агрегирования данных <see cref="AggregateDataIntervalType.OneHour"/>
        /// </summary>
        public override AggregateDataIntervalType AggregateDataInterval => AggregateDataIntervalType.OneHour;
    }
}
