using Analytic.Models;
using System.Linq;

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
            FilterName = filterName;
            ComparisonType = comparisonType;
            Limit = limit;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string FilterName { get; }

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

        /// <inheritdoc />
        public bool CheckConditions(InfoModel model) =>
            ComparisonType switch
            {
                ComparisonType.GreaterThan => model.LastPrice > Limit,
                ComparisonType.LessThan => model.LastPrice < Limit,
                ComparisonType.Equal => model.LastPrice == Limit,
                _ => false
            };

        #endregion
    }
}
