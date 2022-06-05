using Analytic.Filters.Builders.FilterGroupBuilders;
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

        private CommonFilterGroupBuilder _commonFilterGroupBuilder;
        private CommonLatestFilterGroupBuilder _commonLatestFilterGroupBuilder;
        private PrimaryFilterGroupBuilder _primaryFilterGroupBuilder;
        private SpecialFilterGroupBuilder _specialFilterGroupBuilder;
        private SpecialLatestFilterGroupBuilder _specialLatestFilterGroup;
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
        ///     Позволяет добавлять фильтры в группы общего назначения
        /// </summary>
        public CommonFilterGroupBuilder CommonFilterGroup
            => _commonFilterGroupBuilder ??= new();

        /// <summary>
        ///     Позволяет добавлять фильтры в группы общего назначения, которые будут вызываться в последнюю очередь
        /// </summary>
        public CommonLatestFilterGroupBuilder CommonLatestFilterGroup
            => _commonLatestFilterGroupBuilder ??= new();

        /// <summary>
        ///     Позволяет добавлять фильтры в группы первостепенной важности (при фильтрации вызываются самые первые)
        /// </summary>
        public PrimaryFilterGroupBuilder PrimaryFilterGroup
            => _primaryFilterGroupBuilder ??= new();

        /// <summary>
        ///     Позволяет добавлять фильтры в группы специального назначения (фильтруют указанные объекты)
        /// </summary>
        public SpecialFilterGroupBuilder SpecialFilterGroup
            => _specialFilterGroupBuilder ??= new();

        /// <summary>
        ///     Позволяет добавлять фильтры в группы специального назначения <br/>
        ///     При фильтрации будут вызываться самые последние
        /// </summary>
        public SpecialLatestFilterGroupBuilder SpecialLatestFilterGroup
            => _specialLatestFilterGroup ??= new();

        #endregion

        #region Public methods

        /// <summary>
        ///     Сбрасывает настройки
        /// </summary>
        public virtual FilterManagerBuilder Reset()
        {
            _commonFilterGroupBuilder = new();
            _commonLatestFilterGroupBuilder = new();
            _primaryFilterGroupBuilder = new();
            _specialFilterGroupBuilder = new();
            _specialLatestFilterGroup = new();

            return this;
        }

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
