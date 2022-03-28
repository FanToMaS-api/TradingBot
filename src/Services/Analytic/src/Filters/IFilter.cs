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
        ///     Фильтрует данные
        /// </summary>
        InfoModel[] Filter(InfoModel[] models);
    }
}
