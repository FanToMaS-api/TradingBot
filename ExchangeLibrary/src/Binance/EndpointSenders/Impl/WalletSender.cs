using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Models.Wallet;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders.Impl
{
    /// <summary>
    ///     Отвечает за отправку запросов к конечным точкам кошелька
    /// </summary>
    internal class WalletSender : IWalletSender
    {
        #region Fields

        private readonly IBinanceClient _client;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="WalletSender" />
        public WalletSender(IBinanceClient client)
        {
            _client = client;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<SystemStatusModel> GetSystemStatusAsync(CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                 BinanceEndpoints.SYSTEM_STATUS,
                 HttpMethod.Get,
                 cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<SystemStatusModel>(result);
        }

        /// <inheritdoc />
        public async Task<AccountTraidingStatusModel> GetAccountTraidingStatusAsync(
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.ACCOUNT_API_TRADING_STATUS,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "recvWindow", recvWindow },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                },
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<AccountTraidingStatusModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync(
            string symbol = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(symbol))
            {
                parameters["symbol"] = symbol;
            }

            parameters["recvWindow"] = recvWindow;
            parameters["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.TRADE_FEE,
                HttpMethod.Get,
                query: parameters,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<IEnumerable<TradeFeeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CoinModel>> GetAllCoinsInformationAsync(
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                 BinanceEndpoints.ALL_COINS_INFORMATION,
                 HttpMethod.Get,
                 query: new Dictionary<string, object>
                 {
                    { "recvWindow", recvWindow },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                 },
                 cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<List<CoinModel>>(result);
        }

        #endregion
    }
}
