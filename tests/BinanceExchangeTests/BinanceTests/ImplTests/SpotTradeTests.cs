using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BinanceExchange;
using BinanceExchange.EndpointSenders;
using BinanceExchange.Enums;
using BinanceExchange.Impl;
using BinanceExchange.Models;
using BinanceExchange.RequestWeights;
using BinanceExchangeTests.BinanceTests.EndpointSendersTests;
using Common.Enums;
using ExchangeLibrary;
using NSubstitute;
using Redis;
using SharedForTest;
using Xunit;

namespace BinanceExchangeTests.BinanceTests.ImplTests
{
    /// <summary>
    ///     Класс тестирующий <see cref="SpotTrade"/>
    /// </summary>
    public class SpotTradeTests
    {
        #region Fields

        private readonly SpotAccountTradeSenderTest _spotAccountTradeSenderTest = new();
        private readonly IMapper _mapper;
        private readonly SpotTradeWeightStorage _requestsWeightStorage = new();
        private readonly ISpotTrade _spotTrade;
        private readonly IRedisDatabase _redisDatabase;
        private readonly ISpotTradeSender _tradeSender;

        // поля для проверки верного увеличения лимитов ограничения скорости
        private string _actualKey = "";
        private TimeSpan _actualInterval = TimeSpan.FromSeconds(0);
        private int _actualWeight = 0;

        #endregion

        #region .ctor

        /// <inheritdoc cref="SpotTradeTests"/>
        public SpotTradeTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BinanceMapperProfile>();
            });
            _mapper = new Mapper(config);
            _tradeSender = Substitute.For<ISpotTradeSender>();
            _redisDatabase = Substitute.For<IRedisDatabase>();

            _spotTrade = SetupSpotTrade();
        }

        /// <summary>
        ///     Настраивает отдельные компоненты биржи, для возврата нужных значений
        /// </summary>
        private ISpotTrade SetupSpotTrade()
        {
            _tradeSender.GetAccountInformationAsync(Arg.Any<Dictionary<string, object>>(), Arg.Any<CancellationToken>()).Returns(async _ =>
            {
                return await _spotAccountTradeSenderTest.GetAccountInformationAsync_Test();
            });

            return new SpotTrade(_tradeSender, _redisDatabase, _mapper);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Событие для проверки актуальных аргументов
        /// </summary>
        public Action<string, TimeSpan, int> SetArgumentsEvent { get; set; }

        #endregion


        #region Spot Account/Trade Tests (Данные тесты не проверяю результат работы строителя query)

        /// <summary>
        ///     Проверка создания нового лимитного ордера
        /// </summary>
        [Fact(DisplayName = "Create new limit order Test")]
        public async Task CreateNewLimitOrderAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
            var result = await _spotTrade.CreateNewLimitOrderAsync(symbol, orderSide, forceType, priceAndQuantity, priceAndQuantity, isTest: false);
            TestExtensions.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            builder.SetPrice(priceAndQuantity);
            query = builder.GetResult(false).GetQuery();
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _spotTrade.CreateNewLimitOrderAsync(symbol, orderSide, forceType, priceAndQuantity, priceAndQuantity);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestExtensions.CheckingAssertions(expectedResult, testOrderResult);

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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
            var result = await _spotTrade.CreateNewMarketOrderAsync(symbol, orderSide, priceAndQuantity, isTest: false);
            TestExtensions.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            builder.SetQuantity(priceAndQuantity); // обновляю значение кол-ва
            query = builder.GetResult().GetQuery(); // обновляю словарь параметров запроса
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _spotTrade.CreateNewMarketOrderAsync(symbol, orderSide, priceAndQuantity);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestExtensions.CheckingAssertions(expectedResult, testOrderResult);

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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
            var result = await _spotTrade.CreateNewStopLossOrderAsync(symbol, sideType, priceAndQuantity, priceAndQuantity, recvWindow, isTest: false);
            TestExtensions.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            builder.SetQuantity(priceAndQuantity); // обновляю значение кол-ва
            query = builder.GetResult().GetQuery(); // обновляю словарь параметров запроса
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _spotTrade.CreateNewStopLossOrderAsync(symbol, sideType, priceAndQuantity, priceAndQuantity, recvWindow);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestExtensions.CheckingAssertions(expectedResult, testOrderResult);

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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
            var result = await _spotTrade.CreateNewStopLossLimitOrderAsync(symbol, sideType, forceType, priceAndQuantity, priceAndQuantity, recvWindow, isTest: false);
            TestExtensions.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            builder.SetQuantity(priceAndQuantity); // обновляю значение кол-ва
            query = builder.GetResult().GetQuery(); // обновляю словарь параметров запроса
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _spotTrade.CreateNewStopLossLimitOrderAsync(symbol, sideType, forceType, priceAndQuantity, priceAndQuantity, recvWindow);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestExtensions.CheckingAssertions(expectedResult, testOrderResult);

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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
            var result = await _spotTrade.CreateNewTakeProfitOrderAsync(symbol, sideType, priceAndQuantity, priceAndQuantity, recvWindow, isTest: false);
            TestExtensions.CheckingAssertions(expectedResult, result);

            priceAndQuantity = 255;
            builder.SetQuantity(priceAndQuantity); // обновляю значение кол-ва
            query = builder.GetResult().GetQuery(); // обновляю словарь параметров запроса
            binanceResponse.Price = priceAndQuantity;
            binanceResponse.OrigQty = priceAndQuantity;
            _tradeSender.SendNewTestOrderAsync(query, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(binanceResponse);

            // Act #2
            var testOrderResult = await _spotTrade.CreateNewTakeProfitOrderAsync(symbol, sideType, priceAndQuantity, priceAndQuantity, recvWindow);
            expectedResult.Price = priceAndQuantity;
            expectedResult.OrigQty = priceAndQuantity;
            TestExtensions.CheckingAssertions(expectedResult, testOrderResult);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

            SetArgumentsEvent -= SetArgumentsEventHandler;
        }

        /// TODO: тесты создания новых оредров Spot Account/Trade не проверяют работу строителя,
        /// а это практически единственное, что необходимо проверить
        /// поэтому дальше пойдут другие тесты. К этим есть смысл вернуться,
        /// если перегрузить сравнение словарей, чтобы NSubstitute при сравнении аргументов метода
        /// определял их равенство не по хэш кодам

        /// <summary>
        ///     Проверка отмены ордера
        /// </summary>
        [Fact(DisplayName = "Cancel order Test")]
        public async Task CancelOrderAsync_Test()
        {
            var requestWeight = _requestsWeightStorage.CancelOrderWeight;
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
            var result = await _spotTrade.CancelOrderAsync(symbol);
            var expectedResult = TestHelper.CreateExpectedCancelOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                "PartiallyFilled",
                "IOC",
                "Limit",
                OrderSideType.Buy);
            TestExtensions.CheckingAssertions(expectedResult, result);

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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
                new List<CancelOrderResponseModel> { binanceResponse, binanceResponse });

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act
            var result = (await _spotTrade.CancelAllOrdersAsync(symbol)).ToList();
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
                TestExtensions.CheckingAssertions(expectedResult[i], result[i]);
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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
            var result = await _spotTrade.CheckOrderAsync(symbol);
            var expectedResult = TestHelper.CreateExpectedCheckOrderResponseModel(
                "BTCUSDT",
                priceAndQuantity,
                priceAndQuantity,
                "PartiallyFilled",
                "IOC",
                "Limit",
                OrderSideType.Buy);

            TestExtensions.CheckingAssertions(expectedResult, result);


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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
                new List<CheckOrderResponseModel> { binanceResponse, binanceResponse });

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act
            var result = (await _spotTrade.CheckAllOpenOrdersAsync(symbol)).ToList();
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
                TestExtensions.CheckingAssertions(expectedResult[i], result[i]);
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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
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
                new List<CheckOrderResponseModel> { binanceResponse, binanceResponse });

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            // Act
            var result = (await _spotTrade.GetAllOrdersAsync(symbol)).ToList();
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
                TestExtensions.CheckingAssertions(expectedResult[i], result[i]);
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
            var (expectedKey, expectedInterval, expectedWeight) = TestHelper.GetExpectedArguments(requestWeight, RequestWeightModel.GetDefaultKey());
            SetArgumentsEvent += SetArgumentsEventHandler;

            MockRedisIncrementOrCreateKeyValue(_redisDatabase);

            var result = await _spotTrade.GetAccountInformationAsync(CancellationToken.None);
            var expectedResult = TestHelper.GetExpectedAccountInformationModel();

            TestExtensions.CheckingAssertions(expectedResult, result);

            Assert.Equal(expectedKey, _actualKey);
            Assert.Equal(expectedInterval, _actualInterval);
            Assert.Equal(expectedWeight, _actualWeight);

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

        /// <summary>
        ///     Создает мок на вызов метода создания любого типа ордера
        /// </summary>
        private (FullOrderResponseModel binanceResponse, Common.Models.FullOrderResponseModel expectedResult) MockNewOrderCreating(
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
