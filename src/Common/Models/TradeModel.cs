namespace Common.Models
{
    /// <summary>
    ///     Модель сделки
    /// </summary>
    public class TradeModel
    {
        /// <summary>
        ///     Уникальный идентификатор
        /// </summary>
        public long Id { get; internal set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        public double Quantity { get; internal set; }

        /// <summary>
        ///     Кол-во квотируемой
        /// </summary>
        public double QuoteQty { get; internal set; }

        /// <summary>
        ///     Время сделки
        /// </summary>
        public long TimeUnix { get; internal set; }

        /// <summary>
        ///     Была ли покупка по указанной покупателем цене
        /// </summary>
        public bool IsBuyerMaker { get; internal set; }

        /// <summary>
        ///     Была ли встречная сделка
        /// </summary>
        public bool IsBestMatch { get; internal set; }
    }
}
