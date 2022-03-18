using AutoMapper;
using BinanceExchange;
using BinanceExchange.EndpointSenders;
using BinanceExchange.Models;
using BinanceExchangeTests.BinanceTests.EndpointSendersTests;
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
        private readonly IMapper _mapper;
        private readonly RequestsWeightStorage _requestsWeightStorage = new();
        private readonly IExchange _binanceExchange;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IWalletSender _walletSender;

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
            _redisDatabase = Substitute.For<IRedisDatabase>();

            _binanceExchange = SetupBinanceExchange();
        }

        /// <summary>
        ///     Настраивает отдельные компоненты биржи, для возврата нужных значений
        /// </summary>
        private BinanceExchange.BinanceExchange SetupBinanceExchange()
        {
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

            return new BinanceExchange.BinanceExchange(_walletSender, null, null, _redisDatabase, _mapper);
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

        #endregion
    }
}
