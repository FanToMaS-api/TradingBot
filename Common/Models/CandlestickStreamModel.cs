namespace Common.Models
{
    /// <summary>
    ///     Модель потока обновления информации о свече пары
    /// </summary>
    public class CandlestickStreamModel : MarketdataStreamModelBase
    {
        /// <summary>
        ///     Время открытия свечи
        /// </summary>
        public long KineStartTimeUnix { get; set; }

        /// <summary>
        ///     Время закрытия свечи
        /// </summary>
        public long KineStopTimeUnix { get; set; }

        /// <summary>
        ///     Интервал
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        public double MinPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        public double MaxPrice { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public double ClosePrice { get; set; }

        /// <summary>
        ///     Закрыта ли свеча
        /// </summary>
        public bool IsKlineClosed { get; set; }

        /// <summary>
        ///     Объем котируемого актива
        /// </summary>
        public double QuoteAssetVolume { get; set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        public int TradesNumber { get; set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        public double BasePurchaseVolume { get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        public double QuotePurchaseVolume { get; set; }
    }
}
