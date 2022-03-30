namespace Common.Models
{
    /// <summary>
    ///     Модель статистики бегущего окна за 24 часа для одного объекта торговли
    /// </summary>
    public class TradeObjectStreamModel : MarketdataStreamModelBase
    {
        /// <summary>
        ///    Цена
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        ///    Изменение цены в процентах
        /// </summary>
        public double PricePercentChange { get; internal set; }

        /// <summary>
        ///    Средневзвешенная цена
        /// </summary>
        public double WeightedAveragePrice { get; internal set; }

        /// <summary>
        ///    Цена самой первой сделки до 24-х часового скользящего окна
        /// </summary>
        public double FirstPrice { get; internal set; }

        /// <summary>
        ///    Последняя цена
        /// </summary>
        public double LastPrice { get; internal set; }

        /// <summary>
        ///    Последнее кол-во
        /// </summary>
        public double LastQuantity { get; internal set; }

        /// <summary>
        ///    Лучшая цена спроса
        /// </summary>
        public double BestBidPrice { get; internal set; }

        /// <summary>
        ///    Лучшая объем спроса
        /// </summary>
        public double BestBidQuantity { get; internal set; }

        /// <summary>
        ///    Лучшая цена предложения
        /// </summary>
        public double BestAskPrice { get; internal set; }

        /// <summary>
        ///    Лучшая объем предложения
        /// </summary>
        public double BestAskQuantity { get; internal set; }

        /// <summary>
        ///    Цена открытия
        /// </summary>
        public double OpenPrice { get; internal set; }

        /// <summary>
        ///    Максимальная цена
        /// </summary>
        public double MaxPrice { get; internal set; }

        /// <summary>
        ///    Минимальная цена
        /// </summary>
        public double MinPrice { get; internal set; }

        /// <summary>
        ///    Общий торгуемый объем базовых активов
        /// </summary>
        public double AllBaseVolume { get; internal set; }

        /// <summary>
        ///    Общий торгуемый объем котировочного актива
        /// </summary>
        public double AllQuoteVolume { get; internal set; }

        /// <summary>
        ///    Время открытия статистики
        /// </summary>
        public long StatisticOpenTimeUnix { get; internal set; }

        /// <summary>
        ///    Время закрытия статистики
        /// </summary>
        public long StatisticCloseTimeUnix { get; internal set; }

        /// <summary>
        ///    Id первой сделки
        /// </summary>
        public long FirstTradeId { get; internal set; }

        /// <summary>
        ///    Id последней сделки
        /// </summary>
        public long LastTradeId { get; internal set; }

        /// <summary>
        ///    Число сделок
        /// </summary>
        public long TradeNumber { get; internal set; }
    }
}
