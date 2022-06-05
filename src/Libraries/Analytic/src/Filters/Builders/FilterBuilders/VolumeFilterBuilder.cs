using System;
using System.Linq;

namespace Analytic.Filters.Builders.FilterBuilders
{
    /// <summary>
    ///     Строитель <see cref="VolumeFilter"/>
    /// </summary>
    public class VolumeFilterBuilder : FilterManagerBuilder
    {
        #region Fields

        private static readonly int[] ValidArrayOfOrderNumber = new[] { 5, 10, 20, 50, 100, 500, 1000, 5000 };
        private string _filterName;
        private VolumeType _volumeType;
        private VolumeComparisonType _volumeComparisonType;
        private double _percentDeviation;
        private int _orderNumber;

        #endregion

        #region .ctor

        /// <inheritdoc cref="VolumeFilterBuilder"/>
        public VolumeFilterBuilder()
            : base() { }

        #endregion

        #region Implementation of FilterManagerBuilder

        /// <inheritdoc />
        public override FilterManagerBuilder AddFilter()
        {
            var priceFilter = new VolumeFilter(
                _filterName,
                _volumeType,
                _volumeComparisonType,
                _percentDeviation,
                _orderNumber);
            _filters.Add(priceFilter);

            return this;
        }

        /// <summary>
        ///     Сбросить настройки фильтра названий
        /// </summary>
        public override FilterManagerBuilder Reset()
        {
            _filterName = string.Empty;
            _volumeType = VolumeType.Bid;
            _volumeComparisonType = VolumeComparisonType.GreaterThan;
            _percentDeviation = 0.05; // дефолтное, подобранное значение из фильтра объемов
            _orderNumber = 1000; // данные взяты с учетом ограничений по получению данных с бинанса

            return this;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Устанавливает название фильтра
        /// </summary>
        /// <param name="name"> Название фильтра </param>
        public VolumeFilterBuilder SetFilterName(string name)
        {
            _filterName = name;

            return this;
        }

        /// <summary>
        ///     Установить тип фильтруемых объемов
        /// </summary>
        /// <param name="volumeType"> Тип фильтруемых объемов </param>
        public VolumeFilterBuilder SetVolumeType(VolumeType volumeType = VolumeType.Bid)
        {
            _volumeType = volumeType;

            return this;
        }

        /// <summary>
        ///     Установить тип сравнения объемов
        /// </summary>
        /// <param name="volumeComparisonType"> Тип сравнения объемов </param>
        public VolumeFilterBuilder SetVolumeComparisonType(VolumeComparisonType volumeComparisonType = VolumeComparisonType.GreaterThan)
        {
            _volumeComparisonType = volumeComparisonType;

            return this;
        }

        /// <summary>
        ///     Установить процентное отклонение одного типа объема продаж (указанного) от другого
        /// </summary>
        /// <param name="percentDeviation"> Процентное отклонение </param>
        public VolumeFilterBuilder SetPercentDeviation(double percentDeviation = 0.05)
        {
            _percentDeviation = percentDeviation;

            return this;
        }

        /// <summary>
        ///     Установить Необходимое кол-во ордеров для расчетов
        /// </summary>
        /// <param name="orderNumber"> 
        ///     Необходимое кол-во ордеров <br />
        ///     Возможные значения: 5, 10, 20, 50, 100, 500, 1000, 5000
        /// </param>
        public VolumeFilterBuilder SetOrderNumber(int orderNumber = 1000)
        {
            if (ValidArrayOfOrderNumber.Contains(orderNumber))
            {
                throw new ArgumentException($"Invalid number of orders {nameof(orderNumber)}");
            }

            _orderNumber = orderNumber;

            return this;
        }

        #endregion
    }
}
