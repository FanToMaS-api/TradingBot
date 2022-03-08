namespace Common.Models
{
    /// <summary>
    ///     Модель статистики бегущего окна за 24 часа для одного символа
    /// </summary>
    public class TickerStreamModel : MarketdataStreamModelBase
    {
        /// <summary>
        ///    Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///    Изменение цены в процентах
        /// </summary>
        public double PricePercentChange { get; set; }

        /// <summary>
        ///    Средневзвешенная цена
        /// </summary>
        public double WeightedAveragePrice { get; set; }

        /// <summary>
        ///    Цена самой первой сделки до 24-х часового скользящего окна
        /// </summary>
        public double FirstPrice { get; set; }

        /// <summary>
        ///    Последняя цена
        /// </summary>
        public double LastPrice { get; set; }

        /// <summary>
        ///    Последнее кол-во
        /// </summary>
        public double LastQuantity { get; set; }

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

        /// <summary>
        ///    Цена открытия
        /// </summary>
        public double OpenPrice { get; set; }

        /// <summary>
        ///    Максимальная цена
        /// </summary>
        public double MaxPrice { get; set; }

        /// <summary>
        ///    Минимальная цена
        /// </summary>
        public double MinPrice { get; set; }

        /// <summary>
        ///    Общий торгуемый объем базовых активов
        /// </summary>
        public double AllBaseVolume { get; set; }

        /// <summary>
        ///    Общий торгуемый объем котировочного актива
        /// </summary>
        public double AllQuoteVolume { get; set; }

        /// <summary>
        ///    Время открытия статистики
        /// </summary>
        public long StatisticOpenTimeUnix { get; set; }

        /// <summary>
        ///    Время закрытия статистики
        /// </summary>
        public long StatisticCloseTimeUnix { get; set; }

        /// <summary>
        ///    Id первой сделки
        /// </summary>
        public long FirstTradeId { get; set; }

        /// <summary>
        ///    Id последней сделки
        /// </summary>
        public long LastTradeId { get; set; }

        /// <summary>
        ///    Число сделок
        /// </summary>
        public long TradeNumber { get; set; }
    }
}
