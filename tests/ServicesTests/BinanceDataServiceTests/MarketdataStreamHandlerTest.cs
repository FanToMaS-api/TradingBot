using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDataService.DataHandlers;
using Common.Models;
using Common.WebSocket;
using ExchangeLibrary;
using Logger;
using NSubstitute;
using Quartz;
using Scheduler;
using SharedForTest;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace BinanceDataServiceTests
{
    /// <summary>
    ///     Тестирует <see cref="MarketdataStreamHandler"/>
    /// </summary>
    public class MarketdataStreamHandlerTest : IClassFixture<DatabaseFixture>, IDisposable
    {
        #region Fields

        private readonly MarketdataStreamHandler _dataHandler;
        private readonly DatabaseFixture _databaseFixture;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataStreamHandlerTest"/>
        public MarketdataStreamHandlerTest(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            var mapperConfig = new MapperConfiguration(
                mc =>
                {
                    mc.AddProfile(new BinanceDatabaseMappingProfile());
                });

            var webSocket = Substitute.For<IWebSocket>();
            var exchange = Substitute.For<IExchange>();
            exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(null, CancellationToken.None, null)
                .ReturnsForAnyArgs(webSocket);

            var scheduler = Substitute.For<IRecurringJobScheduler>();
            scheduler.ScheduleAsync(null, null).ReturnsForAnyArgs(new TriggerKey("Test"));

            _dataHandler = new MarketdataStreamHandler(
                exchange,
                scheduler,
                mapperConfig.CreateMapper(),
                LoggerManager.CreateDefaultLogger());
            _dataHandler.StartAsync(CancellationToken.None).Wait();
        }

        #endregion

        #region Tests

        /// <summary>
        ///     Тест функции аггрегирования данных объекта
        /// </summary>
        [Fact(DisplayName = "Aggregate fields Test")]
        public void AggregateFields_Test()
        {
            var addedObject = new MiniTickerEntity
            {
                OpenPrice = 1,
                MaxPrice = 1,
                ClosePrice = 1,
                MinPrice = 1,
                QuotePurchaseVolume = 1,
                BasePurchaseVolume = 1,
            };
            var aggregateObject = new MiniTickerEntity
            {
                OpenPrice = 1,
                MaxPrice = 1,
                ClosePrice = 1,
                MinPrice = 1,
                QuotePurchaseVolume = 1,
                BasePurchaseVolume = 1,
            };

            MarketdataStreamHandler.AggregateFields(addedObject, aggregateObject);

            Assert.Equal(1, addedObject.OpenPrice);
            Assert.Equal(1, addedObject.BasePurchaseVolume);

            Assert.Equal(2, aggregateObject.OpenPrice);
            Assert.Equal(2, aggregateObject.MaxPrice);
            Assert.Equal(2, aggregateObject.ClosePrice);
            Assert.Equal(2, aggregateObject.MinPrice);
            Assert.Equal(2, aggregateObject.QuotePurchaseVolume);
            Assert.Equal(2, aggregateObject.BasePurchaseVolume);
        }

        /// <summary>
        ///     Тест функции усреднения значений полей объекта
        /// </summary>
        [Fact(DisplayName = "Averaging fields Test")]
        public void AveragingFields_Test()
        {
            var aggregateObject = new MiniTickerEntity
            {
                OpenPrice = 2.5,
                MaxPrice = 2.5,
                ClosePrice = 2.5,
                MinPrice = 2.5,
                QuotePurchaseVolume = 2.5,
                BasePurchaseVolume = 2.5,
            };

            MarketdataStreamHandler.AveragingFields(aggregateObject, 2);

            Assert.Equal(1.25, aggregateObject.OpenPrice);
            Assert.Equal(1.25, aggregateObject.MaxPrice);
            Assert.Equal(1.25, aggregateObject.ClosePrice);
            Assert.Equal(1.25, aggregateObject.MinPrice);
            Assert.Equal(1.25, aggregateObject.QuotePurchaseVolume);
            Assert.Equal(1.25, aggregateObject.BasePurchaseVolume);
        }

        /// <summary>
        ///     Тест функции добавления усредненной модели в результирующий список и сброса параметров
        /// </summary>
        [Fact(DisplayName = "Add to result Test")]
        public void AddToResult_Test()
        {
            var aggregateObject = new MiniTickerEntity
            {
                OpenPrice = 2.5,
                MaxPrice = 2.5,
                ClosePrice = 2.5,
                MinPrice = 2.5,
                QuotePurchaseVolume = 2.5,
                BasePurchaseVolume = 2.5,
            };

            var list = new List<MiniTickerEntity>();
            var counter = 100;
            var isDataLeft = true;
            MarketdataStreamHandler.AddToResult(list, aggregateObject, ref counter, ref isDataLeft);

            Assert.False(isDataLeft);
            Assert.Equal(1, counter);
            Assert.Single(list);
        }

        #region Memeber data for GetGroupedMiniTickers_Test

        private static readonly DateTime StartDate = new(2022, 12, 12, 12, 12, 12);

        private static readonly List<MiniTradeObjectStreamModel> _streamModels = new()
        {
            CreateMiniTradeStreamModel(2.5, 2.5, 2.5, 2.5, 2.5, 2.5, new DateTimeOffset(StartDate).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(5, 5, 5, 5, 5, 5, new DateTimeOffset(StartDate.AddSeconds(10)).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(7, 7, 7, 7, 7, 7, new DateTimeOffset(StartDate.AddSeconds(20)).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(9, 9, 9, 9, 9, 9, new DateTimeOffset(StartDate.AddSeconds(30)).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(10, 10, 10, 10, 10, 10, new DateTimeOffset(StartDate.AddSeconds(60)).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(11, 11, 11, 11, 11, 11, new DateTimeOffset(StartDate.AddSeconds(70)).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(12, 12, 12, 12, 12, 12, new DateTimeOffset(StartDate.AddMinutes(2)).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(13, 13, 13, 13, 13, 13, new DateTimeOffset(StartDate.AddMinutes(3)).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(14, 14, 14, 14, 14, 14, new DateTimeOffset(StartDate.AddHours(1)).ToUnixTimeMilliseconds()),
            CreateMiniTradeStreamModel(15, 15, 15, 15, 15, 15, new DateTimeOffset(StartDate.AddHours(2)).ToUnixTimeMilliseconds()),
        };

        private static readonly List<MiniTickerEntity> _expectedModelsForDefault = new()
        {
            CreateMiniTickerEntity(2.5, 2.5, 2.5, 2.5, 2.5, 2.5, StartDate, AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(5, 5, 5, 5, 5, 5, StartDate.AddSeconds(10), AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(7, 7, 7, 7, 7, 7, StartDate.AddSeconds(20), AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(9, 9, 9, 9, 9, 9, StartDate.AddSeconds(30), AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(10, 10, 10, 10, 10, 10, StartDate.AddSeconds(60), AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(11, 11, 11, 11, 11, 11, StartDate.AddSeconds(70), AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(12, 12, 12, 12, 12, 12, StartDate.AddMinutes(2), AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(13, 13, 13, 13, 13, 13, StartDate.AddMinutes(3), AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(14, 14, 14, 14, 14, 14, StartDate.AddHours(1), AggregateDataIntervalType.Default),
            CreateMiniTickerEntity(15, 15, 15, 15, 15, 15, StartDate.AddHours(2), AggregateDataIntervalType.Default)
        };

        private static readonly List<MiniTickerEntity> _expectedModelsForOneMinute = new()
        {
            CreateMiniTickerEntity(6.7, 6.7, 6.7, 6.7, 6.7, 6.7, StartDate, AggregateDataIntervalType.OneMinute),
            CreateMiniTickerEntity(11.5, 11.5, 11.5, 11.5, 11.5, 11.5, StartDate.AddSeconds(70), AggregateDataIntervalType.OneMinute),
            CreateMiniTickerEntity(13, 13, 13, 13, 13, 13, StartDate.AddMinutes(3), AggregateDataIntervalType.OneMinute),
            CreateMiniTickerEntity(14, 14, 14, 14, 14, 14, StartDate.AddHours(1), AggregateDataIntervalType.OneMinute),
            CreateMiniTickerEntity(15, 15, 15, 15, 15, 15, StartDate.AddHours(2), AggregateDataIntervalType.OneMinute)
        };

        private static readonly List<MiniTickerEntity> _expectedModelsForOneHour = new()
        {
            CreateMiniTickerEntity(83.5 / 9, 83.5 / 9, 83.5 / 9, 83.5 / 9, 83.5 / 9, 83.5 / 9, StartDate, AggregateDataIntervalType.OneHour),
            CreateMiniTickerEntity(15, 15, 15, 15, 15, 15, StartDate.AddHours(2), AggregateDataIntervalType.OneHour)
        };

        public static readonly IEnumerable<object[]> IntervalsStreamModels = new List<object[]>
        {
            new object[] { AggregateDataIntervalType.Default, _streamModels, _expectedModelsForDefault},
            new object[] { AggregateDataIntervalType.OneMinute, _streamModels, _expectedModelsForOneMinute },
            new object[] { AggregateDataIntervalType.OneHour, _streamModels, _expectedModelsForOneHour }
        };

        #endregion

        /// <summary>
        ///     Тест группировки данных через усреднение
        /// </summary>
        [Theory]
        [MemberData(nameof(IntervalsStreamModels))]
        public void GetGroupedMiniTickers_Test(
            AggregateDataIntervalType intervalType,
            IEnumerable<MiniTradeObjectStreamModel> streamModels,
            MiniTickerEntity[] expectedResult)
        {
            var actual = _dataHandler.GetGroupedMiniTickers(intervalType, streamModels);
            Assert.Equal(expectedResult.Length, actual.Length);
            for (var i = 0; i < expectedResult.Length; i++)
            {
                TestExtensions.CheckingAssertions(expectedResult[i], actual[i]);
            }
        }



        #endregion

        #region Private methods

        /// <summary>
        ///     Создает модель с заданным временм получения
        /// </summary>
        private static MiniTradeObjectStreamModel CreateMiniTradeStreamModel(
            double openPrice,
            double maxPrice,
            double closePrice,
            double minPrice,
            double quotePurchaseVolume,
            double basePurchaseVolume,
            long eventTimeUnix) =>
            new()
            {
                OpenPrice = openPrice,
                MaxPrice = maxPrice,
                ClosePrice = closePrice,
                MinPrice = minPrice,
                QuotePurchaseVolume = quotePurchaseVolume,
                BasePurchaseVolume = basePurchaseVolume,
                ShortName = "SOLUSDT",
                EventTimeUnix = eventTimeUnix
            };

        /// <summary>
        ///     Создает модель с заданным временм получения
        /// </summary>
        private static MiniTickerEntity CreateMiniTickerEntity(
            double openPrice,
            double maxPrice,
            double closePrice,
            double minPrice,
            double quotePurchaseVolume,
            double basePurchaseVolume,
            DateTime eventTime,
            AggregateDataIntervalType aggregateDataInterval) =>
            new()
            {
                OpenPrice = openPrice,
                MaxPrice = maxPrice,
                ClosePrice = closePrice,
                MinPrice = minPrice,
                QuotePurchaseVolume = quotePurchaseVolume,
                BasePurchaseVolume = basePurchaseVolume,
                ShortName = "SOLUSDT",
                EventTime = eventTime,
                AggregateDataInterval = aggregateDataInterval
            };

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
