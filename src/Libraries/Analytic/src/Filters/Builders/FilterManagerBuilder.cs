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
        private readonly List<IFilterGroup> _filterGroups = new();

        #endregion

        #region .ctor

        /// <inheritdoc cref="FilterManagerBuilder"/>
        public FilterManagerBuilder(ILoggerDecorator loggerDecorator)
            => _loggerDecorator = loggerDecorator;

        #endregion

        #region Public methods

        /// <summary>
        ///     Добавляет группу фильтров
        /// </summary>
        public FilterManagerBuilder AddFilterGroup(IFilterGroup group)
        {
            _filterGroups.Add(group);

            return this;
        }

        /// <summary>
        ///     Удаляет группу фильтров
        /// </summary>
        public FilterManagerBuilder RemoveFilterGroup(IFilterGroup group)
        {
            _filterGroups.Remove(group);

            return this;
        }
        
        /// <summary>
        ///     Сбрасывает настройки групп
        /// </summary>
        public FilterManagerBuilder Reset()
        {
            _filterGroups.Clear();
            
            return this;
        }

        /// <summary>
        ///     Получить результат работы строителя
        /// </summary>
        /// <returns></returns>
        public virtual FilterManagerBase GetResult()
            => new DefaultFilterManager(_loggerDecorator, _filterGroups);

        #endregion
    }
}
