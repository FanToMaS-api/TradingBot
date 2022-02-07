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
    ///     Класс, тестирующий <see cref="MarketdataSender"/>
    /// </summary>
    public class MarketdataSenderTest
    {
        #region Public methods

        /// <summary>
        ///     Тест запроса списка ордеров для конкретной монеты
        /// </summary>
        [Fact(DisplayName = "Тест запроса списка ордеров для конкретной монеты")]
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
            Assert.Equal(2, result.Bids[0].Count);
            Assert.Equal(2, result.Asks[0].Count);
            Assert.Equal(4.00000000, result.Bids[0].First());
            Assert.Equal(431.00000000, result.Bids[0].Last());
            Assert.Equal(4.00000200, result.Asks[0].First());
            Assert.Equal(12.00000000, result.Asks[0].Last());
        }

        /// <summary>
        ///     Тест запроса списка недавних сделок
        /// </summary>
        [Fact(DisplayName = "Тест запроса списка недавних сделок")]
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

        #endregion

        #region Private methods

        /// <summary>
        ///     Создает мок HttpClient
        /// </summary>
        /// <param name="filePath"> Путь к файлу с json-response </param>
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
