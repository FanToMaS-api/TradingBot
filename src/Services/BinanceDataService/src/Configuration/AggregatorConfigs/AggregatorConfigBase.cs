using BinanceDatabase.Enums;

namespace BinanceDataService.Configuration.AggregatorConfigs
{
    /// <summary>
    ///     Базовый класс настроек агрегаторов
    /// </summary>
    internal abstract class AggregatorConfigBase
    {
        /// <summary>
        ///     Определяет будет ли производится агрегированние холодных данных
        /// </summary>
        public bool IsNeedToAggregateColdData { get; set; }

        /// <summary>
        ///     Определяет период агрегирования данных, сохраненных в бд
        /// </summary>
        public string AggregateDataCron { get; set; }

        /// <summary>
        ///     Период агрегирования данных
        /// </summary>
        public abstract AggregateDataIntervalType AggregateDataInterval { get; }
    }
}
