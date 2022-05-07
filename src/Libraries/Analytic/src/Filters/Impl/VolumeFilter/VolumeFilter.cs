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
        public VolumeFilter(
            string filterName,
            VolumeType volumeType = VolumeType.Bid,
            VolumeComparisonType volumeComparisonType = VolumeComparisonType.GreaterThan,
            double percentDeviation = 0.05)
        {
            FilterName = filterName;
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
        ///     Базовое отклонение объема спроса от объема предложения
        /// </summary>
        public double PercentDeviation { get; }

        /// <inheritdoc />
        public FilterType Type => FilterType.VolumeFilter;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public bool CheckConditions(InfoModel model) =>
            VolumeComparisonType switch
            {
                VolumeComparisonType.GreaterThan => IsSatisfiesCondition(model, (x, y) => x > y * (1 + PercentDeviation)),
                VolumeComparisonType.LessThan => IsSatisfiesCondition(model, (x, y) => x < y * (1 + PercentDeviation)),
                _ => throw new NotImplementedException(),
            };

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверка условия в соответствии с типом фильтруемых объемов
        /// </summary>
        /// <param name="filterFunc"> Функция для фильтрации </param>
        private bool IsSatisfiesCondition(InfoModel model, Func<double, double, bool> filterFunc) =>
            VolumeType switch
            {
                VolumeType.Bid => filterFunc(model.BidVolume, model.AskVolume),
                VolumeType.Ask => filterFunc(model.AskVolume, model.BidVolume),
                _ => throw new NotImplementedException(),
            };

        #endregion
    }
}
