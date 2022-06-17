using AutoMapper;
using BinanceExchange;
using BinanceExchange.EndpointSenders;
using BinanceExchange.Impl;
using BinanceExchange.Models;
using BinanceExchange.RequestWeights;
using BinanceExchangeTests.BinanceTests.EndpointSendersTests;
using Common.Models;
using ExchangeLibrary;
using NSubstitute;
using Redis;
using SharedForTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinanceExchangeTests.BinanceTests.ImplTests
{
    /// <summary>
    ///     Класс тестирующий <see cref="Marketdata"/>
    /// </summary>
    public class MarketdataTests
    {
        #region Fields

        private readonly MarketdataSenderTest _marketdataSenderTest = new();
        private readonly IMapper _mapper;
        private readonly MarketdataRequestWeightStorage _weightStorage = new();
        private readonly IMarketdata _marketdata;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IMarketdataSender _marketdataSender;

        // поля для проверки верного увеличения лимитов ограничения скорости
        private string _actualKey = "";
        private TimeSpan _actualInterval = TimeSpan.FromSeconds(0);
        private int _actualWeight = 0;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataTests"/>
        public MarketdataTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BinanceMapperProfile>();
            });
            _mapper = new Mapper(config);
            _marketdataSender = Substitute.For<IMarketdataSender>();
            _redisDatabase = Substitute.For<IRedisDatabase>();

            _marketdata = SetupMarketdata();
        }

        /// <summary>
        ///     Настраивает отдельные компоненты биржи, для возврата нужных значений
        /// </summary>
        private IMarketdata SetupMarketdata()
        {
            #region Marketdata Setup

            _marketdataSender.GetExchangeInfoAsync(Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetExchangeInfoAsync_Test();
            });

            _marketdataSender.GetOrderBookAsync(Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetOrderBookAsync_Test();
            });

            _marketdataSender.GetRecentTradesAsync(Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetRecentTradesAsync_Test();
            });

            _marketdataSender.GetOldTradesAsync(Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetOldTradesAsync_Test();
            });

            _marketdataSender.GetCandlestickAsync(Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetCandlestickAsync_Test(
                    "../../../BinanceTests/Jsons/Marketdata/CANDLESTICK_DATA.json",
                    TestHelper.GetBinanceCandlestickModels());
            });

            _marketdataSender.GetAveragePriceAsync(Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetAveragePriceAsync_Test();
            });

            var dayPriceChangeModel = TestHelper.GetBinanceDayPriceChangeModels();
            _marketdataSender.GetDayPriceChangeAsync("", Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetDayPriceChangeAsync_Test(
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/DAY_PRICE_CHANGE_SYMBOL_IS_NULL.json",
                    new List<BinanceExchange.Models.DayPriceChangeModel>
                    {
                        dayPriceChangeModel,
                        dayPriceChangeModel
                    });
            });

            _marketdataSender.GetDayPriceChangeAsync("BNBBTC", Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetDayPriceChangeAsync_Test(
                    "BNBBTC",
                    "../../../BinanceTests/Jsons/Marketdata/DAY_PRICE_CHANGE_SYMBOL.json",
                    new List<BinanceExchange.Models.DayPriceChangeModel>
                    {
                        dayPriceChangeModel,
                    });
            });

            _marketdataSender.GetSymbolPriceTickerAsync("", Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetSymbolPriceTickerAsync_Test(
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_PRICE_TICKERS.json",
                    new List<SymbolPriceTickerModel>
                    {
                        TestHelper.CreatetBinanceSymbolPriceTickerModel("LTCBTC", 4.00000200),
                        TestHelper.CreatetBinanceSymbolPriceTickerModel("ETHBTC", 0.07946600)
                    });
            });

            _marketdataSender.GetSymbolPriceTickerAsync("LTCBTC", Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetSymbolPriceTickerAsync_Test(
                    "LTCBTC",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_PRICE_TICKER.json",
                    new List<SymbolPriceTickerModel>
                    {
                        TestHelper.CreatetBinanceSymbolPriceTickerModel("LTCBTC", 4.00000200),
                    });
            });

            _marketdataSender.GetSymbolOrderBookTickerAsync("", Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetSymbolOrderBookTickerAsync_Test(
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_ORDER_BOOK_TICKERS.json",
                    new List<SymbolOrderBookTickerModel>
                    {
                        TestHelper.CreateBinanceSymbolOrderBookTickerModel("LTCBTC", 4.00000000, 431.00000000, 4.00000200, 9.00000000),
                        TestHelper.CreateBinanceSymbolOrderBookTickerModel("ETHBTC", 0.07946700, 9.00000000, 100000.00000000, 1000.00000000)
                    });
            });

            _marketdataSender.GetSymbolOrderBookTickerAsync("LTCBTC", Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetSymbolOrderBookTickerAsync_Test(
                    "LTCBTC",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_ORDER_BOOK_TICKER.json",
                    new List<SymbolOrderBookTickerModel>
                    {
                        TestHelper.CreateBinanceSymbolOrderBookTickerModel("LTCBTC", 4.00000000, 431.00000000, 4.00000200, 9.00000000)
                    });
            });

            #endregion

            return new Marketdata(_marketdataSender, _redisDatabase, _mapper);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Событие для проверки актуальных аргументов
        /// </summary>
        public Action<string, TimeSpan, int> SetArgumentsEvent { get; set; }

        #endregion

        #region Tests

        /// <summary>
        ///     Проверка получения текущих правил биржевой торговли и информации о символах
        /// </summary>
        [Fact(DisplayName = "Get exchange info Test")]
        public async Task GetExchangeInfoAsync_Test()
        {
            var requestWeight = _weightStorage.ExchangeInfoWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _marketdata.GetExchangeInfoAsync()).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(2, result.Count);
            Assert.Equal($"ETHBTC_0", result[0].Name);
            Assert.Equal("Trading", result[0].Status);
            Assert.Equal("ETH", result[0].BaseAsset);
            Assert.Equal("BTC", result[0].QuoteAsset);
            Assert.Equal(1, result[0].BaseAssetPrecision);
            Assert.Equal(2, result[0].QuotePrecision);
            Assert.True(result[0].IsIcebergAllowed);
            Assert.True(result[0].IsOcoAllowed);

            Assert.Equal($"ETHBTC_1", result[1].Name);
            Assert.Equal("Trading", result[1].Status);
            Assert.Equal("ETH", result[1].BaseAsset);
            Assert.Equal("BTC", result[1].QuoteAsset);
            Assert.Equal(2, result[1].BaseAssetPrecision);
            Assert.Equal(3, result[1].QuotePrecision);
            Assert.True(result[1].IsIcebergAllowed);
            Assert.True(result[1].IsOcoAllowed);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        /// <summary>
        ///     Проверка получения книги ордеров по определенной паре
        /// </summary>
        [Fact(DisplayName = "Get order book Test")]
        public async Task GetOrderBookAsync_Test()
        {
            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = await _marketdata.GetOrderBookAsync("_");

            Assert.Equal(2, result.Asks.Count);
            Assert.Equal(2, result.Bids.Count);

            Assert.Equal(4.00000000, result.Bids[0].Price);
            Assert.Equal(431.00000000, result.Bids[0].Quantity);
            Assert.Equal(5.00000000, result.Bids[1].Price);
            Assert.Equal(15.00000000, result.Bids[1].Quantity);
            Assert.Equal(4.00000200, result.Asks[0].Price);
            Assert.Equal(12.00000000, result.Asks[0].Quantity);
            Assert.Equal(6.00000200, result.Asks[1].Price);
            Assert.Equal(18.00000000, result.Asks[1].Quantity);

            // проверка получения и простановки правильных весов запроса
            var requestWeight = _weightStorage.OrderBookWeight;
            var weightKeys = new string[] { "5", "10", "20", "50", "100", "500", "1000", "5000" };
            SetArgumentsEvent += SetArgumentsEventHandler;
            for (var i = 0; i < weightKeys.Length; i++)
            {
                var key = weightKeys[i];
                var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, key);
                await _marketdata.GetOrderBookAsync("_", int.Parse(key));

                Assert.Equal(expectedKey, _actualKey);
                Assert.Equal(expectedInterval, _actualInterval);
                Assert.Equal(expectedWeight, _actualWeight);
            }

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка получения последних сделок по паре
        /// </summary>
        [Fact(DisplayName = "Get recent trades Test")]
        public async Task GetRecentTradesAsync_Test()
        {
            var requestWeight = _weightStorage.RecentTradesWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _marketdata.GetRecentTradesAsync("_")).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Single(result);
            Assert.Equal(28457, result[0].Id);
            Assert.Equal(4.00000100, result[0].Price);
            Assert.Equal(12.00000000, result[0].Quantity);
            Assert.Equal(48.000012, result[0].QuoteQty);
            Assert.Equal(1499865549590, result[0].TimeUnix);
            Assert.True(result[0].IsBuyerMaker);
            Assert.True(result[0].IsBestMatch);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        /// <summary>
        ///     Проверка получения исторических сделок по паре
        /// </summary>
        [Fact(DisplayName = "Get old trades Test")]
        public async Task GetOldTradesAsync_Test()
        {
            var requestWeight = _weightStorage.OldTradesWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _marketdata.GetOldTradesAsync("_")).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Single(result);
            Assert.Equal(28457, result[0].Id);
            Assert.Equal(4.00000100, result[0].Price);
            Assert.Equal(12.00000000, result[0].Quantity);
            Assert.Equal(48.000012, result[0].QuoteQty);
            Assert.Equal(1499865549590, result[0].TimeUnix);
            Assert.True(result[0].IsBuyerMaker);
            Assert.True(result[0].IsBestMatch);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        /// <summary>
        ///     Проверка получения исторических сделок по паре
        /// </summary>
        [Fact(DisplayName = "Get candlestick Test")]
        public async Task GetCandlestickAsync_Test()
        {
            var requestWeight = _weightStorage.CandlestickDataWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _marketdata.GetCandlestickAsync("_", "1m")).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(2, result.Count);
            var expected = TestHelper.GetExpectedCandlestickModels();
            for (var i = 0; i < 2; i++)
            {
                TestExtensions.CheckingAssertions(expected[i], result[i]);
            }

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        /// <summary>
        ///     Проверка получения текущей средней цены пары
        /// </summary>
        [Fact(DisplayName = "Get average price Test")]
        public async Task GetAveragePriceAsync_Test()
        {
            var requestWeight = _weightStorage.CurrentAveragePriceWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            Assert.Equal(9.35751834, await _marketdata.GetAveragePriceAsync("_"));

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        /// <summary>
        ///     Проверка получения 24 статистики о цене для пары или для всех пар
        /// </summary>
        [Fact(DisplayName = "Get day price change Test")]
        public async Task GetDayPriceChangeAsync_Test()
        {
            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var requestWeight = _weightStorage.DayTickerPriceChangeWeight;
            var symbolWeightKey = new Dictionary<string, string>
            {
                { "", "null" },
                { "BNBBTC", RequestWeightModel.GetDefaultKey() },
            };
            SetArgumentsEvent += SetArgumentsEventHandler;

            var expectedModel = TestHelper.GetExpectedDayPriceChangeModels();
            var expectedResult = new List<Common.Models.DayPriceChangeModel>();
            foreach (var kvp in symbolWeightKey)
            {
                var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, kvp.Value);

                // Act
                var result = (await _marketdata.GetDayPriceChangeAsync(kvp.Key)).ToList();
                switch (kvp.Key)
                {
                    case "":
                        Assert.Equal(2, result.Count);
                        expectedResult = new() { expectedModel, expectedModel, };
                        break;
                    case "BNBBTC":
                        Assert.Single(result);
                        expectedResult = new() { expectedModel };
                        break;
                }

                for (var i = 0; i < expectedResult.Count; i++)
                {
                    TestExtensions.CheckingAssertions(expectedResult[i], result[i]);
                }

                Assert.Equal(expectedKey, _actualKey);
                Assert.Equal(expectedInterval, _actualInterval);
                Assert.Equal(expectedWeight, _actualWeight);
            }

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка получения последней цены для пары или для всех пар
        /// </summary>
        [Fact(DisplayName = "Get symbol price ticker Test")]
        public async Task GetSymbolPriceTickerAsync_Test()
        {
            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var requestWeight = _weightStorage.SymbolPriceTickerWeight;
            var symbolWeightKey = new Dictionary<string, string>
            {
                { "", "null" },
                { "LTCBTC", RequestWeightModel.GetDefaultKey() },
            };
            SetArgumentsEvent += SetArgumentsEventHandler;

            foreach (var kvp in symbolWeightKey)
            {
                var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, kvp.Value);
                var expectedResult = new List<TradeObjectNamePriceModel>();

                // Act
                var result = (await _marketdata.GetSymbolPriceTickerAsync(kvp.Key)).ToList();
                switch (kvp.Key)
                {
                    case "":
                        Assert.Equal(2, result.Count);
                        expectedResult = new()
                        {
                            TestHelper.CreateExpectedSymbolPriceTickerModel("LTCBTC", 4.00000200),
                            TestHelper.CreateExpectedSymbolPriceTickerModel("ETHBTC", 0.07946600),
                        };
                        break;
                    case "LTCBTC":
                        Assert.Single(result);
                        expectedResult = new()
                        {
                            TestHelper.CreateExpectedSymbolPriceTickerModel("LTCBTC", 4.00000200),
                        };
                        break;
                }

                for (var i = 0; i < expectedResult.Count; i++)
                {
                    TestExtensions.CheckingAssertions(expectedResult[i], result[i]);
                }

                Assert.Equal(expectedKey, _actualKey);
                Assert.Equal(expectedInterval, _actualInterval);
                Assert.Equal(expectedWeight, _actualWeight);
            }

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка получения лучшую цену/количество в стакане для символа или символов
        /// </summary>
        [Fact(DisplayName = "Get best symbol orders Test")]
        public async Task GetBestSymbolOrdersAsync_Test()
        {
            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var requestWeight = _weightStorage.SymbolOrderBookTickerWeight;
            var symbolWeightKey = new Dictionary<string, string>
            {
                { "", "null" },
                { "LTCBTC", RequestWeightModel.GetDefaultKey() },
            };
            SetArgumentsEvent += SetArgumentsEventHandler;

            foreach (var kvp in symbolWeightKey)
            {
                var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, kvp.Value);
                var expectedResult = new List<BestSymbolOrderModel>();

                // Act
                var result = (await _marketdata.GetBestSymbolOrdersAsync(kvp.Key)).ToList();
                switch (kvp.Key)
                {
                    case "":
                        Assert.Equal(2, result.Count);
                        expectedResult = new()
                        {
                            TestHelper.CreateExpectedBestSymbolOrderModel("LTCBTC", 4.00000000, 431.00000000, 4.00000200, 9.00000000),
                            TestHelper.CreateExpectedBestSymbolOrderModel("ETHBTC", 0.07946700, 9.00000000, 100000.00000000, 1000.00000000)
                        };
                        break;
                    case "LTCBTC":
                        Assert.Single(result);
                        expectedResult = new()
                        {
                            TestHelper.CreateExpectedBestSymbolOrderModel("LTCBTC", 4.00000000, 431.00000000, 4.00000200, 9.00000000),
                        };
                        break;
                }

                for (var i = 0; i < expectedResult.Count; i++)
                {
                    TestExtensions.CheckingAssertions(expectedResult[i], result[i]);
                }

                Assert.Equal(expectedKey, _actualKey);
                Assert.Equal(expectedInterval, _actualInterval);
                Assert.Equal(expectedWeight, _actualWeight);
            }

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик события получения аргументов от функции увеличения совокупного веса запросов
        /// </summary>
        private void SetArgumentsEventHandler(string key, TimeSpan interval, int value)
        {
            _actualKey = key;
            _actualInterval = interval;
            _actualWeight = value;
        }

        /// <summary>
        ///     Создает мок на вызов метода увеличения совокупного веса запросов api и sapi
        /// </summary>
        private void MockRedisIncrementOrCreateKeyValue(IRedisDatabase redisDatabase)
        {
            redisDatabase.TryIncrementOrCreateKeyValue(
                Arg.Any<string>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<int>()).ReturnsForAnyArgs(callInfo =>
                {
                    var actualKey = callInfo.ArgAt<string>(0);
                    var actualInterval = callInfo.ArgAt<TimeSpan>(1);
                    var actualWeight = callInfo.ArgAt<int>(2);

                    SetArgumentsEvent?.Invoke(actualKey, actualInterval, actualWeight);

                    return true;
                });
        }

        #endregion
    }
}
