using AutoMapper;
using BinanceExchange;
using BinanceExchange.EndpointSenders;
using BinanceExchange.Enums;
using BinanceExchange.Models;
using BinanceExchangeTests.BinanceTests.EndpointSendersTests;
using Common.Enums;
using Common.Models;
using Common.Redis;
using ExchangeLibrary;
using NSubstitute;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinanceExchangeTests.BinanceTests
{
    /// <summary>
    ///     Класс, тестирующий <see cref="BinanceExchange.BinanceExchange"/>
    /// </summary>
    public class BinanceExchangeTests
    {
        #region Fields

        private readonly WalletSenderTest _walletSenderTest = new();
        private readonly MarketdataSenderTest _marketdataSenderTest = new();
        private readonly SpotAccountTradeSenderTest _spotAccountTradeSenderTest = new();
        private readonly IMapper _mapper;
        private readonly RequestsWeightStorage _requestsWeightStorage = new();
        private readonly IExchange _binanceExchange;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IWalletSender _walletSender;
        private readonly IMarketdataSender _marketdataSender;
        private readonly ISpotAccountTradeSender _tradeSender;

        // поля для проверки верного увеличения лимитов ограничения скорости
        private string _actualKey = "";
        private TimeSpan _actualInterval = TimeSpan.FromSeconds(0);
        private int _actualWeight = 0;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceExchangeTests"/>
        public BinanceExchangeTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BinanceMapperProfile>();
            });
            _mapper = new Mapper(config);
            _walletSender = Substitute.For<IWalletSender>();
            _marketdataSender = Substitute.For<IMarketdataSender>();
            _tradeSender = Substitute.For<ISpotAccountTradeSender>();
            _redisDatabase = Substitute.For<IRedisDatabase>();

            _binanceExchange = SetupBinanceExchange();
        }

        /// <summary>
        ///     Настраивает отдельные компоненты биржи, для возврата нужных значений
        /// </summary>
        private BinanceExchange.BinanceExchange SetupBinanceExchange()
        {
            #region Wallet Setup

            _walletSender.GetSystemStatusAsync(Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _walletSenderTest.GetSystemStatusAsync_Test();
            });

            _walletSender.GetAccountTradingStatusAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _walletSenderTest.GetAccountTradingStatusAsync_Test();
            });

            _walletSender.GetAllCoinsInformationAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _walletSenderTest.GetAllCoinsInformationAsync_Test();
            });

            _walletSender.GetTradeFeeAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _walletSenderTest.GetTradeFeeAsync_Test();
            });

            #endregion

            #region Marketdata Setup

            _marketdataSender.GetExchangeInfoAsync(Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetExchangeInfoAsync_Test();
            });

            _marketdataSender.GetOrderBookAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetOrderBookAsync_Test();
            });

            _marketdataSender.GetRecentTradesAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetRecentTradesAsync_Test();
            });

            _marketdataSender.GetOldTradesAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetOldTradesAsync_Test();
            });

            _marketdataSender.GetCandlestickAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _marketdataSenderTest.GetCandlestickAsync_Test(
                    "../../../BinanceTests/Jsons/Marketdata/CANDLESTICK_DATA.json",
                    TestHelper.GetBinanceCandlestickModels());
            });

            _marketdataSender.GetAveragePriceAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
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

            #region Trade Account Setup

            _tradeSender.GetAccountInformationAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _spotAccountTradeSenderTest.GetAccountInformationAsync_Test();
            });

            #endregion

            return new BinanceExchange.BinanceExchange(_walletSender, _marketdataSender, _tradeSender, _redisDatabase, _mapper);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Событие для проверки актуальных аргументов
        /// </summary>
        public Action<string, TimeSpan, int> SetArgumentsEvent { get; set; }

        #endregion

        #region Speed limit Test

        /// <summary>
        ///     Проверка работы отслеживания частоты запросов
        /// </summary>
        [Fact(DisplayName = "Speed limit Test")]
        public void LimitExceeding_Test()
        {
            var (database, redisDatabase) = CreateMockRedisDatabase();
            var expiry = TimeSpan.FromSeconds(5); // время экспирации ключа
            var valueLimit = 5; // ограничение на скорость запроса
            var currentValue = 1; // текущее значение кол-ва скорости запроса
            var key = "rateLimitTest";
            var rateLimit = new RateLimit(Common.Enums.RateLimitType.API_REQUEST, expiry, valueLimit);

            // сначала возвращаю null, так как база пуста
            database.StringGetWithExpiry(Arg.Any<RedisKey>()).Returns(new RedisValueWithExpiry(new RedisValue(string.Empty), null));
            var actualKey = "";
            int actualValueLimit = 0;
            var actualExpiry = TimeSpan.FromSeconds(0);
            database.StringSet(key, valueLimit, expiry).Returns(callInfo =>
            {
                actualKey = callInfo.ArgAt<RedisKey>(0);
                actualValueLimit = (int)callInfo.ArgAt<RedisValue>(1);
                actualExpiry = callInfo.ArgAt<TimeSpan>(2);

                return true;
            });

            // Установить новый лимит так как его еще нет
            redisDatabase.TryIncrementOrCreateKeyValue(key, rateLimit.Interval, rateLimit.Limit);
            Assert.Equal(key, actualKey);
            Assert.Equal(valueLimit, actualValueLimit);
            Assert.Equal(expiry, actualExpiry);

            // устанавливаю действие на функцию увеличения ключа 
            database.StringIncrement(key, 1).Returns(callInfo =>
            {
                currentValue++;
                return currentValue;
            });

            // Увеличить его пару раз без ошибок
            for (var i = 0; i < 4; i++)
            {
                // теперь возвращаем уже со значением, каждый раз с новым
                database.StringGetWithExpiry(key).Returns(new RedisValueWithExpiry(new RedisValue(currentValue.ToString()), expiry));

                redisDatabase.TryIncrementOrCreateKeyValue(key, rateLimit.Interval, 1);
                Assert.False(IsLimitExceeded());
            }

            // возвращаем уже с новым значением
            database.StringGetWithExpiry(key).Returns(new RedisValueWithExpiry(new RedisValue(currentValue.ToString()), expiry));

            // Увеличить с получением ошибки
            redisDatabase.TryIncrementOrCreateKeyValue(key, rateLimit.Interval, 1);
            Assert.True(IsLimitExceeded());

            // Проверяет на достижение лимита
            bool IsLimitExceeded()
            {
                return redisDatabase.TryGetIntValue(key, out var keyValue) && keyValue.Value >= rateLimit.Limit;
            }
        }

        #endregion

        #region Wallet Tests

        /// <summary>
        ///     Проверка получения статуса системы
        /// </summary>
        [Fact(DisplayName = "Get system status Test")]
        public async Task GetSystemStatusAsync_Test()
        {
            // код проверки верного увеличения лимитов
            var requestWeight = _requestsWeightStorage.SystemStatusWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            Assert.True(await _binanceExchange.GetSystemStatusAsync());

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        /// <summary>
        ///     Проверка получения статуса аккаунта
        /// </summary>
        [Fact(DisplayName = "Get account trading status Test")]
        public async Task GetAccountTradingStatusAsync_Test()
        {
            // код проверки верного увеличения лимитов
            var requestWeight = _requestsWeightStorage.AccountStatusWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = await _binanceExchange.GetAccountTradingStatusAsync();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.True(result.IsLocked);
            Assert.Equal(123, result.PlannedRecoverTimeUnix);
            Assert.Equal(1547630471725, result.UpdateTimeUnix);
            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        /// <summary>
        ///     Проверка получения всей информации о монетах
        /// </summary>
        [Fact(DisplayName = "Get all coins information Test")]
        public async Task GetAllCoinsInformationAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.AllCoinsInfoWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _binanceExchange.GetAllCoinsInformationAsync()).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(2, result.Count);
            Assert.Equal("Bitcoin", result[0].Name);
            Assert.Equal("MyCoin", result[1].Name);
            Assert.Equal("BTC", result[0].Coin);
            Assert.Equal("MyCoin", result[1].Coin);
            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        /// <summary>
        ///     Проверка получения информации о таксе за все монеты или за определенную
        /// </summary>
        [Fact(DisplayName = "Get trade fee Test")]
        public async Task GetTradeFeeAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.TradeFeeWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _binanceExchange.GetTradeFeeAsync()).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(2, result.Count);
            Assert.Equal("ADABNB", result[0].Symbol);
            Assert.Equal(0.001, result[0].MakerCommission);
            Assert.Equal(0.002, result[0].TakerCommission);
            Assert.Equal("BNBBTC", result[1].Symbol);
            Assert.Equal(0.003, result[1].MakerCommission);
            Assert.Equal(0.004, result[1].TakerCommission);
            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
        }

        #endregion

        #region Marketdata Tests

        /// <summary>
        ///     Проверка получения текущих правил биржевой торговли и информации о символах
        /// </summary>
        [Fact(DisplayName = "Get exchange info Test")]
        public async Task GetExchangeInfoAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.ExchangeInfoWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _binanceExchange.GetExchangeInfoAsync()).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(2, result.Count);
            Assert.Equal($"ETHBTC_0", result[0].Symbol);
            Assert.Equal("Trading", result[0].Status);
            Assert.Equal("ETH", result[0].BaseAsset);
            Assert.Equal("BTC", result[0].QuoteAsset);
            Assert.Equal(1, result[0].BaseAssetPrecision);
            Assert.Equal(2, result[0].QuotePrecision);
            Assert.True(result[0].IsIcebergAllowed);
            Assert.True(result[0].IsOcoAllowed);

            Assert.Equal($"ETHBTC_1", result[1].Symbol);
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

            var result = await _binanceExchange.GetOrderBookAsync("_");

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
            var requestWeight = _requestsWeightStorage.OrderBookWeight;
            var weightKeys = new string[] { "5", "10", "20", "50", "100", "500", "1000", "5000" };
            SetArgumentsEvent += SetArgumentsEventHandler;
            for (var i = 0; i < weightKeys.Length; i++)
            {
                var key = weightKeys[i];
                var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, key);
                await _binanceExchange.GetOrderBookAsync("_", int.Parse(key));

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
            var requestWeight = _requestsWeightStorage.RecentTradesWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _binanceExchange.GetRecentTradesAsync("_")).ToList();

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
            var requestWeight = _requestsWeightStorage.OldTradesWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _binanceExchange.GetOldTradesAsync("_")).ToList();

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
            var requestWeight = _requestsWeightStorage.CandlestickDataWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _binanceExchange.GetCandlestickAsync("_", "1m")).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(2, result.Count);
            var expected = TestHelper.GetExpectedCandlestickModels();
            for (var i = 0; i < 2; i++)
            {
                TestHelper.CheckingAssertions(expected[i], result[i]);
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
            var requestWeight = _requestsWeightStorage.CurrentAveragePriceWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            Assert.Equal(9.35751834, await _binanceExchange.GetAveragePriceAsync("_"));

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

            var requestWeight = _requestsWeightStorage.DayTickerPriceChangeWeight;
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
                var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, kvp.Value);

                // Act
                var result = (await _binanceExchange.GetDayPriceChangeAsync(kvp.Key)).ToList();
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
                    TestHelper.CheckingAssertions(expectedResult[i], result[i]);
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

            var requestWeight = _requestsWeightStorage.SymbolPriceTickerWeight;
            var symbolWeightKey = new Dictionary<string, string>
            {
                { "", "null" },
                { "LTCBTC", RequestWeightModel.GetDefaultKey() },
            };
            SetArgumentsEvent += SetArgumentsEventHandler;

            foreach (var kvp in symbolWeightKey)
            {
                var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, kvp.Value);
                var expectedResult = new List<SymbolPriceModel>();

                // Act
                var result = (await _binanceExchange.GetSymbolPriceTickerAsync(kvp.Key)).ToList();
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
                    TestHelper.CheckingAssertions(expectedResult[i], result[i]);
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

            var requestWeight = _requestsWeightStorage.SymbolOrderBookTickerWeight;
            var symbolWeightKey = new Dictionary<string, string>
            {
                { "", "null" },
                { "LTCBTC", RequestWeightModel.GetDefaultKey() },
            };
            SetArgumentsEvent += SetArgumentsEventHandler;

            foreach (var kvp in symbolWeightKey)
            {
                var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, kvp.Value);
                var expectedResult = new List<BestSymbolOrderModel>();

                // Act
                var result = (await _binanceExchange.GetBestSymbolOrdersAsync(kvp.Key)).ToList();
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
                    TestHelper.CheckingAssertions(expectedResult[i], result[i]);
                }

                Assert.Equal(expectedKey, _actualKey);
                Assert.Equal(expectedInterval, _actualInterval);
                Assert.Equal(expectedWeight, _actualWeight);
            }

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        #endregion

        #region Spot Account/Trade Tests (Данные тесты не проверяю результат работы строителя query)

        /// <summary>
        ///     Проверка создания нового лимитного ордера
        /// </summary>
        [Fact(DisplayName = "Create new limit order Test")]
        public async Task CreateNewLimitOrderAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT"; // если изменено нужно поменять в методе мока запроса
            var orderSide = OrderSideType.Buy;
            var forceType = "IOC"; // если изменено нужно поменять в методе мока запроса
            var orderType = "Limit";
            var priceAndQuantity = 0.1;
            var recvWindow = 5000;
            var builder = new Builder();
            builder.SetOrderType(OrderType.Limit);
            builder.SetOrderSideType(orderSide);
            builder.SetSymbol(symbol);
            builder.SetTimeInForce(forceType);
            builder.SetPrice(priceAndQuantity);
            builder.SetQuantity(priceAndQuantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult(false).GetQuery();

            var (binanceResponse, expectedResult) = MockNewOrderCreating(query, priceAndQuantity, OrderType.Limit, orderType);
            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act #1
            var result = await _binanceExchange.CreateNewLimitOrderAsync(symbol, orderSide, forceType, priceAndQuantity, priceAndQuantity, isTest: false);
            TestHelper.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            builder.SetPrice(priceAndQuantity);
            query = builder.GetResult(false).GetQuery();
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _binanceExchange.CreateNewLimitOrderAsync(symbol, orderSide, forceType, priceAndQuantity, priceAndQuantity);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestHelper.CheckingAssertions(expectedResult, testOrderResult);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка создания нового рыночного ордера
        /// </summary>
        [Fact(DisplayName = "Create new market order Test")]
        public async Task CreateNewMarketOrderAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT"; // если изменено нужно поменять в методе мока запроса
            var orderSide = OrderSideType.Buy;
            var orderType = "Market";
            var priceAndQuantity = 0.1;
            var recvWindow = 5000;
            var builder = new Builder();
            builder.SetOrderSideType(orderSide);
            builder.SetSymbol(symbol);
            builder.SetQuantity(priceAndQuantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var (binanceResponse, expectedResult) = MockNewOrderCreating(query, priceAndQuantity, OrderType.Market, orderType);
            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act #1
            var result = await _binanceExchange.CreateNewMarketOrderAsync(symbol, orderSide, priceAndQuantity, isTest: false);
            TestHelper.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            builder.SetQuantity(priceAndQuantity); // обновляю значение кол-ва
            query = builder.GetResult().GetQuery(); // обновляю словарь параметров запроса
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _binanceExchange.CreateNewMarketOrderAsync(symbol, orderSide, priceAndQuantity);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestHelper.CheckingAssertions(expectedResult, testOrderResult);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка создания нового стоп-лосс ордера
        /// </summary>
        [Fact(DisplayName = "Create new stop loss order Test")]
        public async Task CreateNewStopLossOrderAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT"; // если изменено нужно поменять в методе мока запроса
            var sideType = OrderSideType.Buy;
            var orderType = "StopLoss";
            var priceAndQuantity = 0.1;
            var recvWindow = 6000;
            var builder = new Builder();
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetStopPrice(priceAndQuantity);
            builder.SetQuantity(priceAndQuantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var (binanceResponse, expectedResult) = MockNewOrderCreating(query, priceAndQuantity, OrderType.StopLoss, orderType);
            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act #1
            var result = await _binanceExchange.CreateNewStopLossOrderAsync(symbol, sideType, priceAndQuantity, priceAndQuantity, recvWindow, isTest: false);
            TestHelper.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            builder.SetQuantity(priceAndQuantity); // обновляю значение кол-ва
            query = builder.GetResult().GetQuery(); // обновляю словарь параметров запроса
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _binanceExchange.CreateNewStopLossOrderAsync(symbol, sideType, priceAndQuantity, priceAndQuantity, recvWindow);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestHelper.CheckingAssertions(expectedResult, testOrderResult);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка создания нового лимитного стоп-лосс ордера
        /// </summary>
        [Fact(DisplayName = "Create new stop loss limit order Test")]
        public async Task CreateNewStopLossLimitOrderAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT"; // если изменено нужно поменять в методе мока запроса
            var sideType = OrderSideType.Buy;
            var orderType = "StopLossLimit";
            var priceAndQuantity = 0.1;
            var forceType = "IOC"; // если изменено нужно поменять в методе мока запроса
            var recvWindow = 6000;
            var builder = new Builder();
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetTimeInForce(forceType);
            builder.SetPrice(priceAndQuantity);
            builder.SetQuantity(priceAndQuantity);
            builder.SetStopPrice(priceAndQuantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var (binanceResponse, expectedResult) = MockNewOrderCreating(query, priceAndQuantity, OrderType.StopLossLimit, orderType);
            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act #1
            var result = await _binanceExchange.CreateNewStopLossLimitOrderAsync(symbol, sideType, forceType, priceAndQuantity, priceAndQuantity, recvWindow, isTest: false);
            TestHelper.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            builder.SetQuantity(priceAndQuantity); // обновляю значение кол-ва
            query = builder.GetResult().GetQuery(); // обновляю словарь параметров запроса
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _binanceExchange.CreateNewStopLossLimitOrderAsync(symbol, sideType, forceType, priceAndQuantity, priceAndQuantity, recvWindow);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestHelper.CheckingAssertions(expectedResult, testOrderResult);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка создания нового TakeProfit ордера
        /// </summary>
        [Fact(DisplayName = "Create new take profit order Test")]
        public async Task CreateNewTakeProfitOrderAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT"; // если изменено нужно поменять в методе мока запроса
            var sideType = OrderSideType.Buy;
            var orderType = "TakeProfit";
            var priceAndQuantity = 0.1;
            var recvWindow = 6000;
            var builder = new Builder();
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetQuantity(priceAndQuantity);
            builder.SetStopPrice(priceAndQuantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var (binanceResponse, expectedResult) = MockNewOrderCreating(query, priceAndQuantity, OrderType.TakeProfit, orderType);
            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act #1
            var result = await _binanceExchange.CreateNewTakeProfitOrderAsync(symbol, sideType, priceAndQuantity, priceAndQuantity, recvWindow, isTest: false);
            TestHelper.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            builder.SetQuantity(priceAndQuantity); // обновляю значение кол-ва
            query = builder.GetResult().GetQuery(); // обновляю словарь параметров запроса
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _binanceExchange.CreateNewTakeProfitOrderAsync(symbol, sideType, priceAndQuantity, priceAndQuantity, recvWindow);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestHelper.CheckingAssertions(expectedResult, testOrderResult);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// TODO: тесты создания новых оредров Spot Account/Trade не проверяют работу строителя, а это практически единственное, что необходимо проверить
        /// поэтому дальше пойдут другие тесты, к этим есть смысл вернуться, если перегрузить сравнение словарей, чтобы NSubstitute при сравнении аргументов метода
        /// определял их равенство не по хэш кодам

        /// <summary>
        ///     Проверка отмены ордера
        /// </summary>
        [Fact(DisplayName = "Cancel order Test")]
        public async Task CancelOrderAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.CancelOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT";
            SetArgumentsEvent += SetArgumentsEventHandler;

            var priceAndQuantity = 0.1;
            var binanceResponse = TestHelper.CreateBinanceCancelOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                OrderStatusType.PartiallyFilled,
                TimeInForceType.IOC,
                OrderType.Limit,
                OrderSideType.Buy);
            _tradeSender.CancelOrderAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act
            var result = await _binanceExchange.CancelOrderAsync(symbol);
            var expectedResult = TestHelper.CreateExpectedCancelOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                "PartiallyFilled",
                "IOC",
                "Limit",
                OrderSideType.Buy);
            TestHelper.CheckingAssertions(expectedResult, result);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка отмены всех открытых ордера по паре
        /// </summary>
        [Fact(DisplayName = "Cancel all open orders Test")]
        public async Task CancelAllOrdersAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.CancelOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT";
            SetArgumentsEvent += SetArgumentsEventHandler;

            var priceAndQuantity = 0.1;
            var binanceResponse = TestHelper.CreateBinanceCancelOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                OrderStatusType.PartiallyFilled,
                TimeInForceType.IOC,
                OrderType.Limit,
                OrderSideType.Buy);
            _tradeSender.CancelAllOrdersAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(
                new List<BinanceExchange.Models.CancelOrderResponseModel> { binanceResponse, binanceResponse });

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act
            var result = (await _binanceExchange.CancelAllOrdersAsync(symbol)).ToList();
            var expectedModel = TestHelper.CreateExpectedCancelOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                "PartiallyFilled",
                "IOC",
                "Limit",
                OrderSideType.Buy);
            var expectedResult = new List<Common.Models.CancelOrderResponseModel> { expectedModel, expectedModel };
            Assert.Equal(expectedResult.Count, result.Count);
            for (var i = 0; i < expectedResult.Count; i++)
            {
                TestHelper.CheckingAssertions(expectedResult[i], result[i]);
            }

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка получения состояния ордера по паре
        /// </summary>
        [Fact(DisplayName = "Check order Test")]
        public async Task CheckOpenOrdersAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.CheckOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT";
            SetArgumentsEvent += SetArgumentsEventHandler;

            var priceAndQuantity = 0.1;
            var binanceResponse = TestHelper.CreateBinanceCheckOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                OrderStatusType.PartiallyFilled,
                TimeInForceType.IOC,
                OrderType.Limit,
                OrderSideType.Buy);
            _tradeSender.CheckOrderAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act
            var result = await _binanceExchange.CheckOrderAsync(symbol);
            var expectedResult = TestHelper.CreateExpectedCheckOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                "PartiallyFilled",
                "IOC",
                "Limit",
                OrderSideType.Buy);

            TestHelper.CheckingAssertions(expectedResult, result);


            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка получения состояния всех открытых ордеров по паре
        /// </summary>
        [Fact(DisplayName = "Check all open orders Test")]
        public async Task CheckAllOpenOrdersAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.CheckAllOpenOrdersWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT";
            SetArgumentsEvent += SetArgumentsEventHandler;

            var priceAndQuantity = 0.1;
            var binanceResponse = TestHelper.CreateBinanceCheckOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                OrderStatusType.PartiallyFilled,
                TimeInForceType.IOC,
                OrderType.Limit,
                OrderSideType.Buy);
            _tradeSender.CheckAllOpenOrdersAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(
                new List<BinanceExchange.Models.CheckOrderResponseModel> { binanceResponse, binanceResponse });

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act
            var result = (await _binanceExchange.CheckAllOpenOrdersAsync(symbol)).ToList();
            var expectedModel = TestHelper.CreateExpectedCheckOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                "PartiallyFilled",
                "IOC",
                "Limit",
                OrderSideType.Buy);
            var expectedResult = new List<Common.Models.CheckOrderResponseModel> { expectedModel, expectedModel };
            Assert.Equal(expectedResult.Count, result.Count);
            for (var i = 0; i < expectedResult.Count; i++)
            {
                TestHelper.CheckingAssertions(expectedResult[i], result[i]);
            }

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка получения всех открытых ордеров по паре
        /// </summary>
        [Fact(DisplayName = "Get all open orders Test")]
        public async Task GetAllOrdersAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.GetAllOrdersWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            var symbol = "BTCUSDT";
            SetArgumentsEvent += SetArgumentsEventHandler;

            var priceAndQuantity = 0.1;
            var binanceResponse = TestHelper.CreateBinanceCheckOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                OrderStatusType.PartiallyFilled,
                TimeInForceType.IOC,
                OrderType.Limit,
                OrderSideType.Buy);
            _tradeSender.GetAllOrdersAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(
                new List<BinanceExchange.Models.CheckOrderResponseModel> { binanceResponse, binanceResponse });

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act
            var result = (await _binanceExchange.GetAllOrdersAsync(symbol)).ToList();
            var expectedModel = TestHelper.CreateExpectedCheckOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                "PartiallyFilled",
                "IOC",
                "Limit",
                OrderSideType.Buy);
            var expectedResult = new List<Common.Models.CheckOrderResponseModel> { expectedModel, expectedModel };
            Assert.Equal(expectedResult.Count, result.Count);
            for (var i = 0; i < expectedResult.Count; i++)
            {
                TestHelper.CheckingAssertions(expectedResult[i], result[i]);
            }

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// <summary>
        ///     Проверка получения информации об аккаунте
        /// </summary>
        [Fact(DisplayName = "Get account information Test")]
        public async Task GetAccountInformationAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.AccountInformationWeight;
            var (expectedKey, expectedInterval, expectedWeight) = GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = await _binanceExchange.GetAccountInformationAsync(CancellationToken.None);
            var expectedResult = TestHelper.GetExpectedAccountInformationModel();

            TestHelper.CheckingAssertions(expectedResult, result);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Создает мок Redis базы данных
        /// </summary>
        private (IDatabase database, RedisDatabase redisDatabase) CreateMockRedisDatabase()
        {
            var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
            var database = Substitute.For<IDatabase>();
            connectionMultiplexer.GetDatabase().Returns(database);
            var redisDatabase = new RedisDatabase(connectionMultiplexer);

            return (database, redisDatabase);
        }

        /// <summary>
        ///     Возвращает ожидаемые параметры в методе увеличения совокупного веса запросов
        /// </summary>
        private (string expectedKey, TimeSpan expectedInterval, int expectedWeight) GetExpectedArguments(RequestWeightModel model, string weightKey)
        {
            var rateLimit = (_binanceExchange as BinanceExchange.BinanceExchange).GetRateLimit(model.Type);
            var expectedKey = (_binanceExchange as BinanceExchange.BinanceExchange).GetRedisKey(rateLimit.Type);
            var expectedInterval = rateLimit.Interval;
            var expectedWeight = model.Weights[weightKey];

            return (expectedKey, expectedInterval, expectedWeight);
        }

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

        /// <summary>
        ///     Создает мок на вызов метода создания любого типа ордера
        /// </summary>
        private (BinanceExchange.Models.FullOrderResponseModel binanceResponse, Common.Models.FullOrderResponseModel expectedResult) MockNewOrderCreating(
            Dictionary<string, object> query,
            double priceAndQuantity,
            OrderType orderType,
            string expectedOrderType)
        {
            var binanceResponse = TestHelper.CreateBinanceFullOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                OrderStatusType.PartiallyFilled,
                TimeInForceType.IOC,
                orderType,
                OrderSideType.Buy);
            _tradeSender.SendNewOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            var expectedResult = TestHelper.CreateExpectedFullOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                "PartiallyFilled",
                "IOC",
                expectedOrderType,
                OrderSideType.Buy);

            return (binanceResponse, expectedResult);
        }

        #endregion
    }
}
