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
        public long KineStartTimeUnix { get; internal set; }

        /// <summary>
        ///     Время закрытия свечи
        /// </summary>
        public long KineStopTimeUnix { get; internal set; }

        /// <summary>
        ///     Интервал
        /// </summary>
        public string Interval { get; internal set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        public double OpenPrice { get; internal set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        public double MinPrice { get; internal set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        public double MaxPrice { get; internal set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Volume { get; internal set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public double ClosePrice { get; internal set; }

        /// <summary>
        ///     Закрыта ли свеча
        /// </summary>
        public bool IsKlineClosed { get; internal set; }

        /// <summary>
        ///     Объем котируемого актива
        /// </summary>
        public double QuoteAssetVolume { get; internal set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        public int TradesNumber { get; internal set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        public double BasePurchaseVolume { get; internal set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        public double QuotePurchaseVolume { get; internal set; }
    }
}
