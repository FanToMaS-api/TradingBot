using Analytic.Models;
using System.Collections.Generic;

namespace Analytic.Filters
{
    /// <summary>
    ///     Общий интерфейс фильтров
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        ///     Название фильтра
        /// </summary>
        public string FilterName { get; }

        /// <summary>
        ///     Проверяет модель на соответствие условию
        /// </summary>
        bool CheckConditions(InfoModel model);
    }
}
