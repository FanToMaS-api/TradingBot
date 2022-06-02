using Analytic.Models;
using Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.Filters.FilterGroup.Impl
{
    /// <summary>
    ///     Группа фильтров
    /// </summary>
    public class FilterGroup : IFilterGroup
    {
        #region .ctor

        /// <summary>
        ///     Группа фильтров
        /// </summary>
        /// <param name="filterGroupName"> Название группы </param>
        /// <param name="filterGroupType"> Тип группы </param>
        /// <param name="targetTradeObjectName">
        ///     Название объекта торговли <br />
        /// <remarks>
        ///     <see langword="null"/> - для фильтрации всех
        /// </remarks>
        /// </param>
        public FilterGroup(string filterGroupName, FilterGroupType filterGroupType, string targetTradeObjectName)
        {
            FilterGroupName = filterGroupName;
            Type = filterGroupType;
            TargetTradeObjectName = targetTradeObjectName;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string FilterGroupName { get; }

        /// <inheritdoc />
        public List<IFilter> Filters { get; } = new();

        /// <inheritdoc />
        public FilterGroupType Type { get; }

        /// <summary>
        ///     Название объекта торговли
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> - для фильтрации всех
        /// </remarks>
        public string TargetTradeObjectName { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void AddFilter(IFilter filter) => Filters.Add(filter);

        /// <inheritdoc />
        public async Task<bool> CheckConditionsAsync(IServiceScopeFactory serviceScopeFactory, InfoModel model, CancellationToken cancellationToken)
            => await Filters.TrueForAllAsync(async _ => await _.CheckConditionsAsync(serviceScopeFactory, model, cancellationToken));

        /// <inheritdoc />
        public bool ContainsFilter(string filterName) => Filters.Any(_ => _.FilterName == filterName);

        /// <inheritdoc />
        public bool IsFilter(string tradeObjectName) =>
            !string.IsNullOrEmpty(TargetTradeObjectName)
            && tradeObjectName.Contains(TargetTradeObjectName, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        public bool RemoveFilter(IFilter filter) => Filters.Remove(filter);

        /// <inheritdoc />
        public object Clone()
        {
            var clone = new FilterGroup(FilterGroupName, Type, TargetTradeObjectName);
            clone.Filters.AddRange(Filters.Select(_ => (IFilter)_.Clone()));

            return clone;
        }

        #endregion
    }
}
