using System.Collections.Generic;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Группа профилей
    /// </summary>
    public interface IProfileGroup : IAnalyticUnit
    {
        /// <summary>
        ///     Профили аналитики
        /// </summary>
        List<IAnalyticUnit> AnalyticUnits { get; }

        /// <summary>
        ///     Добавить профиль аналитики
        /// </summary>
        void AddAnalyticUnit(IAnalyticUnit unit);
    }
}
