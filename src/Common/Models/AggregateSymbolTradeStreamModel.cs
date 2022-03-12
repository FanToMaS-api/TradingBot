namespace Common.Models
{
    /// <summary>
    ///     Модель данных с потока торговой информации, которая агрегируется для одного ордера тейкера
    /// </summary>
    public class AggregateSymbolTradeStreamModel : MarketdataStreamModelBase
    {
        /// <summary>
        ///     Совокупное Id сделки
        /// </summary>
        public long AggregateTradeId { get; set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Объем сделки
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        ///     Первое Id сделки
        /// </summary>
        public long FirstTradeId { get; set; }

        /// <summary>
        ///     Последнее Id сделки
        /// </summary>
        public long LastTradeId { get; set; }

        /// <summary>
        ///     Время Id сделки
        /// </summary>
        public long TradeTimeUnix { get; set; }

        /// <summary>
        ///     Является ли покупатель маркет-мейкером?
        /// </summary>
        public bool IsMarketMaker { get; set; }
    }
}
