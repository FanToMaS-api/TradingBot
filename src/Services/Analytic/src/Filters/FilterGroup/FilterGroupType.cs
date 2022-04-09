namespace Analytic.Filters
{
    /// <summary>
    ///     Тип группы фильтров
    /// </summary>
    public enum FilterGroupType
    {
        /// <summary>
        ///     Первостепенный (вызываются в первую очередь)
        /// </summary>
        Primary,

        /// <summary>
        ///     Специальный (вызывается первее <see cref="Common"/>)
        /// </summary>
        Special,

        /// <summary>
        ///     Общий (для всех у кого нет специального)
        /// </summary>
        Common,

        /// <summary>
        ///     Специальные фильтры (вызываются в последнюю очередь, но первее <see cref="CommonLatest"/>)
        /// </summary>
        SpecialLatest,

        /// <summary>
        ///     Общие для всех фильтры (вызываются в последнюю очередь)
        /// </summary>
        CommonLatest,
    }
}
