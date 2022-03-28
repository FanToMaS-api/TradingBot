using Analytic.Models;
using System;
using System.Collections.Generic;

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
        public InfoModel[] Filter(InfoModel[] models)
        {
            if (FilterType != VolumeFilterType.Default && !Limit.HasValue && VolumeType == VolumeType.Default)
            {
                throw new Exception("A non-default filter type is selected but no filtering limit is specified");
            }

            var result = new List<InfoModel>();
            foreach (var model in models)
            {
                if (TradeObjectName is not null && model.TradeObjectName != TradeObjectName)
                {
                    continue;
                }

                switch (FilterType)
                {
                    case VolumeFilterType.Default:
                        if (model.AskVolume * (1 + PercentDeviation) > model.BidVolume)
                        {
                            result.Add(model);
                        }

                        break;

                    case VolumeFilterType.GreaterThan:
                        if (IsSatisfiesCondition(model, _ => _ > Limit))
                        {
                            result.Add(model);
                        }

                        break;
                    case VolumeFilterType.LessThan:
                        if (IsSatisfiesCondition(model, _ => _ < Limit))
                        {
                            result.Add(model);
                        }

                        break;
                }
            }

            return result.ToArray();
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
