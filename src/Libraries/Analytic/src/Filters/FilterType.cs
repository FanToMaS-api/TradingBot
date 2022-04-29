namespace Analytic.Filters
{
    /// <summary>
    ///     Тип фильтра
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        ///     Фильтр наименования объекта торговли
        /// </summary>
        NameFilter,

        /// <summary>
        ///     Фильтр цены
        /// </summary>
        PriceFilter,

        /// <summary>
        ///     Фильтр отклонения цены
        /// </summary>
        PriceDeviationFilter,

        /// <summary>
        ///     Фильтр объемов
        /// </summary>
        VolumeFilter,
    }
}