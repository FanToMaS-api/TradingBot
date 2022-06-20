using Analytic.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        string Name { get; }

        /// <summary>
        ///     Тип фильтра
        /// </summary>
        FilterType Type { get; }

        /// <summary>
        ///     Проверяет модель на соответствие условию
        /// </summary>
        /// <param name="serviceScopeFactory"> Фабрика для получения сервисов </param>
        Task<bool> CheckConditionsAsync(IServiceScopeFactory serviceScopeFactory, InfoModel model, CancellationToken cancellationToken);
    }
}
