using BinanceExchange;
using BinanceExchange.Client;
using BinanceExchange.Client.Impl;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinanceExchangeTests.BinanceTests.EndpointSendersTests
{
    /// <summary>
    ///     Класс, тестирующий <see cref="WalletSender"/>
    /// </summary>
    public class WalletSenderTest
    {
        #region Tests

        /// <summary>
        ///     Тест запроса статуса системы
        /// </summary>
        [Fact(DisplayName = "Requesting system status Test")]
        internal async Task<SystemStatusModel> GetSystemStatusAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Wallet/SYSTEM_STATUS.json";
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.SYSTEM_STATUS, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = await walletEndpointSender.GetSystemStatusAsync(CancellationToken.None);

            Assert.Equal(0, result.Status);
            Assert.Equal("normal", result.Message);

            return result;
        }

        /// <summary>
        ///     Тест запроса информации обо всех монетах
        /// </summary>
        [Fact(DisplayName = "Requesting information about all coins Test")]
        internal async Task<IEnumerable<CoinModel>> GetAllCoinsInformationAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Wallet/ALL_COINS_INFORMATION.json";
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.ALL_COINS_INFORMATION, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("testApiKey", "testSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = (await walletEndpointSender.GetAllCoinsInformationAsync(new(), CancellationToken.None)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Bitcoin", result[0].Name);
            Assert.Equal("MyCoin", result[1].Name);
            Assert.Equal("BTC", result[0].Coin);
            Assert.Equal("MyCoin", result[1].Coin);

            return result;
        }

        /// <summary>
        ///     Тест запроса статуса аккаунта
        /// </summary>
        [Fact(DisplayName = "The request of status of an account request Test")]
        internal async Task<AccountTradingStatusModel> GetAccountTradingStatusAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Wallet/ACCOUNT_API_TRADING_STATUS.json";
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.ACCOUNT_API_TRADING_STATUS, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("testApiKey", "testSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = await walletEndpointSender.GetAccountTradingStatusAsync(new(), CancellationToken.None);

            Assert.True(result.Data.IsLocked);
            Assert.Equal(123, result.Data.PlannedRecoverTimeUnix);
            Assert.Equal(150, result.Data.TriggerCondition.GCR);
            Assert.Equal(152, result.Data.TriggerCondition.IFER);
            Assert.Equal(302, result.Data.TriggerCondition.UFR);
            Assert.Equal(1547630471725, result.Data.UpdateTimeUnix);

            return result;
        }

        /// <summary>
        ///     Тест запроса таксы по всем монетам
        /// </summary>
        [Fact(DisplayName = "Requesting commission for all coins Test")]
        internal async Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Wallet/TRADE_FEE.json";
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.TRADE_FEE, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("testApiKey", "testSecretKey");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = (await walletEndpointSender.GetTradeFeeAsync(new())).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("ADABNB", result[0].Coin);
            Assert.Equal(0.001, result[0].MakerCommission);
            Assert.Equal(0.002, result[0].TakerCommission);
            Assert.Equal("BNBBTC", result[1].Coin);
            Assert.Equal(0.003, result[1].MakerCommission);
            Assert.Equal(0.004, result[1].TakerCommission);

            return result;
        }

        #endregion
    }
}
