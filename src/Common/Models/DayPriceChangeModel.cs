namespace Common.Models
{
    /// <summary>
    ///     Модель изменения цены за 1 день по паре
    /// </summary>
    public class DayPriceChangeModel
    {
        /// <summary>
        ///     Наименование пары
        /// </summary>
        public string Symbol { get; internal set; }

        /// <summary>
        ///     Изменение цены
        /// </summary>
        public double PriceChange { get; internal set; }

        /// <summary>
        ///     Изменение цены в процентах
        /// </summary>
        public double PriceChangePercent { get; internal set; }

        /// <summary>
        ///     Взвешенная средняя цена
        /// </summary>
        public double WeightedAvgPrice { get; internal set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public double PrevClosePrice { get; internal set; }

        /// <summary>
        ///     Последняя цена
        /// </summary>
        public double LastPrice { get; internal set; }

        /// <summary>
        ///     Последний объем
        /// </summary>
        public double LastQty { get; internal set; }

        /// <summary>
        ///     Цена спроса
        /// </summary>
        public double BidPrice { get; internal set; }

        /// <summary>
        ///     Объем спроса
        /// </summary>
        public double BidQty { get; internal set; }

        /// <summary>
        ///     Цена предложения
        /// </summary>
        public double AskPrice { get; internal set; }

        /// <summary>
        ///     Объем предложения
        /// </summary>
        public double AskQty { get; internal set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        public double OpenPrice { get; internal set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        public double HighPrice { get; internal set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        public double LowPrice { get; internal set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Volume { get; internal set; }

        /// <summary>
        ///     Объем котировки
        /// </summary>
        public double QuoteVolume { get; internal set; }

        /// <summary>
        ///     Время открытия
        /// </summary>
        public long OpenTimeUnix { get; internal set; }

        /// <summary>
        ///     Время закрытия
        /// </summary>
        public long CloseTimeUnix { get; internal set; }

        /// <summary>
        ///     Id первой сделки
        /// </summary>
        public long FirstId { get; internal set; }

        /// <summary>
        ///     Id последеней сделки
        /// </summary>
        public long LastId { get; internal set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        public long Count { get; internal set; }
    }
}
