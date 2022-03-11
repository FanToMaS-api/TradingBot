namespace Common.Models
{
    /// <summary>
    ///     Модель лучшей цены/кол-ва из стакана для пары
    /// </summary>
    public class BestSymbolOrderModel
    {
        /// <summary>
        ///     Название пары
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     Лучшая цена спроса
        /// </summary>
        public double BidPrice { get; set; }

        /// <summary>
        ///     Лучшее кол-во спроса
        /// </summary>
        public double BidQuantity { get; set; }

        /// <summary>
        ///     Лучшая цена предложения
        /// </summary>
        public double AskPrice { get; set; }

        /// <summary>
        ///     Лучшее кол-во предложения
        /// </summary>
        public double AskQuantity { get; set; }
    }
}
