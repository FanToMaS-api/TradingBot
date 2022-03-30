namespace Common.Models
{
    /// <summary>
    ///     Модель текущей цены объекта торговли
    /// </summary>
    public class TradeObjectNamePriceModel
    {
        /// <summary>
        ///     Название объекта торговли
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; internal set; }
    }
}
