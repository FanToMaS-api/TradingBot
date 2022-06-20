using Analytic.Filters;
using Common.Models;
using ExchangeLibrary;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnalyticTests.FilterTests
{
    /// <summary>
    ///     Тестирует класс <see cref="VolumeFilter"/>
    /// </summary>
    public class VolumeFilterTests
    {
        #region Fields

        private readonly OrderBookModel _bidGreaterAskModel = new()
        {
            Bids = new() { new() { Quantity = 10 } },
            Asks = new() { new() { Quantity = 4 } },
        };

        private readonly OrderBookModel _askGreaterBidModel = new()
        {
            Bids = new() { new() { Quantity = 5 } },
            Asks = new() { new() { Quantity = 10 } },
        };

        #endregion

        /// <summary>
        ///     Тест фильтрации: спрос больше предложения
        /// </summary>
        [Fact(DisplayName = "Bid greater than ask Test")]
        public async Task BidGreaterThanAsk_Test()
        {
            var filter = new VolumeFilter(
                "BidGreaterThanAsk",
                VolumeType.Bid,
                VolumeComparisonType.GreaterThan,
                0.5);

            var scopeFactoryMockBidGreaterAsk = GetServiceScopeFactoryMock(10, 4);
            Assert.True(await filter.CheckConditionsAsync(scopeFactoryMockBidGreaterAsk, new("Any"), CancellationToken.None));

            var scopeFactoryMockAskGreaterBid = GetServiceScopeFactoryMock(5, 10);
            Assert.False(await filter.CheckConditionsAsync(scopeFactoryMockAskGreaterBid, new("Any"), CancellationToken.None));
        }

        /// <summary>
        ///     Тест фильтрации: спрос меньше предложения
        /// </summary>
        [Fact(DisplayName = "Bid less than ask Test")]
        public async Task BidLessThanAsk_Test()
        {
            var filter = new VolumeFilter(
                "BidLessThanAsk",
                VolumeType.Bid,
                VolumeComparisonType.LessThan,
                0.5);

            var scopeFactoryMockBidGreaterAsk = GetServiceScopeFactoryMock(10, 4);
            Assert.False(await filter.CheckConditionsAsync(scopeFactoryMockBidGreaterAsk, new("Any"), CancellationToken.None));

            var scopeFactoryMockAskGreaterBid = GetServiceScopeFactoryMock(5, 10);
            Assert.True(await filter.CheckConditionsAsync(scopeFactoryMockAskGreaterBid, new("Any"), CancellationToken.None));
        }

        /// <summary>
        ///     Тест фильтрации: предложение больше спроса
        /// </summary>
        [Fact(DisplayName = "Ask greater than bid Test")]
        public async Task AskGreaterThanBid_Test()
        {
            var filter = new VolumeFilter(
                "AskGreaterThanBid",
                VolumeType.Ask,
                VolumeComparisonType.GreaterThan,
                0.5);

            var scopeFactoryMockBidGreaterAsk = GetServiceScopeFactoryMock(10, 4);
            Assert.False(await filter.CheckConditionsAsync(scopeFactoryMockBidGreaterAsk, new("Any"), CancellationToken.None));

            var scopeFactoryMockAskGreaterBid = GetServiceScopeFactoryMock(5, 10);
            Assert.True(await filter.CheckConditionsAsync(scopeFactoryMockAskGreaterBid, new("Any"), CancellationToken.None));
        }

        /// <summary>
        ///     Тест фильтрации: предложение меньше спроса
        /// </summary>
        [Fact(DisplayName = "Ask less than bid Test")]
        public async Task AskLessThanBid_Test()
        {
            var filter = new VolumeFilter(
                "AskLessThanBid",
                VolumeType.Ask,
                VolumeComparisonType.LessThan,
                0.5);

            var scopeFactoryMockBidGreaterAsk = GetServiceScopeFactoryMock(10, 4);
            Assert.True(await filter.CheckConditionsAsync(scopeFactoryMockBidGreaterAsk, new("Any"), CancellationToken.None));

            var scopeFactoryMockAskGreaterBid = GetServiceScopeFactoryMock(5, 10);
            Assert.False(await filter.CheckConditionsAsync(scopeFactoryMockAskGreaterBid, new("Any"), CancellationToken.None));
        }

        #region Private methods

        /// <summary>
        ///     Создает мок <see cref="IServiceScopeFactory"/>
        /// </summary>
        /// <param name="bidVolume"> Объем спроса, который будет возвращен по запросу </param>
        /// <param name="askVolume"> Объем предложения, который будет возвращен по запросу </param>
        private static IServiceScopeFactory GetServiceScopeFactoryMock(double bidVolume, double askVolume)
        {
            var marketdataMock = Substitute.For<IMarketdata>();
            var exchangeMock = Substitute.For<IExchange>();
            var scopeMock = Substitute.For<IServiceScope>();
            var scopeFactoryMock = Substitute.For<IServiceScopeFactory>();

            scopeFactoryMock.CreateScope().Returns(scopeMock);
            scopeMock.ServiceProvider.GetService<IExchange>().Returns(exchangeMock);
            exchangeMock.Marketdata.Returns(marketdataMock);
            exchangeMock.Marketdata.GetOrderBookAsync(default, default, default).ReturnsForAnyArgs(
                Task.FromResult(
                    new OrderBookModel
                    {
                        Bids = new() { new OrderModel() { Quantity = bidVolume } },
                        Asks = new() { new OrderModel() { Quantity = askVolume } },
                    }));

            return scopeFactoryMock;
        }

        #endregion
    }
}
