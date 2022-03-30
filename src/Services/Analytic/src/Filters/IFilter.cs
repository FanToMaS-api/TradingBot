using Analytic.Models;

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
        ///     Тип фильтра
        /// </summary>
        public FilterType Type { get; }

        /// <summary>
        ///     Проверяет модель на соответствие условию
        /// </summary>
        bool CheckConditions(InfoModel model);
    }
}
