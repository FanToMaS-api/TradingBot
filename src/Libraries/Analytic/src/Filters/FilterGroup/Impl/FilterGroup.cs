using Analytic.Models;
using Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public FilterGroup(
            string filterGroupName,
            FilterGroupType filterGroupType,
            string targetTradeObjectName,
            IList<IFilter> filters)
        {
            Name = filterGroupName;
            Type = filterGroupType;
            TargetTradeObjectName = targetTradeObjectName;
            Filters = new ReadOnlyCollection<IFilter>(filters);
        }

        /// <summary>
        ///     Создает группу фильтров
        /// </summary>
        internal FilterGroup() { }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IFilter> Filters { get; }

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
        public async Task<bool> CheckConditionsAsync(IServiceScopeFactory serviceScopeFactory, InfoModel model, CancellationToken cancellationToken)
            => await Filters.TrueForAllAsync(async _ => await _.CheckConditionsAsync(serviceScopeFactory, model, cancellationToken));

        /// <inheritdoc />
        public bool ContainsFilter(string filterName) => Filters.Any(_ => _.Name == filterName);

        /// <inheritdoc />
        public bool IsFilter(string tradeObjectName) =>
            !string.IsNullOrEmpty(TargetTradeObjectName)
            && tradeObjectName.Contains(TargetTradeObjectName, StringComparison.InvariantCultureIgnoreCase);

        #endregion
    }
}
