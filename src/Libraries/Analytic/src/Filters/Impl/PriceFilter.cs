using Analytic.Filters.Enums;
using Analytic.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.Filters
{
    /// <summary>
    ///     Фильтр цен
    /// </summary>
    public class PriceFilter : IFilter
    {
        #region .ctor

        /// <summary>
        ///     Фильтр цен
        /// </summary>
        /// <param name="filterName"> Название фильтра </param>
        /// <param name="comparisonType"> Тип фильтра цен </param>
        /// <param name="limit"> Ограничение </param>
        public PriceFilter(string filterName, ComparisonType comparisonType, double limit)
        {
            Name = filterName;
            ComparisonType = comparisonType;
            Limit = limit;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Тип сравнения
        /// </summary>
        public ComparisonType ComparisonType { get; }

        /// <summary>
        ///     Ограничение
        /// </summary>
        public double Limit { get; }

        /// <inheritdoc />
        public FilterType Type => FilterType.PriceFilter;

        #endregion

        #region Public methods

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        
        /// <inheritdoc />
        public async Task<bool> CheckConditionsAsync(
            IServiceScopeFactory _,
            InfoModel model,
            CancellationToken cancellationToken) =>
            ComparisonType switch
            {
                ComparisonType.GreaterThan => model.LastPrice > Limit,
                ComparisonType.LessThan => model.LastPrice < Limit,
                ComparisonType.Equal => model.LastPrice == Limit,
                _ => false
            };

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        #endregion
    }
}
