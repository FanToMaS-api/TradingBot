namespace Common.Models
{
    /// <summary>
    ///     Базовый интерфейс объекта торговли
    /// </summary>
    public interface ITrade
    {
        #region Properties

        /// <summary>
        ///     Название объекта торговли
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Цена за единицу
        /// </summary>
        public double Price { get; set; }

        #endregion
    }
}
