using Analytic.Models;
using System;
using System.Linq;

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
        /// <param name="tradeObjectName"> Название объекта торговли. <see langword="null"/> - для фильтрации всех </param>
        /// <param name="comparisonType"> Тип фильтра цен </param>
        /// <param name="limit"> Ограничение </param>
        public PriceDeviationFilter(string filterName, string tradeObjectName, ComparisonType comparisonType, double limit, int timeframeNumber = 5)
        {
            FilterName = filterName;
            TargetTradeObjectName = tradeObjectName;
            ComparisonType = comparisonType;
            Limit = limit;
            TimeframeNumber = timeframeNumber;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string FilterName { get; }

        /// <inheritdoc />
        public string TargetTradeObjectName { get; }

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
        public bool CheckConditions(InfoModel model)
        {
            var summDeviations = model.PricePercentDeviations.Take(TimeframeNumber).Sum();
            model.SumDeviations = summDeviations;

            return ComparisonType switch
                {
                    ComparisonType.GreaterThan => summDeviations > Limit,
                    ComparisonType.LessThan => summDeviations < Limit,
                    ComparisonType.Equal => summDeviations == Limit,
                    _ => throw new NotImplementedException(),
                };
        }

        #endregion
    }
}
