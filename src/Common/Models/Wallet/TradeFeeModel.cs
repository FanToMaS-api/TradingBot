namespace Common.Models
{
    /// <summary>
    ///     Содержит размеры комиссии для объекта торговли
    /// </summary>
    public class TradeFeeModel
    {
        #region Properties

        /// <summary>
        ///     Краткое обозначение объекта торговли
        /// </summary>
        public string ShortName { get; internal set; }

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
