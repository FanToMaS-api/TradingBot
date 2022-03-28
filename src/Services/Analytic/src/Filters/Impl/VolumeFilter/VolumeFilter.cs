using Analytic.Models;
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
        /// <param name="filterType"> Тип фильтра </param>
        /// <param name="volumeType"> Тип объемов для фильтрации </param>
        /// <param name="percentDeviation"> Отклонение для объемов при дефолтной фильтрации </param>
        /// <param name="limit"> Порог при недефолтной фильтрации </param>
        public VolumeFilter(
            string filterName,
            string tradeObjectName,
            VolumeFilterType filterType = VolumeFilterType.Default,
            VolumeType volumeType = VolumeType.Default,
            double percentDeviation = 0.3,
            double? limit = null)
        {
            FilterName = filterName;
            TradeObjectName = tradeObjectName;
            FilterType = filterType;
            Limit = limit;
            VolumeType = volumeType;
            PercentDeviation = percentDeviation;
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
        public string TradeObjectName { get; }

        /// <summary>
        ///     Тип фильтра объемов
        /// </summary>
        public VolumeFilterType FilterType { get; }

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

        #endregion

        #region Public methods

        /// <inheritdoc />
        public bool CheckConditions(InfoModel model)
        {
            return FilterType != VolumeFilterType.Default && !Limit.HasValue && VolumeType == VolumeType.Default
                ? throw new Exception("A non-default filter type is selected but no filtering limit is specified")
                : (TradeObjectName is null || model.TradeObjectName == TradeObjectName)
                    && FilterType switch
                    {
                        VolumeFilterType.Default => model.AskVolume * (1 + PercentDeviation) > model.BidVolume,
                        VolumeFilterType.GreaterThan => IsSatisfiesCondition(model, _ => _ > Limit),
                        VolumeFilterType.LessThan => IsSatisfiesCondition(model, _ => _ < Limit),
                        _ => throw new NotImplementedException(),
                    };
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверка условия в соответствии с типом фильтруемых объемов
        /// </summary>
        /// <param name="filterFunc"> Функция для фильтрации </param>
        private bool IsSatisfiesCondition(InfoModel model, Func<double, bool> filterFunc)
        {
            return VolumeType switch
            {
                VolumeType.Default => false,
                VolumeType.Bid => filterFunc(model.BidVolume),
                VolumeType.Ask => filterFunc(model.BidVolume),
                _ => false,
            };
        }

        #endregion
    }
}
