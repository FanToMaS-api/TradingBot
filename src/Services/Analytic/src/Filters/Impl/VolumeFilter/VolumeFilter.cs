using Analytic.Models;
using System;

namespace Analytic.Filters
{
    /// <summary>
    ///     Фильтр объемов спроса и предложения
    /// </summary>
    public class VolumeFilter : IFilter
    {
        #region .ctor

        /// <summary>
        ///     Фильтр объемов спроса и предложения
        /// </summary>
        /// <param name="filterName"> Название фильтра </param>
        /// <param name="volumeType"> Тип объемов для фильтрации </param>
        /// <param name="volumeComparisonType"> Тип фильтра </param>
        /// <param name="percentDeviation"> Отклонение для объемов при дефолтной фильтрации </param>
        /// <param name="limit"> Порог при недефолтной фильтрации </param>
        public VolumeFilter(
            string filterName,
            VolumeType volumeType = VolumeType.Default,
            VolumeComparisonType volumeComparisonType = VolumeComparisonType.Default,
            double percentDeviation = 0.3,
            double? limit = null)
        {
            FilterName = filterName;
            Limit = limit;
            VolumeType = volumeType;
            VolumeComparisonType = volumeComparisonType;
            PercentDeviation = percentDeviation;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string FilterName { get; }

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
            : VolumeComparisonType switch
                {
                    VolumeComparisonType.Default => model.BidVolume > model.AskVolume * (1 + PercentDeviation),
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
