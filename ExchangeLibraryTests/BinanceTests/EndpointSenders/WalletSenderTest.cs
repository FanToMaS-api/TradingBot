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
    ///     Класс, тестирующий <see cref="WalletSender"/>
    /// </summary>
    public class WalletSenderTest
    {
        #region Public methods

        /// <summary>
        ///     Тест запроса статуса системы
        /// </summary>
        [Fact(DisplayName = "Тест запроса статуса системы")]
        public async Task GetSystemStatusAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\Wallet\\SYSTEM_STATUS.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.SYSTEM_STATUS, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

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
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\Wallet\\ALL_COINS_INFORMATION.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.ALL_COINS_INFORMATION, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "testApiKey", "testSecretKey");
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = (await walletEndpointSender.GetAllCoinsInformationAsync(7000, CancellationToken.None))
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Bitcoin", result[0].Name);
            Assert.Equal("MyCoin", result[1].Name);
            Assert.Equal("BTC", result[0].Coin);
            Assert.Equal("MyCoin", result[1].Coin);
        }

        /// <summary>
        ///     Тест запроса статуса аккаунта
        /// </summary>
        [Fact(DisplayName = "Тест запроса статуса аккаунта")]
        public async Task GetAccountTraidingStatusAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\Wallet\\ACCOUNT_API_TRADING_STATUS.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.ACCOUNT_API_TRADING_STATUS, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "testApiKey", "testSecretKey");
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = await walletEndpointSender.GetAccountTraidingStatusAsync(7000, CancellationToken.None);

            Assert.True(result.Data.IsLocked);
            Assert.Equal(123, result.Data.PlannedRecoverTimeUnix);
            Assert.Equal(150, result.Data.TriggerCondition.GCR);
            Assert.Equal(152, result.Data.TriggerCondition.IFER);
            Assert.Equal(302, result.Data.TriggerCondition.UFR);
            Assert.Equal(1547630471725, result.Data.UpdateTimeUnix);
        }

        /// <summary>
        ///     Тест запроса таксы по всем монетам
        /// </summary>
        [Fact(DisplayName = "Тест запроса таксы по всем монетам")]
        public async Task GetTradeFeeAsyncTest()
        {
            var filePath = "..\\..\\..\\BinanceTests\\Jsons\\Wallet\\TRADE_FEE.json";
            using var client = CreateMockHttpClient(BinanceEndpoints.TRADE_FEE, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "testApiKey", "testSecretKey");
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = (await walletEndpointSender.GetTradeFeeAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("ADABNB", result[0].Coin);
            Assert.Equal(0.001, result[0].MakerCommission);
            Assert.Equal(0.002, result[0].TakerCommission);
            Assert.Equal("BNBBTC", result[1].Coin);
            Assert.Equal(0.003, result[1].MakerCommission);
            Assert.Equal(0.004, result[1].TakerCommission);
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
