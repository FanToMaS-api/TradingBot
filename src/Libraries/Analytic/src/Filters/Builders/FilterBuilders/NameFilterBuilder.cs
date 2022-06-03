using System.Collections.Generic;

namespace Analytic.Filters.Builders.FilterBuilders
{
    /// <summary>
    ///     Строитель <see cref="NameFilter"/>
    /// </summary>
    public class NameFilterBuilder : FilterManagerBuilder
    {
        #region Fields

        private string _filterName;
        private readonly List<string> _tradeObjectNamesToAnalyze = new();

        #endregion

        #region .ctor

        /// <inheritdoc cref="NameFilterBuilder"/>
        public NameFilterBuilder()
            : base() { }

        #endregion

        #region Implementation of FilterManagerBuilder

        /// <inheritdoc />
        public override FilterManagerBuilder AddFilter()
        {
            var nameFilter = new NameFilter(_filterName, _tradeObjectNamesToAnalyze.ToArray());
            _filters.Add(nameFilter);

            return this;
        }

        /// <summary>
        ///     Сбросить настройки фильтра названий
        /// </summary>
        public override FilterManagerBuilder Reset()
        {
            _filterName = string.Empty;
            _tradeObjectNamesToAnalyze.Clear();

            return this;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Устанавливает название фильтра
        /// </summary>
        /// <param name="name"> Название фильтра </param>
        public NameFilterBuilder SetFilterName(string name)
        {
            _filterName = name;
            
            return this;
        }

        /// <summary>
        ///     Добавить название объекта торговли, который сможет пройти фильтр
        /// </summary>
        /// <param name="name"> Название объекта торговли </param>
        public NameFilterBuilder AddTradeObjectName(string name)
        {
            _tradeObjectNamesToAnalyze.Add(name);

            return this;
        }

        #endregion
    }
}
