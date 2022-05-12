using BinanceDataService.Configuration.AggregatorConfigs;

namespace BinanceDataService.Configuration
{
    /// <summary>
    ///     Конфигурация обработчика данных со стримов маркетдаты для тикеров
    /// </summary>
    internal class MarketdataStreamHandlerConfig
    {
        /// <summary>
        ///     Название блока настроек
        /// </summary>
        public static string Name => "MarketdataStreamHandler";

        /// <summary>
        ///     Определяет период сохранения данных, полученных от веб-сокета
        /// </summary>
        public string SaveDataCron { get; set; }

        /// <summary>
        ///     Настройки агрегации данных по одной минуте
        /// </summary>
        public OneMinuteAggregatorConfig OneMinuteAggregator { get; set; }

        /// <summary>
        ///     Настройки агрегации данных по пять минут
        /// </summary>
        public FiveMinutesAggregatorConfig FiveMinutesAggregator { get; set; }

        /// <summary>
        ///     Настройки агрегации данных по пятнадцать минут
        /// </summary>
        public FifteenMinutesAggregatorConfig FifteenMinutesAggregator { get; set; }

        /// <summary>
        ///     Настройки агрегации данных по одному часу
        /// </summary>
        public OneHourAggregatorConfig OneHourAggregator { get; set; }

        /// <summary>
        ///     Определяет период удаления данных, сохраненных в бд
        /// </summary>
        public string DeleteDataCron { get; set; }
    }
}
