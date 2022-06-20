using Analytic.Filters.FilterGroup.Impl;
using System;
using System.Collections.Generic;

namespace Analytic.Filters.Builders.FilterGroupBuilders
{
    /// <summary>
    ///    Строитель для групп фильтров
    /// </summary>
    public class FilterGroupBuilder
    {
        #region Fields

        private string _targetTradeObjectName;
        private string _filterGroupName;
        private FilterGroupType? _filterGroupType;
        private readonly List<IFilter> _filters = new();

        #endregion

        #region Public methods

        /// <summary>
        ///     Установить название группе фильтров
        /// </summary>
        /// <param name="filterGroupName"> Название группы фильтров </param>
        public FilterGroupBuilder SetFilterGroupName(string filterGroupName)
        {
            _filterGroupName = filterGroupName;

            return this;
        }
        
        /// <summary>
        ///     Добавить фильтр к группе
        /// </summary>
        /// <param name="filter"> Фильтр </param>
        public FilterGroupBuilder AddFilter(IFilter filter)
        {
            _filters.Add(filter);

            return this;
        }

        /// <summary>
        ///     Удалить фильтр из группы
        /// </summary>
        /// <param name="filter"> Фильтр </param>
        public FilterGroupBuilder RemoveFilter(IFilter filter)
        {
            _filters.Remove(filter);

            return this;
        }

        /// <summary>
        ///     Устанавливает тип группы фильтров
        /// </summary>
        /// <param name="filterGroupType"> Тип группы фильтров </param>
        public FilterGroupBuilder SetFilterGroupType(FilterGroupType filterGroupType)
        {
            _filterGroupType = filterGroupType;

            return this;
        }

        /// <summary>
        ///     Установить название объекта торговли 
        /// </summary>
        /// <param name="name"> Название (фильтр будет фильтровать только оюъекты с таким названием) </param>
        /// <remarks>
        ///     <see langword="null"/> - для фильтрации всех
        /// </remarks>
        public FilterGroupBuilder SetTargetTradeObjectName(string name)
        {
            _targetTradeObjectName = name;

            return this;
        }

        /// <summary>
        ///     Сбрасывает настройки группы фильтров
        /// </summary>
        public FilterGroupBuilder Reset()
        {
            _targetTradeObjectName = string.Empty;
            _filterGroupName = string.Empty;
            _filterGroupType = null;
            _filters.Clear();

            return this;
        }

        /// <summary>
        ///     Получить группу фильтров
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> Если не указан тип группы фильтра </exception>
        public IFilterGroup GetResult()
        {
            return _filterGroupType is null
                ? throw new ArgumentNullException($"{nameof(_filterGroupType)} can not be null")
                : (IFilterGroup)new FilterGroup.Impl.FilterGroup(_filterGroupName, _filterGroupType.Value, _targetTradeObjectName, _filters);
        }

        #endregion
    }
}
