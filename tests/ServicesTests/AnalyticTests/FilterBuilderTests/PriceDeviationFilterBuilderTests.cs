using Analytic.Filters.Builders.FilterBuilders;
using Analytic.Filters.Enums;
using System;
using Xunit;

namespace AnalyticTests.FilterBuilderTests
{
    /// <summary>
    ///     Тестирует класс <see cref="PriceDeviationFilterBuilder"/>
    /// </summary>
    public class PriceDeviationFilterBuilderTests
    {
        /// <summary>
        ///     Тест установки имени фильтра
        /// </summary>
        [Fact(DisplayName = "Set filter name Test")]
        public void SetFilterName_Test()
        {
            var expectedName = "TestName";
            var builder = new PriceDeviationFilterBuilder()
                .SetFilterName(expectedName);

            var actualFilter = builder.GetResult();

            Assert.Equal(expectedName, actualFilter.Name);
        }

        /// <summary>
        ///     Тест установки кол-ва таймфреймов для анализа
        /// </summary>
        [Fact(DisplayName = "Set timeframe number Test")]
        public void SetTimeframeNumber_Test()
        {
            var expectedTimeframeNumber = 20;
            var builder = new PriceDeviationFilterBuilder()
                .SetTimeframeNumber(expectedTimeframeNumber);

            var actualFilter = builder.GetResult();

            Assert.Equal(expectedTimeframeNumber, actualFilter.TimeframeNumber);
        }

        /// <summary>
        ///     Тест установки лимита отклонения цены
        /// </summary>
        [Fact(DisplayName = "Set limit Test")]
        public void SetLimit_Test()
        {
            var expectedValue = 20;
            var builder = new PriceDeviationFilterBuilder()
                .SetLimit(expectedValue);

            var actualFilter = builder.GetResult();

            Assert.Equal(expectedValue, actualFilter.Limit);
        }
        
        /// <summary>
        ///     Тест установки интервала агрегирования данных
        /// </summary>
        [Fact(DisplayName = "Set aggregate data interval type Test")]
        public void SetAggregateDataIntervalType_Test()
        {
            var expectedValue = AggregateDataIntervalType.FiveMinutes;
            var builder = new PriceDeviationFilterBuilder()
                .SetAggregateDataIntervalType(expectedValue);

            var actualFilter = builder.GetResult();

            Assert.Equal(expectedValue, actualFilter.Interval);
        }
        
        /// <summary>
        ///     Тест установки типа сравнения данных
        /// </summary>
        [Fact(DisplayName = "Set comparison type Test")]
        public void SetComparisonType_Test()
        {
            var expectedValue = ComparisonType.LessThan;
            var builder = new PriceDeviationFilterBuilder()
                .SetComparisonType(expectedValue);

            var actualFilter = builder.GetResult();

            Assert.Equal(expectedValue, actualFilter.ComparisonType);
        }

        /// <summary>
        ///     Тест сброса параметров строителя
        /// </summary>
        [Fact(DisplayName = "Reset builder Test")]
        public void ResetBuilder_Test()
        {
            var expectedName = "TestName";
            var expectedTimeframeNumber = 20;
            var expectedLimit = 5;
            var expectedComparisonType= ComparisonType.Equal;
            var expectedAggregateDataIntervalType = AggregateDataIntervalType.OneHour;
            var builder = new PriceDeviationFilterBuilder()
                .SetFilterName(expectedName)
                .SetComparisonType(expectedComparisonType)
                .SetLimit(expectedLimit)
                .SetAggregateDataIntervalType(expectedAggregateDataIntervalType)
                .SetTimeframeNumber(expectedTimeframeNumber);

            var actualFilter = builder.GetResult();
            Assert.Equal(expectedName, actualFilter.Name);
            Assert.Equal(expectedTimeframeNumber, actualFilter.TimeframeNumber);
            Assert.Equal(expectedLimit, actualFilter.Limit);
            Assert.Equal(expectedComparisonType, actualFilter.ComparisonType);
            Assert.Equal(expectedAggregateDataIntervalType, actualFilter.Interval);

            // Act
            builder.Reset();

            var actualFilter1 = builder.GetResult();
            Assert.Empty(actualFilter1.Name);
            Assert.Equal(5, actualFilter1.TimeframeNumber);
            Assert.Equal(0, actualFilter1.Limit);
            Assert.Equal(ComparisonType.GreaterThan, actualFilter1.ComparisonType);
            Assert.Equal(AggregateDataIntervalType.Default, actualFilter1.Interval);
        }
    }
}
