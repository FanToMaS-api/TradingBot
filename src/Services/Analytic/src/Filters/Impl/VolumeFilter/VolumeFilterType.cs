namespace Analytic.Filters
{
    /// <summary>
    ///     Тип фильтра объемов
    /// </summary>
    public enum VolumeFilterType
    {
        /// <summary>
        ///     Объем покупки должен быть больше объема продажи
        /// </summary>
        Default,

        /// <summary>
        ///     Больше чем
        /// </summary>
        GreaterThan,

        /// <summary>
        ///     Меньше чем
        /// </summary>
        LessThan
    }
}
