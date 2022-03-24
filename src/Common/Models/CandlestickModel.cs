namespace Common.Models
{
    /// <summary>
    ///     Модель свечи для объекта торговли
    /// </summary>
    public class CandlestickModel
    {
        /// <summary>
        ///     Время открытия
        /// </summary>
        public long OpenTimeUnix { get; internal set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        public double OpenPrice { get; internal set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        public double MinPrice { get; internal set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        public double MaxPrice { get; internal set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Volume { get; internal set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public double ClosePrice { get; internal set; }

        /// <summary>
        ///     Время закрытия
        /// </summary>
        public long CloseTimeUnix { get; internal set; }

        /// <summary>
        ///     Объем котируемого актива
        /// </summary>
        public double QuoteAssetVolume { get; internal set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        public int TradesNumber { get; internal set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        public double BasePurchaseVolume { get; internal set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        public double QuotePurchaseVolume { get; internal set; }
    }
}
