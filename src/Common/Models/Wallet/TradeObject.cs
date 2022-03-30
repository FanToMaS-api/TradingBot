namespace Common.Models
{
    /// <summary>
    ///     Модель объекта торговли
    /// </summary>
    public class TradeObject
    {
        /// <summary>
        ///     Кратное наименовение
        /// </summary>
        public string ShortName { get; internal set; }

        /// <summary>
        ///     Полное наименование
        /// </summary>
        public string Name { get; internal set; }
    }
}
