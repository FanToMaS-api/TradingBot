using System.Collections.Generic;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Группа профилей аналитики
    /// </summary>
    public interface IProfileGroup : IAnalyticUnit
    {
        /// <summary>
        ///     Профили аналитики
        /// </summary>
        List<IAnalyticUnit> AnalyticUnits { get; }

        /// <summary>
        ///     Используется ли профиль
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        ///     Добавить профиль аналитики
        /// </summary>
        void AddAnalyticUnit(IAnalyticUnit unit);

        /// <summary>
        ///     Изменить статус профиля
        /// </summary>
        void ChangeProfileActivity(bool isAsctive);
    }
}
