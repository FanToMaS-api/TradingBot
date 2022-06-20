using Analytic.Filters;
using Analytic.Models;
using Analytic.Filters.Enums;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnalyticTests.FilterTests
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
        public async Task LastPriceGreaterThanLimit_Test()
        {
            var filter = new PriceFilter(
                "LastPriceGreaterThan",
                ComparisonType.GreaterThan,
                10);

            Assert.True(await filter.CheckConditionsAsync(null, _infoModel_15, CancellationToken.None));
            Assert.False(await filter.CheckConditionsAsync(null, _infoModel_10, CancellationToken.None));
        }

        /// <summary>
        ///     Тест фильтрации: последняя цена меньше указанного значения
        /// </summary>
        [Fact(DisplayName = "Last price greater than limit Test")]
        public async Task LastPriceLessThanLimit_Test()
        {
            var filter = new PriceFilter(
                "LastPriceLessThan",
                ComparisonType.LessThan,
                11);

            Assert.False(await filter.CheckConditionsAsync(null, _infoModel_15, CancellationToken.None));
            Assert.True(await filter.CheckConditionsAsync(null, _infoModel_10, CancellationToken.None));
        }

        /// <summary>
        ///     Тест фильтрации: последняя цена равна указанному значению
        /// </summary>
        [Fact(DisplayName = "Last price equal limit Test")]
        public async Task LastPriceEqualLimit_Test()
        {
            var filter = new PriceFilter(
                "LastPriceEqual",
                ComparisonType.Equal,
                10);

            Assert.False(await filter.CheckConditionsAsync(null, _infoModel_15, CancellationToken.None));
            Assert.True(await filter.CheckConditionsAsync(null, _infoModel_10, CancellationToken.None));
        }
    }
}
