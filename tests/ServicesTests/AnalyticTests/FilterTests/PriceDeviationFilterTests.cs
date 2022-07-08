using Analytic.Filters;
using Analytic.Filters.Enums;
using Analytic.Models;
using BinanceDatabase;
using BinanceDatabase.Repositories;
using BinanceDatabase.Repositories.ColdRepositories;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnalyticTests.FilterTests
{
    /// <summary>
    ///     Тестирует <see cref="BinancePriceDeviationFilter"/>
    /// </summary>
    public class PriceDeviationFilterTests
    {
        #region Fields

        public static readonly IEnumerable<object[]> InfoModelsMemberData = new List<object[]>
        {
            new object[]
            {
                CreatePriceDeviationFilter(ComparisonType.GreaterThan, AggregateDataIntervalType.Default, 10, 60),
                18.299999999999997,
                true
            },
            new object[]
            {
                CreatePriceDeviationFilter(ComparisonType.GreaterThan, AggregateDataIntervalType.Default, 20, 60),
                18.299999999999997,
                false
            },
            new object[]
            {
                CreatePriceDeviationFilter(ComparisonType.LessThan, AggregateDataIntervalType.Default, 10, 6),
                0.15000000000000002,
                true
            },
            new object[]
            {
                CreatePriceDeviationFilter(ComparisonType.LessThan, AggregateDataIntervalType.Default, 0.2, 6),
                0.21000000000000002,
                false
            },
            new object[]
            {
                CreatePriceDeviationFilter(ComparisonType.Equal, AggregateDataIntervalType.Default, 0.21000000000000002, 6),
                0.21000000000000002,
                true
            },
            new object[]
            {
                CreatePriceDeviationFilter(ComparisonType.Equal, AggregateDataIntervalType.Default, 0.44999999999999996, 7),
                0.28,
                false
            },
            new object[]
            {
                CreatePriceDeviationFilter(ComparisonType.Equal, AggregateDataIntervalType.Default, 0.44999999999999996, 7),
                0.15000000000000002,
                false
            },
        };

        #endregion

        /// <summary>
        ///     Тест фильтрации по отклонению цены за определенных промежуток времени
        /// </summary>
        [Theory(DisplayName = "Filter by price deviation Test")]
        [MemberData(nameof(InfoModelsMemberData))]
        public async Task FilterByPriceDeviation_Test(
            BinancePriceDeviationFilter filter,
            double expectedSum,
            bool expectedFilterResult)
        {
            var serviceScopeFactory = GetServiceScopeFactoryMock(expectedSum);
            var model = new InfoModel("Any");
            var actualFilterResult = await filter.CheckConditionsAsync(serviceScopeFactory, model, CancellationToken.None);

            Assert.Equal(expectedFilterResult, actualFilterResult);
            Assert.Equal(expectedSum, model.DeviationsSum);
        }

        #region Private methods

        /// <summary>
        ///     Создает модель фильтра с заданными параметрами
        /// </summary>
        /// <param name="comparisonType"> Тип сравнения </param>
        /// <param name="limit"> Ограничение </param>
        /// <param name="timeframeNumber"> Кол-во таймфреймов участвующих в анализе </param>
        private static BinancePriceDeviationFilter CreatePriceDeviationFilter(
            ComparisonType comparisonType,
            AggregateDataIntervalType interval,
            double limit,
            int timeframeNumber)
            => new(
                "BinancePriceDeviationFilter",
                interval,
                comparisonType,
                limit: limit,
                timeframeNumber: timeframeNumber);

        /// <summary>
        ///     Создает мок <see cref="IServiceScopeFactory"/>
        /// </summary>
        /// <param name="neededPriceDeviation"> Требуемое отклонение цены </param>
        private static IServiceScopeFactory GetServiceScopeFactoryMock(double neededPriceDeviation)
        {
            var miniTickerRepositoryMock = Substitute.For<IMiniTickerRepository>();
            var coldUnitOfWorkMock = Substitute.For<IColdUnitOfWork>();
            var databaseMock = Substitute.For<IUnitOfWork>();
            var databaseFactoryMock = Substitute.For<IBinanceDbContextFactory>();
            var scopeMock = Substitute.For<IServiceScope>();
            var scopeFactoryMock = Substitute.For<IServiceScopeFactory>();

            miniTickerRepositoryMock.GetPricePercentDeviationAsync(
                default,
                default,
                default,
                default)
                .ReturnsForAnyArgs(Task.FromResult(neededPriceDeviation));
            coldUnitOfWorkMock.MiniTickers.ReturnsForAnyArgs(miniTickerRepositoryMock);
            databaseMock.ColdUnitOfWork.Returns(coldUnitOfWorkMock);

            databaseFactoryMock.CreateScopeDatabase().Returns(databaseMock);
            scopeMock.ServiceProvider.GetService<IBinanceDbContextFactory>().Returns(databaseFactoryMock);
            scopeFactoryMock.CreateScope().Returns(scopeMock);

            return scopeFactoryMock;
        }

        #endregion
    }
}
