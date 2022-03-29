﻿using Analytic.Models;
using System;

namespace Analytic.Filters
{
    /// <summary>
    ///     Фильтр объемов купли/продажи
    /// </summary>
    public class VolumeFilter : IFilter
    {
        #region .ctor

        /// <summary>
        ///     Фильтр объемов купли/продажи
        /// </summary>
        /// <param name="filterName"> Название фильтра </param>
        /// <param name="tradeObjectName">
        ///     Название объекта торговли <br/>
        ///     <see langword="null"/> - для фильтрации всех
        /// </param>
        /// <param name="volumeComparisonType"> Тип фильтра </param>
        /// <param name="volumeType"> Тип объемов для фильтрации </param>
        /// <param name="percentDeviation"> Отклонение для объемов при дефолтной фильтрации </param>
        /// <param name="limit"> Порог при недефолтной фильтрации </param>
        public VolumeFilter(
            string filterName,
            string tradeObjectName,
            VolumeComparisonType volumeComparisonType = VolumeComparisonType.Default,
            VolumeType volumeType = VolumeType.Default,
            double percentDeviation = 0.3,
            double? limit = null)
        {
            FilterName = filterName;
            TargetTradeObjectName = tradeObjectName;
            VolumeComparisonType = volumeComparisonType;
            Limit = limit;
            VolumeType = volumeType;
            PercentDeviation = percentDeviation;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string FilterName { get; }

        /// <inheritdoc />
        public string TargetTradeObjectName { get; }

        /// <summary>
        ///     Тип сравнения объемов
        /// </summary>
        public VolumeComparisonType VolumeComparisonType { get; }

        /// <summary>
        ///     Тип фильтруемых объемов
        /// </summary>
        public VolumeType VolumeType { get; }

        /// <summary>
        ///     Ограничение
        /// </summary>
        public double? Limit { get; }

        /// <summary>
        ///     Базовое отклонение объема спроса от объема предложения
        /// </summary>
        public double PercentDeviation { get; }

        /// <inheritdoc />
        public FilterType Type => FilterType.VolumeFilter;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public bool CheckConditions(InfoModel model) =>
            VolumeComparisonType != VolumeComparisonType.Default && !Limit.HasValue && VolumeType == VolumeType.Default
            ? throw new Exception("A non-default filter type is selected but no filtering limit is specified")
            : (TargetTradeObjectName is null || model.TradeObjectName == TargetTradeObjectName)
                && VolumeComparisonType switch
                {
                    VolumeComparisonType.Default => model.AskVolume * (1 + PercentDeviation) > model.BidVolume,
                    VolumeComparisonType.GreaterThan => IsSatisfiesCondition(model, _ => _ > Limit),
                    VolumeComparisonType.LessThan => IsSatisfiesCondition(model, _ => _ < Limit),
                    _ => throw new NotImplementedException(),
                };

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверка условия в соответствии с типом фильтруемых объемов
        /// </summary>
        /// <param name="filterFunc"> Функция для фильтрации </param>
        private bool IsSatisfiesCondition(InfoModel model, Func<double, bool> filterFunc) =>
            VolumeType switch
            {
                VolumeType.Default => false,
                VolumeType.Bid => filterFunc(model.BidVolume),
                VolumeType.Ask => filterFunc(model.AskVolume),
                _ => throw new NotImplementedException(),
            };

        #endregion
    }
}
