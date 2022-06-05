﻿using Analytic.Filters.Builders.FilterBuilders;
using Analytic.Filters.FilterGroup.Impl;

namespace Analytic.Filters.Builders.FilterGroupBuilders
{
    /// <summary>
    ///    Строитель для групп фильтров первостепенного назначения
    /// </summary>
    public class PrimaryFilterGroupBuilder : FilterManagerBuilder
    {
        #region Fields

        private string _targetTradeObjectName;
        private string _filterGroupName;

        #endregion

        #region .ctor

        /// <inheritdoc cref="PrimaryFilterGroupBuilder"/>
        public PrimaryFilterGroupBuilder()
            : base()
        { }

        #endregion

        #region Implementation of FilterManagerBuilder

        /// <inheritdoc />
        public override FilterManagerBuilder AddFilterGroup()
        {
            var filterGroup = new FilterGroup.Impl.FilterGroup(
                _filterGroupName,
                FilterGroupType.Primary,
                _targetTradeObjectName,
                _filters);

            _filterGroups.Add(filterGroup);

            return this;
        }

        /// <inheritdoc />
        public override FilterManagerBuilder Reset()
        {
            _targetTradeObjectName = string.Empty;
            _filterGroupName = string.Empty;
            _filters.Clear();

            return this;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Строитель фильтра наименования объекта торговли
        /// </summary>
        public NameFilterBuilder NameFilterBuilder =>
            new();

        /// <summary>
        ///     Строитель фильтра цен
        /// </summary>
        public PriceFilterBuilder PriceFilterBuilder =>
            new();

        /// <summary>
        ///     Строитель фильтра отклонения цен
        /// </summary>
        public PriceDeviationFilterBuilder PriceDeviationFilterBuilder =>
            new();

        /// <summary>
        ///     Строитель фильтра объемов
        /// </summary>
        public VolumeFilterBuilder VolumeFilterBuilder =>
            new();

        #endregion

        #region Public methods

        /// <summary>
        ///     Установить название объекта торговли 
        /// </summary>
        /// <param name="name"> Название (фильтр будет фильтровать только объекты с таким названием) </param>
        /// <remarks>
        ///     <see langword="null"/> - для фильтрации всех
        /// </remarks>
        public PrimaryFilterGroupBuilder SetTargetTradeObjectName(string name = null)
        {
            _targetTradeObjectName = name;

            return this;
        }

        /// <summary>
        ///     Установить название группе фильтров
        /// </summary>
        /// <param name="filterGroupName"> Название группы фильтров </param>
        public PrimaryFilterGroupBuilder SetFilterGroupName(string filterGroupName)
        {
            _filterGroupName = filterGroupName;

            return this;
        }

        #endregion
    }
}
