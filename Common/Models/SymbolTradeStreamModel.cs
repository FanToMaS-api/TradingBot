namespace Common.Models
{
    /// <summary>
    ///     Модель данных с потока необработанной торговой информации; у каждой сделки есть уникальный покупатель и продавец
    /// </summary>
    public class SymbolTradeStreamModel : MarketdataStreamModelBase
    {
        /// <summary>
        ///     Идентификатор заказа продавца
        /// </summary>
        public long SellerOrderId { get; set; }

        /// <summary>
        ///     Идентификатор заказа покупателя
        /// </summary>
        public long BuyerOrderId { get; set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Объем сделки
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        ///     Id сделки
        /// </summary>
        public long TradeId { get; set; }

        /// <summary>
        ///     Время сделки
        /// </summary>
        public long TradeTimeUnix { get; set; }

        /// <summary>
        ///     Является ли покупатель маркет-мейкером?
        /// </summary>
        public bool IsMarketMaker { get; set; }
    }
}
