﻿using Analytic.Models;
using System;
using System.Collections.Generic;
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
        /// <param name="tradeObjectName"> Название объекта торговли. <see langword="null"/> - для фильтрации всех </param>
        /// <param name="filterType"> Тип фильтра цен </param>
        /// <param name="limit"> Ограничение </param>
        public PriceFilter(string filterName, string tradeObjectName, PriceFilterType filterType, double limit)
        {
            FilterName = filterName;
            TargetTradeObjectName = tradeObjectName;
            FilterType = filterType;
            Limit = limit;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string FilterName { get; }

        /// <summary>
        ///     Название объекта торговли
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> - для фильтрации всех
        /// </remarks>
        public string TargetTradeObjectName { get; }

        /// <summary>
        ///     Тип фильтра цен
        /// </summary>
        public PriceFilterType FilterType { get; }

        /// <summary>
        ///     Ограничение
        /// </summary>
        public double Limit { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public bool CheckConditions(InfoModel model)
        {
            var summDeviations = model.PricePercentDeviations.Take(5).Sum();
            model.Sum5Deviations = summDeviations;

            return FilterType switch
                {
                    PriceFilterType.GreaterThan => summDeviations > Limit,
                    PriceFilterType.LessThan => summDeviations < Limit,
                    PriceFilterType.Equal => summDeviations == Limit,
                    _ => false
                };
        }

        #endregion
    }
}
