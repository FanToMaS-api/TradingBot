using System;
using System.Collections.Generic;
using System.Linq;
using BinanceExchange.Impl;
using System.Threading.Tasks;
using BinanceExchange.RequestWeights;
using ExchangeLibrary;
using AutoMapper;
using NSubstitute;
using Redis;
using BinanceExchange.EndpointSenders;
using BinanceExchange;
using BinanceExchangeTests.BinanceTests.EndpointSendersTests;
using System.Threading;
using Xunit;
using BinanceExchange.Models;

namespace BinanceExchangeTests.BinanceTests.ImplTests
{
    /// <summary>
    ///     Класс тестирующий <see cref="Wallet"/>
    /// </summary>
    public class WalletTests
    {
        #region Fields

        private readonly WalletSenderTest _walletSenderTest = new();
        private readonly IMapper _mapper;
        private readonly WalletRequestWeightStorage _weightStorage = new();
        private readonly IWallet _wallet;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IWalletSender _walletSender;

        // поля для проверки верного увеличения лимитов ограничения скорости
        private string _actualKey = "";
        private TimeSpan _actualInterval = TimeSpan.FromSeconds(0);
        private int _actualWeight = 0;

        #endregion
        #region .ctor

        /// <inheritdoc cref="WalletTests"/>
        public WalletTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BinanceMapperProfile>();
            });
            _mapper = new Mapper(config);
            _walletSender = Substitute.For<IWalletSender>();
            _redisDatabase = Substitute.For<IRedisDatabase>();

            _wallet = SetupWallet();
        }

        /// <summary>
        ///     Настраивает отдельные компоненты биржи, для возврата нужных значений
        /// </summary>
        private IWallet SetupWallet()
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

            return new Wallet(_walletSender, _redisDatabase, _mapper);
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
        ///     Проверка получения статуса системы
        /// </summary>
        [Fact(DisplayName = "Get system status Test")]
        public async Task GetSystemStatusAsync_Test()
        {
            // код проверки верного увеличения лимитов
            var requestWeight = _weightStorage.SystemStatusWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            Assert.True(await _wallet.GetSystemStatusAsync());

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
            var requestWeight = _weightStorage.AccountStatusWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = await _wallet.GetAccountTradingStatusAsync();

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
            var requestWeight = _weightStorage.AllCoinsInfoWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _wallet.GetAllTradeObjectInformationAsync()).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(2, result.Count);
            Assert.Equal("Bitcoin", result[0].Name);
            Assert.Equal("MyCoin", result[1].Name);
            Assert.Equal("BTC", result[0].ShortName);
            Assert.Equal("MyCoin", result[1].ShortName);
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
            var requestWeight = _weightStorage.TradeFeeWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());

            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = (await _wallet.GetTradeFeeAsync()).ToList();

            SetArgumentsEvent -= SetArgumentsEventHandler;

            Assert.Equal(2, result.Count);
            Assert.Equal("ADABNB", result[0].ShortName);
            Assert.Equal(0.001, result[0].MakerCommission);
            Assert.Equal(0.002, result[0].TakerCommission);
            Assert.Equal("BNBBTC", result[1].ShortName);
            Assert.Equal(0.003, result[1].MakerCommission);
            Assert.Equal(0.004, result[1].TakerCommission);
            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);
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
