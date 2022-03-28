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
        public string ShortName { get; internal set; }

        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; internal set; }
    }
}
