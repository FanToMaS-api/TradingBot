using Analytic.Models;
using System;

namespace Analytic.Filters
{
    /// <summary>
    ///     Фильтр цен
    /// </summary>
    public class PriceDeviationFilter : IFilter
    {
        #region .ctor

        /// <summary>
        ///     Фильтр цен
        /// </summary>
        /// <param name="filterName"> Название фильтра </param>
        /// <param name="comparisonType"> Тип фильтра цен </param>
        /// <param name="limit"> Ограничение </param>
        /// <param name="timeframeNumber"> Кол-во таймфреймов участвующих в анализе </param>
        public PriceDeviationFilter(string filterName, ComparisonType comparisonType, double limit, int timeframeNumber = 5)
        {
            FilterName = filterName;
            ComparisonType = comparisonType;
            Limit = limit;
            TimeframeNumber = timeframeNumber;
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
        public FilterType Type => FilterType.PriceDeviationFilter;

        /// <summary>
        ///     Кол-во таймфреймов участвующих в анализе
        /// </summary>
        public int TimeframeNumber { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        /// <remarks>
        ///     Данный метод также вычисляет значение <see cref="InfoModel.DeviationsSum"/>
        /// </remarks>
        public bool CheckConditions(InfoModel model)
        {
            model.ComputeDeviationsSum(TimeframeNumber);

            return ComparisonType switch
                {
                    ComparisonType.GreaterThan => model.DeviationsSum > Limit,
                    ComparisonType.LessThan => model.DeviationsSum < Limit,
                    ComparisonType.Equal => model.DeviationsSum == Limit,
                    _ => throw new NotImplementedException(),
                };
        }

        #endregion
    }
}
