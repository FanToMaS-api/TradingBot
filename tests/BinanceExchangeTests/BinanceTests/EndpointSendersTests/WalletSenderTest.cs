using BinanceExchange;
using BinanceExchange.Client;
using BinanceExchange.Client.Impl;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinanceExchangeTests.BinanceTests.EndpointSendersTests
{
    /// <summary>
    ///     �����, ����������� <see cref="WalletSender"/>
    /// </summary>
    public class WalletSenderTest
    {
        #region Tests

        /// <summary>
        ///     ���� ������� ������� �������
        /// </summary>
        [Fact(DisplayName = "Requesting system status Test")]
        internal async Task<SystemStatusModel> GetSystemStatusAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Wallet/SYSTEM_STATUS.json";
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.SYSTEM_STATUS, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = await walletEndpointSender.GetSystemStatusAsync(CancellationToken.None);

            Assert.Equal(0, result.Status);
            Assert.Equal("normal", result.Message);

            return result;
        }

        /// <summary>
        ///     ���� ������� ���������� ��� ���� �������
        /// </summary>
        [Fact(DisplayName = "Requesting information about all coins Test")]
        public async Task GetAllCoinsInformationAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Wallet/ALL_COINS_INFORMATION.json";
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.ALL_COINS_INFORMATION, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "testApiKey", "testSecretKey");
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = (await walletEndpointSender.GetAllCoinsInformationAsync(null, CancellationToken.None))
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Bitcoin", result[0].Name);
            Assert.Equal("MyCoin", result[1].Name);
            Assert.Equal("BTC", result[0].Coin);
            Assert.Equal("MyCoin", result[1].Coin);
        }

        /// <summary>
        ///     ���� ������� ������� ��������
        /// </summary>
        [Fact(DisplayName = "The request of status of an account request Test")]
        public async Task GetAccountTraidingStatusAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Wallet/ACCOUNT_API_TRADING_STATUS.json";
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.ACCOUNT_API_TRADING_STATUS, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "testApiKey", "testSecretKey");
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = await walletEndpointSender.GetAccountTradingStatusAsync(null, CancellationToken.None);

            Assert.True(result.Data.IsLocked);
            Assert.Equal(123, result.Data.PlannedRecoverTimeUnix);
            Assert.Equal(150, result.Data.TriggerCondition.GCR);
            Assert.Equal(152, result.Data.TriggerCondition.IFER);
            Assert.Equal(302, result.Data.TriggerCondition.UFR);
            Assert.Equal(1547630471725, result.Data.UpdateTimeUnix);
        }

        /// <summary>
        ///     ���� ������� ����� �� ���� �������
        /// </summary>
        [Fact(DisplayName = "Requesting commission for all coins Test")]
        public async Task GetTradeFeeAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Wallet/TRADE_FEE.json";
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.TRADE_FEE, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "testApiKey", "testSecretKey");
            IWalletSender walletEndpointSender = new WalletSender(binanceClient);

            // Act
            var result = (await walletEndpointSender.GetTradeFeeAsync(null)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("ADABNB", result[0].Coin);
            Assert.Equal(0.001, result[0].MakerCommission);
            Assert.Equal(0.002, result[0].TakerCommission);
            Assert.Equal("BNBBTC", result[1].Coin);
            Assert.Equal(0.003, result[1].MakerCommission);
            Assert.Equal(0.004, result[1].TakerCommission);
        }

        #endregion
    }
}