using Analytic.Filters;
using Analytic.Filters.Builders.FilterBuilders;
using System;
using Xunit;

namespace AnalyticTests.FilterBuilderTests
{
    /// <summary>
    ///     Тестирует класс <see cref="VolumeFilterBuilder"/>
    /// </summary>
    public class VolumeFilterBuilderTests
    {
        /// <summary>
        ///     Тест установки имени фильтра
        /// </summary>
        [Fact(DisplayName = "Set filter name Test")]
        public void SetFilterName_Test()
        {
            var expectedName = "TestName";
            var builder = new VolumeFilterBuilder()
                .SetFilterName(expectedName);

            var actualFilter = builder.GetResult();

            Assert.Equal(expectedName, actualFilter.Name);
        }

        /// <summary>
        ///     Тест установки типа сравнения данных
        /// </summary>
        [Fact(DisplayName = "Set comparison type Test")]
        public void SetComparisonType_Test()
        {
            var expectedValue = VolumeComparisonType.LessThan;
            var builder = new VolumeFilterBuilder()
                .SetComparisonType(expectedValue);

            var actualFilter = builder.GetResult();

            Assert.Equal(expectedValue, actualFilter.ComparisonType);
        }

        /// <summary>
        ///     Тест установки необходимого кол-ва ордеров для расчетов
        /// </summary>
        [Fact(DisplayName = "Set order number Test")]
        public void SetOrderNumber_Test()
        {
            var expectedValue = 5000;
            var builder = new VolumeFilterBuilder()
                .SetOrderNumber(expectedValue);

            Assert.Throws<ArgumentException>(() => builder.SetOrderNumber(0));
            Assert.Throws<ArgumentException>(() => builder.SetOrderNumber(-1));
            Assert.Throws<ArgumentException>(() => builder.SetOrderNumber(21));
            Assert.Throws<ArgumentException>(() => builder.SetOrderNumber(40));
            var actualFilter = builder.GetResult();

            Assert.Equal(expectedValue, actualFilter.OrderNumber);
        }

        /// <summary>
        ///     Тест установки процентного отклонения одного типа объема продаж (указанного) от другого
        /// </summary>
        [Fact(DisplayName = "Set percent deviation Test")]
        public void SetPercentDeviation_Test()
        {
            var expectedValue = -1.15;
            var builder = new VolumeFilterBuilder()
                .SetPercentDeviation(expectedValue);
            
            var actualFilter = builder.GetResult();

            Assert.Equal(expectedValue, actualFilter.PercentDeviation);
        }

        /// <summary>
        ///     Тест установки типа фильтруемых объемов
        /// </summary>
        [Fact(DisplayName = "Set volume type Test")]
        public void SetVolumeType_Test()
        {
            var expectedValue = VolumeType.Ask;
            var builder = new VolumeFilterBuilder()
                .SetVolumeType(expectedValue);
            
            var actualFilter = builder.GetResult();

            Assert.Equal(expectedValue, actualFilter.VolumeType);
        }

        /// <summary>
        ///     Тест сброса параметров строителя
        /// </summary>
        [Fact(DisplayName = "Reset builder Test")]
        public void ResetBuilder_Test()
        {
            var expectedName = "TestName";
            var expectedPercentDeviation = 5.5;
            var expectedOrderNumber = 100;
            var expectedComparisonType = VolumeComparisonType.LessThan;
            var expectedVolumeType= VolumeType.Ask;
            var builder = new VolumeFilterBuilder()
                .SetFilterName(expectedName)
                .SetComparisonType(expectedComparisonType)
                .SetVolumeType(expectedVolumeType)
                .SetPercentDeviation(expectedPercentDeviation)
                .SetOrderNumber(expectedOrderNumber);

            var actualFilter = builder.GetResult();
            Assert.Equal(expectedName, actualFilter.Name);
            Assert.Equal(expectedPercentDeviation, actualFilter.PercentDeviation);
            Assert.Equal(expectedOrderNumber, actualFilter.OrderNumber);
            Assert.Equal(expectedComparisonType, actualFilter.ComparisonType);
            Assert.Equal(expectedVolumeType, actualFilter.VolumeType);

            // Act
            builder.Reset();

            var actualFilter1 = builder.GetResult();

            Assert.Empty(actualFilter1.Name);
            Assert.Equal(0.05, actualFilter1.PercentDeviation);
            Assert.Equal(1000, actualFilter1.OrderNumber);
            Assert.Equal(VolumeComparisonType.GreaterThan, actualFilter1.ComparisonType);
            Assert.Equal(VolumeType.Bid, actualFilter1.VolumeType);
        }
    }
}
