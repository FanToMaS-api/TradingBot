using Analytic.Filters.Builders.FilterBuilders;
using System;
using System.Linq;
using Xunit;

namespace AnalyticTests.FilterBuilderTests
{
    /// <summary>
    ///     Тестирует <see cref="NameFilterBuilder"/>
    /// </summary>
    public class NameFilterBuilderTests
    {
        /// <summary>
        ///     Тест установки имени фильтра
        /// </summary>
        [Fact(DisplayName = "Set filter name Test")]
        public void SetFilterName_Test()
        {
            var expectedName = "TestName";
            var expectedTradeObjectName = "TestTradeObjectName";
            var builder = new NameFilterBuilder()
                .SetFilterName(expectedName);

            Assert.Throws<Exception>(() => builder.GetResult());
            
            builder.AddTradeObjectName(expectedTradeObjectName);

            var actualFilter = builder.GetResult();

            Assert.Equal(expectedName, actualFilter.Name);
            Assert.Single(actualFilter.TradeObjectNamesToAnalyze);
            Assert.Equal(expectedTradeObjectName, actualFilter.TradeObjectNamesToAnalyze.Single());
        }

        /// <summary>
        ///     Тест добавления имени объекта торговли
        /// </summary>
        [Fact(DisplayName = "Add trade object name Test")]
        public void AddTradeObjectName_Test()
        {
            var expectedTradeObjectName1 = "TestName";
            var expectedTradeObjectName2 = "TestTradeObjectName";
            var builder = new NameFilterBuilder();

            Assert.Throws<Exception>(() => builder.GetResult());

            // Act
            builder.AddTradeObjectName(expectedTradeObjectName1);

            var actualFilter = builder.GetResult();
            Assert.Single(actualFilter.TradeObjectNamesToAnalyze);
            Assert.Equal(expectedTradeObjectName1, actualFilter.TradeObjectNamesToAnalyze[0]);

            builder.AddTradeObjectName(expectedTradeObjectName2);
            var actualFilter1 = builder.GetResult();

            Assert.Equal(2, actualFilter1.TradeObjectNamesToAnalyze.Length);
            Assert.Equal(expectedTradeObjectName1, actualFilter1.TradeObjectNamesToAnalyze[0]);
            Assert.Equal(expectedTradeObjectName2, actualFilter1.TradeObjectNamesToAnalyze[1]);
        }

        /// <summary>
        ///     Тест добавления имен объектов торговли
        /// </summary>
        [Fact(DisplayName = "Add trade object names Test")]
        public void AddTradeObjectNames_Test()
        {
            var expectedTradeObjectName1 = "TestName";
            var expectedTradeObjectName2 = "TestTradeObjectName";
            var expectedNames = new[] { expectedTradeObjectName1, expectedTradeObjectName2 };
            var builder = new NameFilterBuilder();

            Assert.Throws<Exception>(() => builder.GetResult());

            // Act
            builder.AddTradeObjectNames(expectedNames);

            var actualFilter = builder.GetResult();
            Assert.Equal(2, actualFilter.TradeObjectNamesToAnalyze.Length);
            Assert.Equal(expectedTradeObjectName1, actualFilter.TradeObjectNamesToAnalyze[0]);
            Assert.Equal(expectedTradeObjectName2, actualFilter.TradeObjectNamesToAnalyze[1]);

            Assert.Throws<ArgumentNullException>(() => builder.AddTradeObjectNames(new string[] { null }));
            Assert.Throws<ArgumentNullException>(() => builder.AddTradeObjectNames(new string[] { "" }));
        }

        /// <summary>
        ///     Тест сброса параметров строителя
        /// </summary>
        [Fact(DisplayName = "Reset builder Test")]
        public void ResetBuilder_Test()
        {
            var expectedName = "TestName";
            var expectedTradeObjectName = "TestTradeObjectName";
            var builder = new NameFilterBuilder()
                .SetFilterName(expectedName)
                .AddTradeObjectName(expectedTradeObjectName);

            var actualFilter = builder.GetResult();
            Assert.Equal(expectedName, actualFilter.Name);
            Assert.Single(actualFilter.TradeObjectNamesToAnalyze);
            Assert.Equal(expectedTradeObjectName, actualFilter.TradeObjectNamesToAnalyze[0]);

            // Act
            builder.Reset();
            Assert.Throws<Exception>(() => builder.GetResult());
            builder.AddTradeObjectName("_");

            var actualFilter1 = builder.GetResult();
            Assert.Empty(actualFilter1.Name);
        }
    }
}
