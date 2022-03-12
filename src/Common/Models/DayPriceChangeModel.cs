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
        public string Symbol { get; set; }

        /// <summary>
        ///     Изменение цены
        /// </summary>
        public double PriceChange { get; set; }

        /// <summary>
        ///     Изменение цены в процентах
        /// </summary>
        public double PriceChangePercent { get; set; }

        /// <summary>
        ///     Взвешенная средняя цена
        /// </summary>
        public double WeightedAvgPrice { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public double PrevClosePrice { get; set; }

        /// <summary>
        ///     Последняя цена
        /// </summary>
        public double LastPrice { get; set; }

        /// <summary>
        ///     Последний объем
        /// </summary>
        public double LastQty { get; set; }

        /// <summary>
        ///     Цена спроса
        /// </summary>
        public double BidPrice { get; set; }

        /// <summary>
        ///     Объем спроса
        /// </summary>
        public double BidQty { get; set; }

        /// <summary>
        ///     Цена предложения
        /// </summary>
        public double AskPrice { get; set; }

        /// <summary>
        ///     Объем предложения
        /// </summary>
        public double AskQty { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        public double HighPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        public double LowPrice { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        ///     Объем котировки
        /// </summary>
        public double QuoteVolume { get; set; }

        /// <summary>
        ///     Время открытия
        /// </summary>
        public long OpenTimeUnix { get; set; }

        /// <summary>
        ///     Время закрытия
        /// </summary>
        public long CloseTimeUnix { get; set; }

        /// <summary>
        ///     Id первой сделки
        /// </summary>
        public long FirstId { get; set; }

        /// <summary>
        ///     Id последеней сделки
        /// </summary>
        public long LastId { get; set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        public long Count { get; set; }
    }
}
