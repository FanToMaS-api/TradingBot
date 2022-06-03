using Analytic.Filters.Impl.FilterManagers;
using Logger;
using System.Collections.Generic;

namespace Analytic.Filters.Builders
{
    /// <summary>
    ///     Строитель <see cref="FilterManagerBase"/>
    /// </summary>
    public class FilterManagerBuilder
    {
        #region Fields

        private readonly ILoggerDecorator _loggerDecorator;
        protected readonly List<IFilterGroup> _filterGroups;
        protected readonly List<IFilter> _filters;

        #endregion

        #region .ctor

        /// <inheritdoc cref="FilterManagerBuilder"/>
        public FilterManagerBuilder(ILoggerDecorator loggerDecorator)
            => _loggerDecorator = loggerDecorator;

        /// <inheritdoc cref="FilterManagerBuilder"/>
        protected FilterManagerBuilder() => _filters = new();

        #endregion

        #region Properties

        /// <summary>
        ///     Позволяет добавлять фильтры общего назначения
        /// </summary>
        public CommonFilterGroupBuilder CommonFilterGroup
            => new();

        #endregion

        #region Public methods

        /// <summary>
        ///     Сбрасывает настройки
        /// </summary>
        public virtual FilterManagerBuilder Reset() => this;

        /// <summary>
        ///     Добавляет группу фильтров к менеджеру
        /// </summary>
        public virtual FilterManagerBuilder AddFilterGroup() => this;

        /// <summary>
        ///     Добавляет фильтр к группе
        /// </summary>
        public virtual FilterManagerBuilder AddFilter() => this;

        /// <summary>
        ///     Получить результат работы строителя
        /// </summary>
        /// <returns></returns>
        public virtual FilterManagerBase GetResult()
            => new DefaultFilterManager(_loggerDecorator, _filterGroups);

        #endregion
    }
}
