using Analytic.Filters.Enums;

namespace Analytic.Filters.Builders.FilterBuilders
{
    /// <summary>
    ///     Строитель <see cref="PriceDeviationFilter"/>
    /// </summary>
    public class PriceDeviationFilterBuilder : FilterManagerBuilder
    {
        #region Fields

        private string _filterName;
        private ComparisonType _comparisonType;
        private AggregateDataIntervalType _aggregateDataIntervalType;
        private double _limit;
        private int _timeframeNumber;

        #endregion

        #region .ctor

        /// <inheritdoc cref="PriceDeviationFilterBuilder"/>
        public PriceDeviationFilterBuilder()
            : base() { }

        #endregion

        #region Implementation of FilterManagerBuilder

        /// <inheritdoc />
        public override FilterManagerBuilder AddFilter()
        {
            var priceFilter = new PriceDeviationFilter(
                _filterName,
                _aggregateDataIntervalType,
                _comparisonType,
                _limit,
                _timeframeNumber);
            _filters.Add(priceFilter);

            return this;
        }

        /// <summary>
        ///     Сбросить настройки фильтра названий
        /// </summary>
        public override FilterManagerBuilder Reset()
        {
            _filterName = string.Empty;
            _aggregateDataIntervalType = AggregateDataIntervalType.Default;
            _comparisonType = ComparisonType.GreaterThan;
            _limit = 0;
            _timeframeNumber = 5; // базовое значение, взято из конструктора фильтра отклонения цен

            return this;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Устанавливает название фильтра
        /// </summary>
        /// <param name="name"> Название фильтра </param>
        public PriceDeviationFilterBuilder SetFilterName(string name)
        {
            _filterName = name;

            return this;
        }

        /// <summary>
        ///     Установить тип сравнения для лимита
        /// </summary>
        /// <param name="comparisonType"> Тип сравнения </param>
        public PriceDeviationFilterBuilder SetComparisonType(ComparisonType comparisonType = ComparisonType.GreaterThan)
        {
            _comparisonType = comparisonType;

            return this;
        }

        /// <summary>
        ///     Установить тип интервала агрегирования данных
        /// </summary>
        /// <param name="aggregateDataIntervalType"> Тип интервала агрегирования данных </param>
        public PriceDeviationFilterBuilder SetAggregateDataIntervalType(
            AggregateDataIntervalType aggregateDataIntervalType = AggregateDataIntervalType.Default)
        {
            _aggregateDataIntervalType = aggregateDataIntervalType;

            return this;
        }

        /// <summary>
        ///     Установить ограничение на отклонение цены
        /// </summary>
        /// <param name="limit"> Ограничение </param>
        public PriceDeviationFilterBuilder SetLimit(double limit = 0)
        {
            _limit = limit;

            return this;
        }

        /// <summary>
        ///     Установить кол-во таймфреймов, участвующих в анализе
        /// </summary>
        /// <param name="timeframeNumber"> Кол-во таймфреймов, участвующих в анализе </param>
        public PriceDeviationFilterBuilder SetTimeframeNumber(int timeframeNumber = 5)
        {
            _timeframeNumber = timeframeNumber;

            return this;
        }

        #endregion
    }
}
