using ExchangeLibrary.Binance;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Client.Impl;
using ExchangeLibrary.Binance.EndpointSenders;
using ExchangeLibrary.Binance.EndpointSenders.Impl;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeLibraryTests.BinanceTests.EndpointSenders
{
    /// <summary>
    ///     Тестирует <see cref="SpotAccountTradeSender"/>
    /// </summary>
    public class SpotAccountTradeSenderTest
    {
        #region Fields

        private FullOrderResponseModel _expectedResponse = new()
        {
            Symbol = "BTCUSDT",
            OrderId = 28,
            ClientOrderId = "6gCrw2kRUAF9CvJDGP16IP",
            TransactTimeUnix = 1507725176595,
            OrderListId = -1,
            Price = 0.00000001,
            OrigQty = 10.00000000,
            ExecutedQty = 10.1,
            CumulativeQuoteQty = 10.2,
            Status = OrderStatusType.Filled,
            TimeInForce = TimeInForceType.GTC,
            OrderType = OrderType.Market,
            OrderSide = OrderSideType.Sell,
            Fills = new List<FillModel>
            {
                new FillModel
                {
                    Price = 4000.00000000,
                    Quantity = 1.00000000,
                    Commission = 4.00000000,
                    CommissionAsset = "USDT",
                    TradeId = 56
                },
                new FillModel
                {
                    Price = 3999.00000000,
                    Quantity = 5.00000000,
                    Commission = 19.99500000,
                    CommissionAsset = "USDT",
                    TradeId = 57
                },
                new FillModel
                {
                    Price = 3998.00000000,
                    Quantity = 2.00000000,
                    Commission = 7.99600000,
                    CommissionAsset = "USDT",
                    TradeId = 58
                },
                new FillModel
                {
                    Price = 3997.00000000,
                    Quantity = 1.00000000,
                    Commission = 3.99700000,
                    CommissionAsset = "USDT",
                    TradeId = 59
                },
                new FillModel
                {
                    Price = 3995.00000000,
                    Quantity = 1.00000000,
                    Commission = 3.99500000,
                    CommissionAsset = "USDT",
                    TradeId = 60
                },
            }
        };

        private CheckOrderResponseModel _expectedCheckOrderResponse = new()
        {
            Symbol = "LTCBTC",
            ClientOrderId = "myOrder1",
            OrderId = 1,
            OrderListId = -1,
            Price = 0.1,
            OrigQty = 1.0,
            ExecutedQty = 0.0,
            CumulativeQuoteQty = 0.1,
            Status = OrderStatusType.New,
            TimeInForce = TimeInForceType.GTC,
            OrderType = OrderType.Limit,
            OrderSide = OrderSideType.Buy,
            StopPrice = 0.001,
            IcebergQty = 0.002,
            TimeUnix = 1499827319559,
            UpdateTimeUnix = 1499827319559,
            IsWorking = true,
            OrigQuoteOrderQty = 0.000300,

        };

        #endregion

        #region Tests

        /// <summary>
        ///     Тест запроса создания нового тестового ордера
        /// </summary>
        [Fact(DisplayName = "Request to create a new test order Test")]
        public async Task SendNewTestOrderAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/NewOrderResponse.json";
            var builder = new Builder();
            builder.SetSymbol("ARPABNB");
            builder.SetRecvWindow(5000);
            builder.SetCandlestickInterval("1m");
            var query = builder.GetResult().GetQuery();
            var requestUrl = CreateSignUrl(BinanceEndpoints.NEW_TEST_ORDER, query, "apiSecretKey");

            using var client = TestHelper.CreateMockHttpClient(requestUrl, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "apiSecretKey");
            ISpotAccountTradeSender tradeSender = new SpotAccountTradeSender(binanceClient);

            // Act
            var result = await tradeSender.SendNewTestOrderAsync(query, cancellationToken: CancellationToken.None);

            var properties = typeof(FullOrderResponseModel).GetProperties();
            for (var j = 0; j < properties.Length; j++)
            {
                if (properties[j].Name == nameof(FullOrderResponseModel.Fills))
                {
                    continue;
                }

                Assert.Equal(properties[j].GetValue(_expectedResponse), properties[j].GetValue(result));
            }

            var fillModelProperties = typeof(FillModel).GetProperties();
            for (var i = 0; i < _expectedResponse.Fills.Count; i++)
            {
                for (var j = 0; j < fillModelProperties.Length; j++)
                {
                    Assert.Equal(
                        fillModelProperties[j].GetValue(_expectedResponse.Fills[i]),
                        fillModelProperties[j].GetValue(result.Fills[i]));
                }
            }
        }

        /// <summary>
        ///     Тест запроса создания нового ордера
        /// </summary>
        [Fact(DisplayName = "Request to create a new order Test")]
        public async Task SendNewOrderAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/NewOrderResponse.json";
            var builder = new Builder();
            builder.SetSymbol("ARPABNB");
            builder.SetRecvWindow(5000);
            builder.SetCandlestickInterval("1m");
            var query = builder.GetResult().GetQuery();
            var requestUrl = CreateSignUrl(BinanceEndpoints.NEW_ORDER, query, "apiSecretKey");

            using var client = TestHelper.CreateMockHttpClient(requestUrl, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "apiSecretKey");
            ISpotAccountTradeSender tradeSender = new SpotAccountTradeSender(binanceClient);

            // Act
            var result = await tradeSender.SendNewOrderAsync(query, cancellationToken: CancellationToken.None);

            var properties = typeof(FullOrderResponseModel).GetProperties();
            for (var j = 0; j < properties.Length; j++)
            {
                if (properties[j].Name == nameof(FullOrderResponseModel.Fills))
                {
                    continue;
                }

                Assert.Equal(properties[j].GetValue(_expectedResponse), properties[j].GetValue(result));
            }

            var fillModelProperties = typeof(FillModel).GetProperties();
            for (var i = 0; i < _expectedResponse.Fills.Count; i++)
            {
                for (var j = 0; j < fillModelProperties.Length; j++)
                {
                    Assert.Equal(
                        fillModelProperties[j].GetValue(_expectedResponse.Fills[i]),
                        fillModelProperties[j].GetValue(result.Fills[i]));
                }
            }
        }

        /// <summary>
        ///     Тест запроса отмены ордера по паре
        /// </summary>
        [Fact(DisplayName = "Request to cancel the order Test")]
        public async Task CancelOrderAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/CancelOrderResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            builder.SetOrderId(4);
            var query = builder.GetResult().GetQuery();
            var requestUrl = CreateSignUrl(BinanceEndpoints.CANCEL_ORDER, query, "apiSecretKey");

            using var client = TestHelper.CreateMockHttpClient(requestUrl, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "apiSecretKey");
            ISpotAccountTradeSender tradeSender = new SpotAccountTradeSender(binanceClient);

            // Act
            var result = await tradeSender.CancelOrderAsync(query, cancellationToken: CancellationToken.None);

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
        public async Task CancelAllOrdersAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/CancelAllOrdersResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            builder.SetOrderId(4);
            var query = builder.GetResult().GetQuery();
            var requestUrl = CreateSignUrl(BinanceEndpoints.CANCEL_All_ORDERS, query, "apiSecretKey");

            using var client = TestHelper.CreateMockHttpClient(requestUrl, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "apiSecretKey");
            ISpotAccountTradeSender tradeSender = new SpotAccountTradeSender(binanceClient);

            // Act
            var result = (await tradeSender.CancelAllOrdersAsync(query, cancellationToken: CancellationToken.None)).ToList();

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
        public async Task CheckOrderAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/CheckOrderResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            builder.SetOrderId(1);
            var query = builder.GetResult().GetQuery();
            var url = CreateSignUrl(BinanceEndpoints.CHECK_ORDER, query, "apiSecretKey");

            using var client = TestHelper.CreateMockHttpClient(url, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "apiSecretKey");
            ISpotAccountTradeSender tradeSender = new SpotAccountTradeSender(binanceClient);

            // Act
            var result = await tradeSender.CheckOrderAsync(query, cancellationToken: CancellationToken.None);

            var properties = typeof(CheckOrderResponseModel).GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                Assert.Equal(properties[i].GetValue(_expectedCheckOrderResponse), properties[i].GetValue(result));
            }
        }

        /// <summary>
        ///     Тест запроса состояния всех открытых оредров (или по опред паре)
        /// </summary>
        [Fact(DisplayName = "Request to check all open orders Test")]
        public async Task CheckAllOpenOrdersAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/CheckAllOpenOrdersResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            var query = builder.GetResult().GetQuery();
            var url = CreateSignUrl(BinanceEndpoints.CHECK_ALL_OPEN_ORDERS, query, "apiSecretKey");

            using var client = TestHelper.CreateMockHttpClient(url, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "apiSecretKey");
            ISpotAccountTradeSender tradeSender = new SpotAccountTradeSender(binanceClient);

            // Act
            var result = (await tradeSender.CheckAllOpenOrdersAsync(query, cancellationToken: CancellationToken.None)).ToList();

            var properties = typeof(CheckOrderResponseModel).GetProperties();
            Assert.Equal(2, result.Count);
            for (var j = 0; j < 2; j++)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    Assert.Equal(properties[i].GetValue(_expectedCheckOrderResponse), properties[i].GetValue(result[j]));
                }

                // изменение данных тут, чтобы не плодить объекты
                _expectedCheckOrderResponse.ClientOrderId = "1";
                _expectedCheckOrderResponse.OrigQuoteOrderQty = 15;
                _expectedCheckOrderResponse.Symbol = "asd";
            }
        }

        /// <summary>
        ///     Тест запроса всех ордеров по паре
        /// </summary>
        [Fact(DisplayName = "Request to get all orders Test")]
        public async Task GetAllOrdersAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/GetAllOrdersResponse.json";
            var builder = new Builder();
            builder.SetSymbol("LTCBTC");
            builder.SetRecvWindow(5000);
            var query = builder.GetResult().GetQuery();
            var url = CreateSignUrl(BinanceEndpoints.GET_ALL_ORDERS, query, "apiSecretKey");

            using var client = TestHelper.CreateMockHttpClient(url, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "apiSecretKey");
            ISpotAccountTradeSender tradeSender = new SpotAccountTradeSender(binanceClient);

            // Act
            var result = (await tradeSender.GetAllOrdersAsync(query, cancellationToken: CancellationToken.None)).ToList();

            var properties = typeof(CheckOrderResponseModel).GetProperties();
            Assert.Equal(2, result.Count);
            for (var j = 0; j < 2; j++)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    Assert.Equal(properties[i].GetValue(_expectedCheckOrderResponse), properties[i].GetValue(result[j]));
                }

                // изменение данных тут, чтобы не плодить объекты
                _expectedCheckOrderResponse.ClientOrderId = "1";
                _expectedCheckOrderResponse.OrigQuoteOrderQty = 15;
                _expectedCheckOrderResponse.Symbol = "asd";
                _expectedCheckOrderResponse.Status = OrderStatusType.Filled;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Создает подписанный Url-запрос
        /// </summary>
        private string CreateSignUrl(string endPoint, Dictionary<string, object> query, string apiSecretKey)
        {
            var urlSb = new StringBuilder();
            BinanceUrlHelper.BuildQueryString(query, urlSb);
            var signature = BinanceUrlHelper.Sign(urlSb.ToString(), apiSecretKey);
            urlSb.Append($"&signature={signature}");

            return $"https://api.binance.com{endPoint}?{urlSb}";
        }

        #endregion
    }
}
