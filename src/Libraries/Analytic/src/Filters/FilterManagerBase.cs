using Analytic.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.Filters
{
    /// <summary>
    ///     Класс, управляющий фильтрацией
    /// </summary>
    public abstract class FilterManagerBase
    {
        /// <summary>
        ///     Фильтры данных
        /// </summary>
        public List<IFilterGroup> FilterGroups { get; } = new();

        /// <summary>
        ///     Возвращает отфильтрованные данные
        /// </summary>
        public abstract Task<IEnumerable<InfoModel>> GetFilteredDataAsync<T>(
            IServiceScopeFactory serviceScopeFactory,
            IEnumerable<T> modelsToFilter,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Добавить группу фильтров
        /// </summary>
        public void AddFilterGroup(IFilterGroup filterGroup) => FilterGroups.Add(filterGroup);

        /// <summary>
        ///     Удалить группу фильтров
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        public bool RemoveFilterGroup(IFilterGroup filterGroup) => FilterGroups.Remove(filterGroup);

        /// <summary>
        ///     Удалить фильтр
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        public bool RemoveFilter(string filterName)
        {
            foreach (var group in FilterGroups)
            {
                if (group.ContainsFilter(filterName))
                {
                    return FilterGroups.Remove(group);
                }
            }

            return false;
        }
    }
}
