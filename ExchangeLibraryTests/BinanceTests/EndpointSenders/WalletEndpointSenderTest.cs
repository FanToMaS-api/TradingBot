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
    ///     Класс, тестирующий <see cref="WalletEndpointSender"/>
    /// </summary>
    public class WalletEndpointSenderTest
    {
        #region Public methods

        /// <summary>
        ///     Тест запроса статуса системы
        /// </summary>
        [Fact(DisplayName = "Тест запроса статуса системы")]
        public async Task GetSystemStatusAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\SYSTEM_STATUS.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.SYSTEM_STATUS, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IWalletEndpointSender walletEndpointSender = new WalletEndpointSender(binanceClient);

            // Act
            var result = await walletEndpointSender.GetSystemStatusAsync(CancellationToken.None);

            Assert.Equal(123, result.Status);
            Assert.Equal("test", result.Message);
        }

        /// <summary>
        ///     Тест запроса информации обо всех монетах
        /// </summary>
        [Fact(DisplayName = "Тест запроса информации обо всех монетах")]
        public async Task GetAllCoinsInformationAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\ALL_COINS_INFORMATION.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.ALL_COINS_INFORMATION, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "testApiKey", "testSecretKey");
            IWalletEndpointSender walletEndpointSender = new WalletEndpointSender(binanceClient);

            // Act
            var result = (await walletEndpointSender.GetAllCoinsInformationAsync(7000, CancellationToken.None))
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Bitcoin", result[0].Name);
            Assert.Equal("MyCoin", result[1].Name);
            Assert.Equal("BTC", result[0].Coin);
            Assert.Equal("MyCoin", result[1].Coin);
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
