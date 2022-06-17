using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDataService.DataAggregators;
using BinanceDataService.DataHandlers;
using Common.Models;
using Common.WebSocket;
using ExchangeLibrary;
using ExtensionsLibrary;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Quartz;
using Scheduler;
using SharedForTest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinanceDataServiceTests
{
    /// <summary>
    ///     Тестирует <see cref="MarketdataStreamHandler"/>
    /// </summary>
    public class MarketdataStreamHandlerTest
    {
        #region Fields

        private readonly MarketdataStreamHandler _dataHandler;
        private static readonly DateTime StartDate = new(2022, 12, 12, 12, 12, 12, DateTimeKind.Unspecified);

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataStreamHandlerTest"/>
        public MarketdataStreamHandlerTest()
        {
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
                new(),
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
            var addedObject = CreateMiniTickerEntity(1, 5, 1, 0.2, 1, 1, StartDate, AggregateDataIntervalType.Default);
            var aggregateObject = CreateMiniTickerEntity(1, 4, 1, 0.1, 1, 1, StartDate, AggregateDataIntervalType.Default);

            DataAggregator.AggregateFields(addedObject, aggregateObject);

            Assert.Equal(1, addedObject.OpenPrice);
            Assert.Equal(1, addedObject.BasePurchaseVolume);

            Assert.Equal(2, aggregateObject.OpenPrice);
            Assert.Equal(5, aggregateObject.MaxPrice);
            Assert.Equal(2, aggregateObject.ClosePrice);
            Assert.Equal(0.1, aggregateObject.MinPrice);
            Assert.Equal(2, aggregateObject.QuotePurchaseVolume);
            Assert.Equal(2, aggregateObject.BasePurchaseVolume);
        }

        /// <summary>
        ///     Тест функции усреднения значений полей объекта
        /// </summary>
        [Fact(DisplayName = "Averaging fields Test")]
        public void AveragingFields_Test()
        {
            var aggregateObject = CreateMiniTickerEntity(2.5, 2.5, 2.5, 2.5, 2.5, 2.5, StartDate, AggregateDataIntervalType.Default);

            DataAggregator.AveragingFields(aggregateObject, 2);

            Assert.Equal(1.25, aggregateObject.OpenPrice);
            Assert.Equal(2.5, aggregateObject.MaxPrice);
            Assert.Equal(1.25, aggregateObject.ClosePrice);
            Assert.Equal(2.5, aggregateObject.MinPrice);
            Assert.Equal(1.25, aggregateObject.QuotePurchaseVolume);
            Assert.Equal(1.25, aggregateObject.BasePurchaseVolume);
        }

        #region Member data for GetAveragingMiniTickers_Test

        private static readonly List<MiniTradeObjectStreamModel> _streamModels = new()
        {
            CreateMiniTradeStreamModel(2.5, 2.5, 2.5, 2.5, 2.5, 2.5, StartDate.FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(5, 5, 5, 5, 5, 5, StartDate.AddSeconds(10).FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(7, 7, 7, 7, 7, 7, StartDate.AddSeconds(20).FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(9, 9, 9, 9, 9, 9, StartDate.AddSeconds(30).FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(10, 10, 10, 10, 10, 10, StartDate.AddSeconds(60).FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(11, 11, 11, 11, 11, 11, StartDate.AddSeconds(70).FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(12, 12, 12, 12, 12, 12, StartDate.AddMinutes(2).FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(13, 13, 13, 13, 13, 13, StartDate.AddMinutes(3).FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(14, 14, 14, 14, 14, 14, StartDate.AddHours(1).FromDateTimeToUnix()),
            CreateMiniTradeStreamModel(15, 15, 15, 15, 15, 15, StartDate.AddHours(2).FromDateTimeToUnix()),
        };

        private static readonly List<MiniTickerEntity> _entities = new()
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
            CreateMiniTickerEntity(15, 15, 15, 15, 15, 15, StartDate.AddHours(2), AggregateDataIntervalType.Default),
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
            CreateMiniTickerEntity(6.7, 10, 6.7, 2.5, 6.7, 6.7, StartDate, AggregateDataIntervalType.OneMinute),
            CreateMiniTickerEntity(11.5, 12, 11.5, 11, 11.5, 11.5, StartDate.AddSeconds(70), AggregateDataIntervalType.OneMinute),
            CreateMiniTickerEntity(13, 13, 13, 13, 13, 13, StartDate.AddMinutes(3), AggregateDataIntervalType.OneMinute),
            CreateMiniTickerEntity(14, 14, 14, 14, 14, 14, StartDate.AddHours(1), AggregateDataIntervalType.OneMinute),
            CreateMiniTickerEntity(15, 15, 15, 15, 15, 15, StartDate.AddHours(2), AggregateDataIntervalType.OneMinute)
        };

        private static readonly List<MiniTickerEntity> _expectedModelsForOneHour = new()
        {
            CreateMiniTickerEntity(83.5 / 9, 14, 83.5 / 9, 2.5, 83.5 / 9, 83.5 / 9, StartDate, AggregateDataIntervalType.OneHour),
            CreateMiniTickerEntity(15, 15, 15, 15, 15, 15, StartDate.AddHours(2), AggregateDataIntervalType.OneHour)
        };

        public static readonly IEnumerable<object[]> IntervalsStreamModels = new List<object[]>
        {
            new object[] { AggregateDataIntervalType.Default, _entities, _expectedModelsForDefault},
            new object[] { AggregateDataIntervalType.OneMinute, _entities, _expectedModelsForOneMinute },
            new object[] { AggregateDataIntervalType.OneHour, _entities, _expectedModelsForOneHour }
        };

        #endregion

        /// <summary>
        ///     Тест агрегирования данных через усреднение
        /// </summary>
        [Theory(DisplayName = "Get averaging mini tickers Test")]
        [MemberData(nameof(IntervalsStreamModels))]
        public void GetAveragingMiniTickers_Test(
            AggregateDataIntervalType intervalType,
            IEnumerable<MiniTickerEntity> entities,
            MiniTickerEntity[] expectedResult)
        {
            var actual = DataAggregator.GetAveragingTicker(entities, intervalType);
            Assert.Equal(expectedResult.Length, actual.Count);
            for (var i = 0; i < expectedResult.Length; i++)
            {
                TestExtensions.CheckingAssertions(expectedResult[i], actual[i]);
            }
        }

        /// <summary>
        ///     Тест обработки данных
        /// </summary>
        [Fact(DisplayName = "Handle data async Test")]
        public async Task HandleDataAsync_Test()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var databaseFactory = Substitute.For<IBinanceDbContextFactory>();
            var unitOfWork = Substitute.For<IUnitOfWork>();
            databaseFactory.CreateScopeDatabase().Returns(unitOfWork);
            serviceProvider.GetService<IBinanceDbContextFactory>().ReturnsForAnyArgs(databaseFactory);

            // Act #1
            await _dataHandler.OnDataReceived(_streamModels, CancellationToken.None);

            var isAssistantStorageSaving = _dataHandler.IsAssistantStorageSaving;
            await _dataHandler.SaveDataAsync(serviceProvider);
            Assert.Equal(!isAssistantStorageSaving, _dataHandler.IsAssistantStorageSaving);

            // Act #1
            _streamModels.AddRange(_streamModels);
            await _dataHandler.OnDataReceived(_streamModels, CancellationToken.None);

            isAssistantStorageSaving = _dataHandler.IsAssistantStorageSaving;
            await _dataHandler.SaveDataAsync(serviceProvider);
            Assert.Equal(!isAssistantStorageSaving, _dataHandler.IsAssistantStorageSaving);
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
    }
}
