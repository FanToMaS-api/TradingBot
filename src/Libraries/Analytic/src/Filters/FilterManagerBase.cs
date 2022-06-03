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
        public IReadOnlyCollection<IFilterGroup> FilterGroups { init; get; }

        /// <summary>
        ///     Возвращает отфильтрованные данные
        /// </summary>
        public abstract Task<IEnumerable<InfoModel>> GetFilteredDataAsync<T>(
            IServiceScopeFactory serviceScopeFactory,
            IEnumerable<T> modelsToFilter,
            CancellationToken cancellationToken);
    }
}
