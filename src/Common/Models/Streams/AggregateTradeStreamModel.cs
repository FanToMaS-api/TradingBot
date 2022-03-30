namespace Common.Models
{
    /// <summary>
    ///     Модель данных с потока торговой информации, которая агрегируется для одного ордера тейкера
    /// </summary>
    public class AggregateTradeStreamModel : MarketdataStreamModelBase
    {
        /// <summary>
        ///     Совокупное Id сделки
        /// </summary>
        public long AggregateTradeId { get; internal set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        ///     Объем сделки
        /// </summary>
        public double Quantity { get; internal set; }

        /// <summary>
        ///     Первое Id сделки
        /// </summary>
        public long FirstTradeId { get; internal set; }

        /// <summary>
        ///     Последнее Id сделки
        /// </summary>
        public long LastTradeId { get; internal set; }

        /// <summary>
        ///     Время Id сделки
        /// </summary>
        public long TradeTimeUnix { get; internal set; }

        /// <summary>
        ///     Является ли покупатель маркет-мейкером?
        /// </summary>
        public bool IsMarketMaker { get; internal set; }
    }
}
