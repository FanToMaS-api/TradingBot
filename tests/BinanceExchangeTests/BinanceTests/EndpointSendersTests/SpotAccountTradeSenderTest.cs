using BinanceExchange;
using BinanceExchange.Client;
using BinanceExchange.Client.Helpers;
using BinanceExchange.Client.Impl;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Enums;
using BinanceExchange.Models;
using Common.Enums;
using SharedForTest;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace BinanceExchangeTests.BinanceTests.EndpointSendersTests
{
    /// <summary>
    ///     Тестирует <see cref="SpotTradeSender"/>
    /// </summary>
    public class SpotAccountTradeSenderTest
    {
        #region Fields

        private readonly FullOrderResponseModel _expectedResponse = TestHelper.CreateBinanceFullOrderResponseModel(
            "BTCUSDT",
            0.00000001,
            10.00000000,
            OrderStatusType.Filled,
            TimeInForceType.GTC,
            OrderType.Market,
            OrderSideType.Sell);

        private readonly CheckOrderResponseModel _expectedCheckOrderResponse = TestHelper.CreateBinanceCheckOrderResponseModel(
            "LTCBTC",
            0.1,
            1.0,
            OrderStatusType.New,
            TimeInForceType.GTC,
            OrderType.Limit,
            OrderSideType.Buy);

        private readonly AccountInformationModel _expectedAccountInformationResponse = TestHelper.GetBinanceAccountInformationModel();

        #endregion

        #region Tests

        /// <summary>
        ///     Тест запроса создания нового тестового ордера
        /// </summary>
        [Fact(DisplayName = "Request to create a new test order Test")]
        internal async Task<FullOrderResponseModel> SendNewTestOrderAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/NewOrderResponse.json";
            var builder = new Builder()
                .SetSymbol("ARPABNB")
                .SetRecvWindow(5000)
                .SetCandlestickInterval("1m");
            var parameters = builder.GetResult().GetRequestParameters();
            var requestUrl = CreateSignUrl(BinanceEndpoints.NEW_TEST_ORDER, parameters, "apiSecretKey");

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(requestUrl, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "apiSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            ISpotTradeSender tradeSender = new SpotTradeSender(binanceClient);

            // Act
            var result = await tradeSender.SendNewTestOrderAsync(parameters, cancellationToken: CancellationToken.None);
            var properties = typeof(FullOrderResponseModel).GetProperties();
            for (var j = 0; j < properties.Length; j++)
            {
                if (properties[j].Name == nameof(FullOrderResponseModel.Fills))
                {
                    continue;
                }

                Assert.Equal(properties[j].GetValue(_expectedResponse), properties[j].GetValue(result));
            }

            for (var i = 0; i < _expectedResponse.Fills.Count; i++)
            {
                TestExtensions.CheckingAssertions(_expectedResponse.Fills[i], result.Fills[i]);
            }

            return result;
        }

        /// <summary>
        ///     Тест запроса создания нового ордера
        /// </summary>
        [Fact(DisplayName = "Request to create a new order Test")]
        internal async Task<FullOrderResponseModel> SendNewOrderAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/NewOrderResponse.json";
            var builder = new Builder();
            builder.SetSymbol("ARPABNB");
            builder.SetRecvWindow(5000);
            builder.SetCandlestickInterval("1m");
            var parameters = builder.GetResult().GetRequestParameters();
            var requestUrl = CreateSignUrl(BinanceEndpoints.NEW_ORDER, parameters, "apiSecretKey");

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(requestUrl, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "apiSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            ISpotTradeSender tradeSender = new SpotTradeSender(binanceClient);

            // Act
            var result = await tradeSender.SendNewOrderAsync(parameters, cancellationToken: CancellationToken.None);
            TestExtensions.CheckingAssertions(_expectedResponse, result);

            return result;
        }

        /// <summary>
        ///     Тест запроса отмены ордера по паре
        /// </summary>
        [Fact(DisplayName = "Request to cancel the order Test")]
        public async Task CancelOrderAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/CancelOrderResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            builder.SetOrderId(4);
            var parameters = builder.GetResult().GetRequestParameters();
            var requestUrl = CreateSignUrl(BinanceEndpoints.CANCEL_ORDER, parameters, "apiSecretKey");

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(requestUrl, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "apiSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            ISpotTradeSender tradeSender = new SpotTradeSender(binanceClient);

            // Act
            var result = await tradeSender.CancelOrderAsync(parameters, cancellationToken: CancellationToken.None);

            Assert.Equal("LTCBTC", result.Symbol);
            Assert.Equal("myOrder1", result.OrigClientOrderId);
            Assert.Equal(4, result.OrderId);
            Assert.Equal(-1, result.OrderListId);
            Assert.Equal("cancelMyOrder1", result.ClientOrderId);
            Assert.Equal(2.00000000, result.Price);
            Assert.Equal(1.00000000, result.OrigQty);
            Assert.Equal(0.00000000, result.ExecutedQty);
            Assert.Equal(0.00001000, result.CumulativeQuoteQty);
            Assert.Equal(OrderStatusType.Canceled, result.Status);
            Assert.Equal(TimeInForceType.GTC, result.TimeInForce);
            Assert.Equal(OrderType.Limit, result.OrderType);
            Assert.Equal(OrderSideType.Buy, result.OrderSide);
        }

        /// <summary>
        ///     Тест запроса отмены всех ордеров по паре
        /// </summary>
        [Fact(DisplayName = "Request to cancel all orders Test")]
        public async Task CancelAllOrdersAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/CancelAllOrdersResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            builder.SetOrderId(4);
            var parameters = builder.GetResult().GetRequestParameters();
            var requestUrl = CreateSignUrl(BinanceEndpoints.CANCEL_All_ORDERS, parameters, "apiSecretKey");

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(requestUrl, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "apiSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            ISpotTradeSender tradeSender = new SpotTradeSender(binanceClient);

            // Act
            var result = (await tradeSender.CancelAllOrdersAsync(parameters, cancellationToken: CancellationToken.None)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("BTCUSDT", result[0].Symbol);
            Assert.Equal("E6APeyTJvkMvLMYMqu1KQ4", result[0].OrigClientOrderId);
            Assert.Equal(11, result[0].OrderId);
            Assert.Equal(-1, result[0].OrderListId);
            Assert.Equal("pXLV6Hz6mprAcVYpVMTGgx", result[0].ClientOrderId);
            Assert.Equal(0.089853, result[0].Price);
            Assert.Equal(0.178622, result[0].OrigQty);
            Assert.Equal(0.00000000, result[0].ExecutedQty);
            Assert.Equal(0.010000, result[0].CumulativeQuoteQty);
            Assert.Equal(OrderStatusType.Canceled, result[0].Status);
            Assert.Equal(TimeInForceType.GTC, result[0].TimeInForce);
            Assert.Equal(OrderType.Limit, result[0].OrderType);
            Assert.Equal(OrderSideType.Buy, result[0].OrderSide);

            Assert.Equal("BTCUSDT", result[1].Symbol);
            Assert.Equal("A3EF2HCwxgZPFMrfwbgrhv", result[1].OrigClientOrderId);
            Assert.Equal(13, result[1].OrderId);
            Assert.Equal(-1, result[1].OrderListId);
            Assert.Equal("pXLV6Hz6mprAcVYpVMTGgx", result[1].ClientOrderId);
            Assert.Equal(0.090430, result[1].Price);
            Assert.Equal(0.178623, result[1].OrigQty);
            Assert.Equal(0.00000000, result[1].ExecutedQty);
            Assert.Equal(0.100000, result[1].CumulativeQuoteQty);
            Assert.Equal(OrderStatusType.Canceled, result[1].Status);
            Assert.Equal(TimeInForceType.GTC, result[1].TimeInForce);
            Assert.Equal(OrderType.Market, result[1].OrderType);
            Assert.Equal(OrderSideType.Buy, result[1].OrderSide);
        }

        /// <summary>
        ///     Тест запроса состояния ордера по паре
        /// </summary>
        [Fact(DisplayName = "Request to check the order Test")]
        public async Task CheckOrderAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/CheckOrderResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            builder.SetOrderId(1);
            var parameters = builder.GetResult().GetRequestParameters();
            var url = CreateSignUrl(BinanceEndpoints.CHECK_ORDER, parameters, "apiSecretKey");

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(url, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "apiSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            ISpotTradeSender tradeSender = new SpotTradeSender(binanceClient);

            // Act
            var result = await tradeSender.CheckOrderAsync(parameters, cancellationToken: CancellationToken.None);

            TestExtensions.CheckingAssertions(_expectedCheckOrderResponse, result);
        }

        /// <summary>
        ///     Тест запроса состояния всех открытых оредров (или по опред паре)
        /// </summary>
        [Fact(DisplayName = "Request to check all open orders Test")]
        public async Task CheckAllOpenOrdersAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/CheckAllOpenOrdersResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            var parameters = builder.GetResult().GetRequestParameters();
            var url = CreateSignUrl(BinanceEndpoints.CHECK_ALL_OPEN_ORDERS, parameters, "apiSecretKey");

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(url, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "apiSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            ISpotTradeSender tradeSender = new SpotTradeSender(binanceClient);

            // Act
            var result = (await tradeSender.CheckAllOpenOrdersAsync(parameters, cancellationToken: CancellationToken.None)).ToList();

            Assert.Equal(2, result.Count);
            for (var j = 0; j < 2; j++)
            {
                TestExtensions.CheckingAssertions(_expectedCheckOrderResponse, result[j]);

                // изменение данных тут, чтобы не плодить объекты
                _expectedCheckOrderResponse.ClientOrderId = "1";
                _expectedCheckOrderResponse.OrigQuoteOrderQty = 15;
                _expectedCheckOrderResponse.ExecutedQty = 10.2;
                _expectedCheckOrderResponse.CumulativeQuoteQty = 10.3;
                _expectedCheckOrderResponse.Symbol = "asd";
            }
        }

        /// <summary>
        ///     Тест запроса всех ордеров по паре
        /// </summary>
        [Fact(DisplayName = "Request to get all orders Test")]
        public async Task GetAllOrdersAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/GetAllOrdersResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            var parameters = builder.GetResult().GetRequestParameters();
            var url = CreateSignUrl(BinanceEndpoints.GET_ALL_ORDERS, parameters, "apiSecretKey");

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(url, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "apiSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            ISpotTradeSender tradeSender = new SpotTradeSender(binanceClient);

            // Act
            var result = (await tradeSender.GetAllOrdersAsync(parameters, cancellationToken: CancellationToken.None)).ToList();

            Assert.Equal(2, result.Count);
            for (var j = 0; j < 2; j++)
            {
                TestExtensions.CheckingAssertions(_expectedCheckOrderResponse, result[j]);

                // изменение данных тут, чтобы не плодить объекты
                _expectedCheckOrderResponse.ClientOrderId = "1";
                _expectedCheckOrderResponse.OrigQuoteOrderQty = 15;
                _expectedCheckOrderResponse.ExecutedQty = 10.2;
                _expectedCheckOrderResponse.CumulativeQuoteQty = 10.3;
                _expectedCheckOrderResponse.Symbol = "asd";
                _expectedCheckOrderResponse.Status = OrderStatusType.Filled;
            }
        }

        /// <summary>
        ///     Тест запроса информации по аккаунту
        /// </summary>
        [Fact(DisplayName = "Request to get account information Test")]
        internal async Task<AccountInformationModel> GetAccountInformationAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/GetAccountInformation.json";
            var parameters = new Builder().GetResult().GetRequestParameters();
            var url = CreateSignUrl(BinanceEndpoints.ACCOUNT_INFORMATION, parameters, "apiSecretKey");

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(url, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "apiSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            ISpotTradeSender tradeSender = new SpotTradeSender(binanceClient);

            // Act
            var result = await tradeSender.GetAccountInformationAsync(parameters, cancellationToken: CancellationToken.None);

            TestExtensions.CheckingAssertions(_expectedAccountInformationResponse, result);

            return result;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Создает подписанный Url-запрос
        /// </summary>
        private string CreateSignUrl(string endPoint, Dictionary<string, string> parameters, string apiSecretKey)
        {
            var request = GetRequestUrl($"https://api.binance.com{endPoint}", parameters);
            var signature = BinanceUrlHelper.Sign(request, apiSecretKey);
            request += $"&signature={signature}";

            return request;
        }

        /// <summary>
        ///     Возвращает полный урл запроса со всеми параметрами
        /// </summary>
        /// <param name="requestUri"> Адрес запроса (с инлайн параметрами) </param>
        /// <param name="customParameters"> Параметры запроса </param>
        private string GetRequestUrl(string requestUri, IDictionary<string, string> customParameters)
            => string.Format(
                "{0}?{1}",
                requestUri,
                GetCustomParametersString(customParameters));

        /// <summary>
        ///     Получить строковое представление тела запроса <br />
        ///     Кодирует параметры запроса
        /// </summary>
        private static string GetCustomParametersString(IDictionary<string, string> body)
            => body
                .Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value)))
                .JoinToString("&");

        #endregion
    }
}
