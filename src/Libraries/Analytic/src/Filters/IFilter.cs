using Analytic.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="serviceScopeFactory"> Фабрика для получения сервисов </param>
        Task<bool> CheckConditionsAsync(IServiceScopeFactory serviceScopeFactory, InfoModel model, CancellationToken cancellationToken);
    }
}
