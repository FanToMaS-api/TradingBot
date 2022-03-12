namespace Common.Models
{
    /// <summary>
    ///     Модель данных обновления лучшей цены или количества спроса или предложения
    ///     в режиме реального времени для указанного символа
    /// </summary>
    public class BookTickerStreamModel
    {
        /// <summary>
        ///     Идентификатор обновления книги заказов
        /// </summary>
        public long OrderBookUpdatedId { get; set; }

        /// <summary>
        ///     Имя пары
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///    Лучшая цена спроса
        /// </summary>
        public double BestBidPrice { get; set; }

        /// <summary>
        ///    Лучшая объем спроса
        /// </summary>
        public double BestBidQuantity { get; set; }

        /// <summary>
        ///    Лучшая цена предложения
        /// </summary>
        public double BestAskPrice { get; set; }

        /// <summary>
        ///    Лучшая объем предложения
        /// </summary>
        public double BestAskQuantity { get; set; }
    }
}
