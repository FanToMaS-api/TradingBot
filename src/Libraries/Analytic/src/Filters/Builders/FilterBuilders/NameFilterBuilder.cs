using System;
using System.Collections.Generic;
using System.Linq;

namespace Analytic.Filters.Builders.FilterBuilders
{
    /// <summary>
    ///     Строитель <see cref="NameFilter"/>
    /// </summary>
    public class NameFilterBuilder
    {
        #region Fields

        private string _filterName;
        private readonly List<string> _tradeObjectNamesToAnalyze = new();

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

        /// <summary>
        ///     Добавить названия объектов торговли, которые смогет пройти фильтр
        /// </summary>
        /// <param name="names"> Названия объектов торговли </param>
        public NameFilterBuilder AddTradeObjectNames(IEnumerable<string> names)
        {
            _tradeObjectNamesToAnalyze.AddRange(names);

            return this;
        }

        /// <summary>
        ///     Добавить названия объектов торговли, которые смогет пройти фильтр
        /// </summary>
        /// <param name="names"> Названия объектов торговли </param>
        public NameFilterBuilder AddTradeObjectNames(string[] names)
        {
            _tradeObjectNamesToAnalyze.AddRange(names);

            return this;
        }
        
        /// <summary>
        ///     Возвращает результат работы билдера - фильтр названий торговых объектов
        /// </summary>
        /// <exception cref="Exception"> Если не указано ни одного названия торговых объектов </exception>
        public NameFilter GetResult()
        {
            return !_tradeObjectNamesToAnalyze.Any() ?
                throw new Exception($"Trade object names {nameof(_tradeObjectNamesToAnalyze)} to analyze cannot be empty.")
                : new NameFilter(_filterName, _tradeObjectNamesToAnalyze.ToArray());
        }

        /// <summary>
        ///     Сбросить настройки фильтра названий
        /// </summary>
        public NameFilterBuilder Reset()
        {
            _filterName = string.Empty;
            _tradeObjectNamesToAnalyze.Clear();

            return this;
        }

        #endregion
    }
}
