using Analytic.Filters.Impl.FilterManagers;
using Logger;

namespace Analytic.Filters.Builders
{
    /// <summary>
    ///     Строитель <see cref="FilterManagerBase"/>
    /// </summary>
    public class FilterManagerBuilder
    {
        #region Fields

        protected readonly FilterManagerBase _filterManager;

        #endregion

        #region .ctor

        /// <inheritdoc cref="FilterManagerBuilder"/>
        public FilterManagerBuilder(ILoggerDecorator loggerDecorator)
            => _filterManager = new DefaultFilterManager(loggerDecorator);

        /// <inheritdoc cref="FilterManagerBuilder"/>
        protected FilterManagerBuilder(FilterManagerBase filterManager)
            => _filterManager = filterManager;

        #endregion

        #region Properties

        /// <summary>
        ///     Позволяет добавлять фильтры общего назначения
        /// </summary>
        public CommonFilterGroupBuilder CommonFilterGroup
            => new(_filterManager);

        #endregion

        #region Public methods

        /// <summary>
        ///     Позволяет кастить объект
        /// </summary>
        public static implicit operator FilterManagerBase(FilterManagerBuilder builder)
            => builder._filterManager;

        #endregion
    }
}
