using ExchangeLibrary.Binance;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Client.Impl;
using ExchangeLibrary.Binance.EndpointSenders;
using ExchangeLibrary.Binance.EndpointSenders.Impl;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeLibraryTests.BinanceTests.EndpointSenders
{
    /// <summary>
    ///     �����, ����������� <see cref="MarketdataSender"/>
    /// </summary>
    public class MarketdataSenderTest
    {
        #region Public methods

        /// <summary>
        ///     ���� ������� ������ ������� ��� ���������� ������
        /// </summary>
        [Fact(DisplayName = "���� ������� ������ ������� ��� ���������� ������")]
        public async Task GetOrderBookAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\Marketdata\\ORDER_BOOK.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.ORDER_BOOK, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = await marketdataSender.GetOrderBookAsync("", cancellationToken: CancellationToken.None);

            Assert.Single(result.Bids);
            Assert.Single(result.Asks);
            Assert.Equal(4.00000000, result.Bids[0].Price);
            Assert.Equal(431.00000000, result.Bids[0].Qty);
            Assert.Equal(4.00000200, result.Asks[0].Price);
            Assert.Equal(12.00000000, result.Asks[0].Qty);
        }

        /// <summary>
        ///     ���� ������� ������ �������� ������
        /// </summary>
        [Fact(DisplayName = "���� ������� ������ �������� ������")]
        public async Task GetRecentTradesAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\Marketdata\\RECENT_TRADES.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.RECENT_TRADES, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetRecentTradesAsync("", cancellationToken: CancellationToken.None)).ToList();

            Assert.Single(result);
            Assert.Equal(28457, result[0].Id);
            Assert.Equal(4.00000100, result[0].Price);
            Assert.Equal(12.00000000, result[0].Qty);
            Assert.Equal(48.000012, result[0].QuoteQty);
            Assert.Equal(1499865549590, result[0].TimeUnix);
            Assert.True(result[0].IsBuyerMaker);
            Assert.True(result[0].IsBestMatch);
        }

        /// <summary>
        ///     ���� ������� ������ ������������ ������
        /// </summary>
        [Fact(DisplayName = "���� ������� ������ ������������ ������")]
        public async Task GetOldTradesAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\Marketdata\\OLD_TRADES.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.OLD_TRADES, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetOldTradesAsync("", 1000, cancellationToken: CancellationToken.None)).ToList();

            Assert.Single(result);
            Assert.Equal(28457, result[0].Id);
            Assert.Equal(4.00000100, result[0].Price);
            Assert.Equal(12.00000000, result[0].Qty);
            Assert.Equal(48.000012, result[0].QuoteQty);
            Assert.Equal(1499865549590, result[0].TimeUnix);
            Assert.True(result[0].IsBuyerMaker);
            Assert.True(result[0].IsBestMatch);
        }

        /// <summary>
        ///     ���� ������� ������ ������ �� ������
        /// </summary>
        [Fact(DisplayName = "���� ������� ������ ������ �� ������")]
        public async Task GetCandleStickAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\Marketdata\\CANDLESTICK_DATA.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.CANDLESTICK_DATA, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetCandleStickAsync("", Common.Enums.CandleStickIntervalType.OneMinute, cancellationToken: CancellationToken.None)).ToList();

            Assert.Single(result);
            Assert.Equal(1499040000000, result[0].OpenTimeUnix);
            Assert.Equal(0.01634790, result[0].OpenPrice);
            Assert.Equal(0.80000000, result[0].MaxPrice);
            Assert.Equal(0.01575800, result[0].MinPrice);
            Assert.Equal(0.01577100, result[0].ClosePrice);
            Assert.Equal(148976.11427815, result[0].Volume);
            Assert.Equal(1499644799999, result[0].CloseTimeUnix);
            Assert.Equal(2434.19055334, result[0].QuoteAssetVolume);
            Assert.Equal(308, result[0].TradesNumber);
            Assert.Equal(1756.87402397, result[0].BasePurchaseVolume);
            Assert.Equal(28.46694368, result[0].QuotePurchaseVolume);
            Assert.Equal("17928899.62484339", result[0].Ignore);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     ������� ��� HttpClient
        /// </summary>
        /// <param name="filePath"> ���� � ����� � json-response </param>
        /// <returns></returns>
        private HttpClient CreateMockHttpClient(string url, string filePath)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, filePath);
            var json = File.ReadAllText(path);
            using var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url).Respond("application/json", json);

            return new HttpClient(mockHttp);
        }

        #endregion
    }
}
