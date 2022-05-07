using Analytic.Filters;
using Analytic.Models;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует класс <see cref="PriceFilter"/>
    /// </summary>
    public class PriceFilterTests
    {
        #region Fields

        private readonly InfoModel _infoModel_15 = new("15", 15);
        private readonly InfoModel _infoModel_10 = new("10", 10);

        #endregion

        /// <summary>
        ///     Тест фильтрации: последняя цена больше указанного значения
        /// </summary>
        [Fact(DisplayName = "Last price greater than limit Test")]
        public void LastPriceGreaterThanLimit_Test()
        {
            var filter = new PriceFilter(
                "LastPriceGreaterThan",
                ComparisonType.GreaterThan,
                10);

            Assert.True(filter.CheckConditions(_infoModel_15));
            Assert.False(filter.CheckConditions(_infoModel_10));
        }

        /// <summary>
        ///     Тест фильтрации: последняя цена меньше указанного значения
        /// </summary>
        [Fact(DisplayName = "Last price greater than limit Test")]
        public void LastPriceLessThanLimit_Test()
        {
            var filter = new PriceFilter(
                "LastPriceLessThan",
                ComparisonType.LessThan,
                11);

            Assert.False(filter.CheckConditions(_infoModel_15));
            Assert.True(filter.CheckConditions(_infoModel_10));
        }

        /// <summary>
        ///     Тест фильтрации: последняя цена равна указанному значению
        /// </summary>
        [Fact(DisplayName = "Last price equal limit Test")]
        public void LastPriceEqualLimit_Test()
        {
            var filter = new PriceFilter(
                "LastPriceEqual",
                ComparisonType.Equal,
                10);

            Assert.False(filter.CheckConditions(_infoModel_15));
            Assert.True(filter.CheckConditions(_infoModel_10));
        }
    }
}
