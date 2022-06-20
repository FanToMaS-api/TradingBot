using Analytic.Filters.Builders;
using Analytic.Filters.Builders.FilterGroupBuilders;
using Analytic.Filters.FilterGroup.Impl;
using Logger;
using NSubstitute;
using SharedForTest;
using System.Linq;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует класс <see cref="FilterManagerBuilder"/>
    /// </summary>
    public class FilterManagerBuilderTests
    {
        /// <summary>
        ///     Тест добавления группы фильтров
        /// </summary>
        [Fact(DisplayName = "Add and remove filter group Test")]
        public void AddAndRemoveFilterGroup_Test()
        {
            var logger = Substitute.For<ILoggerDecorator>();
            var builder = new FilterManagerBuilder(logger);
            var expectedFilterGroup = new FilterGroup();

            builder.AddFilterGroup(expectedFilterGroup);

            var actualFilterManager = builder.GetResult();

            Assert.Single(actualFilterManager.FilterGroups);
            TestExtensions.CheckingAssertions(expectedFilterGroup, actualFilterManager.FilterGroups.First() as FilterGroup);

            builder.RemoveFilterGroup(expectedFilterGroup);

            var actualFilterManager1 = builder.GetResult();

            Assert.Empty(actualFilterManager1.FilterGroups);
        }

        /// <summary>
        ///     Тест сброса строителя менеджера фильтров
        /// </summary>
        [Fact(DisplayName = "Reset FilterManagerBuilder Test")]
        public void ResetFilterManagerBuilder_Test()
        {
            var logger = Substitute.For<ILoggerDecorator>();
            var builder = new FilterManagerBuilder(logger);
            var expectedFilterGroup = new FilterGroup();

            builder.AddFilterGroup(expectedFilterGroup);
            builder.AddFilterGroup(expectedFilterGroup);

            var actualFilterManager = builder.GetResult();

            Assert.Equal(2, actualFilterManager.FilterGroups.Count);
            TestExtensions.CheckingAssertions(expectedFilterGroup, actualFilterManager.FilterGroups.First() as FilterGroup);

            builder.Reset();

            var actualFilterManager1 = builder.GetResult();
            Assert.Empty(actualFilterManager1.FilterGroups);
        }
    }
}
