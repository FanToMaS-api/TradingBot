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

        private FullOrderResponseModel _expectedResponse = new FullOrderResponseModel
        {
            Symbol = "BTCUSDT",
            OrderId = 28,
            ClientOrderId = "6gCrw2kRUAF9CvJDGP16IP",
            TransactTimeUnix = 1507725176595,
            OrderListId = -1,
            Price = 0.00000001,
            OrigQty = 10.00000000,
            ExecutedQty = 10.1,
            OrigClientOrderId = null,
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

        #endregion

        #region Tests

        /// <summary>
        ///     Тест запроса создания нового тестового ордера
        /// </summary>
        [Fact(DisplayName = "Request to create a new test order Test")]
        public async Task SendNewTestOrderAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/SpotAccountTrade/NewOrderResponse.json";
            var urlSb = new StringBuilder();
            var query = new Dictionary<string, object>
            {
                { "recvWindow", 5000 },
                { "timeStamp", 5789 },
                { "symbol", "ARPABNB" },
                { "t", "1m" },
            };

            BinanceUrlHelper.BuildQueryString(query, urlSb);
            var signature = BinanceUrlHelper.Sign(urlSb.ToString(), "");
            urlSb.Append($"&signature={signature}");
            var requestUrl = $"https://api.binance.com{BinanceEndpoints.NEW_TEST_ORDER}?{urlSb}";

            using var client = TestHelper.CreateMockHttpClient(requestUrl, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
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
            var urlSb = new StringBuilder();
            var query = new Dictionary<string, object>
            {
                { "recvWindow", 5000 },
                { "timeStamp", 5789 },
                { "symbol", "ARPABNB" },
                { "t", "1m" },
            };

            BinanceUrlHelper.BuildQueryString(query, urlSb);
            var signature = BinanceUrlHelper.Sign(urlSb.ToString(), "apiSecretKey");
            urlSb.Append($"&signature={signature}");
            var requestUrl = $"https://api.binance.com{BinanceEndpoints.NEW_ORDER}?{urlSb}";

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
            var urlSb = new StringBuilder();
            var query = new Dictionary<string, object>
            {
                { "recvWindow", 5000 },
                { "timeStamp", 5789 },
                { "symbol", "LTCBTC" },
                { "orderId", "4" },
            };

            BinanceUrlHelper.BuildQueryString(query, urlSb);
            var signature = BinanceUrlHelper.Sign(urlSb.ToString(), "apiSecretKey");
            urlSb.Append($"&signature={signature}");
            var requestUrl = $"https://api.binance.com{BinanceEndpoints.CANCEL_ORDER}?{urlSb}";

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
            var urlSb = new StringBuilder();
            var query = new Dictionary<string, object>
            {
                { "recvWindow", 5000 },
                { "timeStamp", 5789 },
                { "symbol", "LTCBTC" },
                { "orderId", "4" },
            };

            BinanceUrlHelper.BuildQueryString(query, urlSb);
            var signature = BinanceUrlHelper.Sign(urlSb.ToString(), "apiSecretKey");
            urlSb.Append($"&signature={signature}");
            var requestUrl = $"https://api.binance.com{BinanceEndpoints.CANCEL_All_ORDERS}?{urlSb}";

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

        #endregion
    }
}
