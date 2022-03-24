namespace Common.Models
{
    /// <summary>
    ///     Содержит размеры комиссии по паре
    /// </summary>
    public class TradeFeeModel
    {
        #region Properties

        /// <summary>
        ///     Обозначение пары
        /// </summary>
        public string Symbol { get; internal set; }

        /// <summary>
        ///     Коммисия продавца
        /// </summary>
        public double MakerCommission { get; internal set; }

        /// <summary>
        ///     Коммисия покупателя
        /// </summary>
        public double TakerCommission { get; internal set; }

        #endregion
    }
}
