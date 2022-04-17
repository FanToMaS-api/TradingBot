using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDataService.DataHandlers;
using Common.WebSocket;
using ExchangeLibrary;
using Logger;
using NSubstitute;
using Quartz;
using Scheduler;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace BinanceDataServiceTests
{
    /// <summary>
    ///     Тестирует <see cref="MarketdataStreamHandler"/>
    /// </summary>
    public class MarketdataStreamHandlerTest
    {
        #region Fields

        private readonly MarketdataStreamHandler _handler;

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

            _handler = new MarketdataStreamHandler(
                exchange,
                scheduler,
                mapperConfig.CreateMapper(),
                LoggerManager.CreateDefaultLogger());
            _handler.StartAsync(CancellationToken.None).Wait();
        }

        #endregion

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
            Assert.Equal(0, counter);
            Assert.Single(list);
        }
    }
}
