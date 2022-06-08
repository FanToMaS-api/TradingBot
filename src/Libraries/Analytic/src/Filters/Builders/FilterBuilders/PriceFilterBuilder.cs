using Analytic.Filters.Enums;
using System;

namespace Analytic.Filters.Builders.FilterBuilders
{
    /// <summary>
    ///     Строитель <see cref="PriceFilter"/>
    /// </summary>
    public class PriceFilterBuilder
    {
        #region Fields

        private string _filterName;
        private ComparisonType _comparisonType;
        private double _limit;

        #endregion

        #region Public methods

        /// <summary>
        ///     Устанавливает название фильтра
        /// </summary>
        /// <param name="name"> Название фильтра </param>
        public PriceFilterBuilder SetFilterName(string name)
        {
            _filterName = name;

            return this;
        }

        /// <summary>
        ///     Установить тип сравнения для лимита
        /// </summary>
        /// <param name="comparisonType"> Тип сравнения </param>
        public PriceFilterBuilder SetComparisonType(ComparisonType comparisonType = ComparisonType.GreaterThan)
        {
            _comparisonType = comparisonType;

            return this;
        }
        
        /// <summary>
        ///     Установить ценовой лимит
        /// </summary>
        /// <param name="limit"> Лимит </param>
        public PriceFilterBuilder SetLimit(double limit)
        {
            _limit = limit;

            return this;
        }

        /// <summary>
        ///     Создает фильтр цен
        /// </summary>
        /// <exception cref="Exception"> Если значение лимита <= 0 </exception>
        public PriceFilter GetResult()
        {
            return _limit <= 0 ?
                throw new Exception($"Price limit {nameof(_limit)} cannot be 0.")
                : new PriceFilter(_filterName, _comparisonType, _limit);
        }

        /// <summary>
        ///     Сбросить настройки фильтра цен
        /// </summary>
        public PriceFilterBuilder Reset()
        {
            _filterName = string.Empty;
            _comparisonType = ComparisonType.GreaterThan;
            _limit = 0;

            return this;
        }

        #endregion
    }
}
