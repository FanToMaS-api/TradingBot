namespace Common.Models
{
    /// <summary>
    ///     Модель данных с потока необработанной торговой информации; у каждой сделки есть уникальный покупатель и продавец
    /// </summary>
    public class TradeStreamModel : MarketdataStreamModelBase
    {
        /// <summary>
        ///     Идентификатор заказа продавца
        /// </summary>
        public long SellerOrderId { get; internal set; }

        /// <summary>
        ///     Идентификатор заказа покупателя
        /// </summary>
        public long BuyerOrderId { get; internal set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        ///     Объем сделки
        /// </summary>
        public double Quantity { get; internal set; }

        /// <summary>
        ///     Id сделки
        /// </summary>
        public long TradeId { get; internal set; }

        /// <summary>
        ///     Время сделки
        /// </summary>
        public long TradeTimeUnix { get; internal set; }

        /// <summary>
        ///     Является ли покупатель маркет-мейкером?
        /// </summary>
        public bool IsMarketMaker { get; internal set; }
    }
}
