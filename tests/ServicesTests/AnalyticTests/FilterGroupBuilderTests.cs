using Analytic.Filters;
using Analytic.Filters.Builders.FilterGroupBuilders;
using Analytic.Filters.FilterGroup.Impl;
using SharedForTest;
using System;
using System.Linq;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует класс <see cref="FilterGroupBuilder"/>
    /// </summary>
    public class FilterGroupBuilderTests
    {
        /// <summary>
        ///     Тест установки имени группы фильтров
        /// </summary>
        [Fact(DisplayName = "Set filter group name Test")]
        public void SetFilterName_Test()
        {
            var expectedName = "TestName";
            var builder = new FilterGroupBuilder()
                .SetFilterGroupName(expectedName);

            Assert.Throws<ArgumentNullException>(() => builder.GetResult());

            builder.SetFilterGroupType(FilterGroupType.Primary);

            var actualFilterGroup = builder.GetResult();

            Assert.Equal(expectedName, actualFilterGroup.Name);
            Assert.Equal(FilterGroupType.Primary, actualFilterGroup.Type);
        }

        /// <summary>
        ///     Тест добавления и удаления фильтра из группы фильтров
        /// </summary>
        [Fact(DisplayName = "Set add and remove filter Test")]
        public void AddAndRemoveFilter_Test()
        {
            var expectedName = "TestName";
            var expectedFilter = new VolumeFilter(expectedName);
            var builder = new FilterGroupBuilder()
                .SetFilterGroupName(expectedName)
                .AddFilter(expectedFilter);

            Assert.Throws<ArgumentNullException>(() => builder.GetResult());

            builder.SetFilterGroupType(FilterGroupType.Primary);

            var actualFilterGroup = builder.GetResult();

            Assert.Equal(expectedName, actualFilterGroup.Name);
            Assert.Equal(FilterGroupType.Primary, actualFilterGroup.Type);
            Assert.Single(actualFilterGroup.Filters);
            TestExtensions.CheckingAssertions(expectedFilter, actualFilterGroup.Filters.First() as VolumeFilter);

            builder.RemoveFilter(expectedFilter);

            var actualFilterGroup1 = builder.GetResult();

            Assert.Equal(expectedName, actualFilterGroup1.Name);
            Assert.Equal(FilterGroupType.Primary, actualFilterGroup1.Type);
            Assert.Empty(actualFilterGroup1.Filters);
        }

        /// <summary>
        ///     Тест установки типа группы фильтров
        /// </summary>
        [Fact(DisplayName = "Set filter group type Test")]
        public void SetFilterGroupType_Test()
        {
            var expectedType = FilterGroupType.SpecialLatest;
            var builder = new FilterGroupBuilder()
                .SetFilterGroupType(expectedType);

            var actualFilterGroup = builder.GetResult();

            Assert.Equal(expectedType, actualFilterGroup.Type);
        }

        /// <summary>
        ///     Тест установки названия объекта торговли
        /// </summary>
        [Fact(DisplayName = "Set target trade object name Test")]
        public void SetTargetTradeObjectName_Test()
        {
            var expectedType = FilterGroupType.SpecialLatest;
            var expectedName = "TestName";
            var builder = new FilterGroupBuilder()
                .SetTargetTradeObjectName(expectedName);

            Assert.Throws<ArgumentNullException>(() => builder.GetResult());

            builder.SetFilterGroupType(expectedType);
            var actualFilterGroup = builder.GetResult();

            Assert.Equal(expectedName, actualFilterGroup.TargetTradeObjectName);
            Assert.Equal(expectedType, actualFilterGroup.Type);
        }

        /// <summary>
        ///     Тест установки названия объекта торговли
        /// </summary>
        [Fact(DisplayName = "Set target trade object name Test")]
        public void Reset_Test()
        {
            var expectedType = FilterGroupType.SpecialLatest;
            var expectedName = "TestName";
            var expectedTargetTradeObjectName = "TestTargetTradeObjectName";
            var expectedFilter = new VolumeFilter(expectedName);
            var builder = new FilterGroupBuilder()
                .SetFilterGroupType(expectedType)
                .SetFilterGroupName(expectedName)
                .AddFilter(expectedFilter)
                .AddFilter(expectedFilter)
                .SetTargetTradeObjectName(expectedTargetTradeObjectName);

            var actualFilterGroup = builder.GetResult();

            Assert.Equal(expectedName, actualFilterGroup.Name);
            Assert.Equal(expectedTargetTradeObjectName, actualFilterGroup.TargetTradeObjectName);
            Assert.Equal(expectedType, actualFilterGroup.Type);
            Assert.Equal(2, actualFilterGroup.Filters.Count);
            TestExtensions.CheckingAssertions(expectedFilter, actualFilterGroup.Filters.First() as VolumeFilter);

            // Act
            builder.Reset();

            Assert.Throws<ArgumentNullException>(() => builder.GetResult());
            var actualFilterGroup1 = builder.SetFilterGroupType(expectedType).GetResult();

            Assert.Equal(expectedType, actualFilterGroup.Type);
            Assert.Empty(actualFilterGroup1.Name);
            Assert.Empty(actualFilterGroup1.TargetTradeObjectName);
            Assert.Empty(actualFilterGroup1.Filters);
        }
    }
}
