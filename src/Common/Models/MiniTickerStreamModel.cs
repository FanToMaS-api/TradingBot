namespace Common.Models
{
    /// <summary>
    ///     Модель индивидуального потока мини-тикера символа
    /// </summary>
    public class MiniTickerStreamModel : MarketdataStreamModelBase
    {
        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public double ClosePrice { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        public double MinPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        public double MaxPrice { get; set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        public double BasePurchaseVolume { get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        public double QuotePurchaseVolume { get; set; }
    }
}
