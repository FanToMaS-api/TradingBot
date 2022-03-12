namespace Common.Models
{
    /// <summary>
    ///     Модель текущей цены пары
    /// </summary>
    public class SymbolPriceModel
    {
        /// <summary>
        ///     Название пары
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; set; }
    }
}
