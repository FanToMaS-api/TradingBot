using Analytic.Filters.Builders.FilterBuilders;
using Analytic.Filters.Enums;
using System;
using Xunit;

namespace AnalyticTests.FilterBuilderTests
{
    /// <summary>
    ///     Тестирует класс <see cref="PriceFilterBuilder"/>
    /// </summary>
    public class PriceFilterBuilderTests
    {
        /// <summary>
        ///     Тест установки имени фильтра
        /// </summary>
        [Fact(DisplayName = "Set filter name Test")]
        public void SetFilterName_Test()
        {
            var expectedName = "TestName";
            var builder = new PriceFilterBuilder()
                .SetFilterName(expectedName);
            
            Assert.Throws<Exception>(() => builder.GetResult());

            var actualFilter = builder.SetLimit(10).GetResult();

            Assert.Equal(expectedName, actualFilter.Name);
        }

        /// <summary>
        ///     Тест установки типа сравнения данных
        /// </summary>
        [Fact(DisplayName = "Set comparison type Test")]
        public void SetComparisonType_Test()
        {
            var expectedValue = ComparisonType.LessThan;
            var builder = new PriceFilterBuilder()
                .SetComparisonType(expectedValue);

            Assert.Throws<Exception>(() => builder.GetResult());

            var actualFilter = builder.SetLimit(10).GetResult();

            Assert.Equal(expectedValue, actualFilter.ComparisonType);
        }

        /// <summary>
        ///     Тест установки лимита отклонения цены
        /// </summary>
        [Fact(DisplayName = "Set limit Test")]
        public void SetLimit_Test()
        {
            var expectedValue = 20;
            var builder = new PriceFilterBuilder()
                .SetLimit(expectedValue);

            Assert.Throws<ArgumentException>(() => builder.SetLimit(0));
            Assert.Throws<ArgumentException>(() => builder.SetLimit(-1));
            var actualFilter = builder.GetResult();

            Assert.Equal(expectedValue, actualFilter.Limit);
        }
        
        /// <summary>
        ///     Тест сброса параметров строителя
        /// </summary>
        [Fact(DisplayName = "Reset builder Test")]
        public void ResetBuilder_Test()
        {
            var expectedName = "TestName";
            var expectedLimit = 5;
            var expectedComparisonType = ComparisonType.Equal;
            var builder = new PriceFilterBuilder()
                .SetFilterName(expectedName)
                .SetComparisonType(expectedComparisonType)
                .SetLimit(expectedLimit);
            
            var actualFilter = builder.GetResult();
            Assert.Equal(expectedName, actualFilter.Name);
            Assert.Equal(expectedLimit, actualFilter.Limit);
            Assert.Equal(expectedComparisonType, actualFilter.ComparisonType);

            // Act
            builder.Reset();

            Assert.Throws<Exception>(() => builder.GetResult());
            var actualFilter1 = builder.SetLimit(10).GetResult();
            
            Assert.Empty(actualFilter1.Name);
            Assert.Equal(10, actualFilter1.Limit);
            Assert.Equal(ComparisonType.GreaterThan, actualFilter1.ComparisonType);
        }
    }
}
