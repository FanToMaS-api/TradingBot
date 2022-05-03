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
        public string Name => "MarketdataStreamHandler";

        /// <summary>
        ///     Определяет период сохранения данных, полученных от веб-сокета
        /// </summary>
        public string SaveDataCron { get; set; }

        /// <summary>
        ///     Определяет период агрегирования данных, сохраненных в бд
        /// </summary>
        public string AggregateDataCron { get; set; }

        /// <summary>
        ///     Определяет период удаления данных, сохраненных в бд
        /// </summary>
        public string DeleteDataCron { get; set; }

        /// <summary>
        ///     Определяет будет ли производится сохранение и агрегированние холоддных данных
        /// </summary>
        public bool IsNeedToSaveColdData { get; set; }
    }
}
