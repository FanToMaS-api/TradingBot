namespace Analytic.Filters
{
    /// <summary>
    ///     Тип сравнения объемов
    /// </summary>
    public enum VolumeComparisonType
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
