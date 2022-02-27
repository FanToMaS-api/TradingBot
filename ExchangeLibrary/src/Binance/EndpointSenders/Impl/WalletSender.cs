﻿using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Models;
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
            Dictionary<string, object> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.ACCOUNT_API_TRADING_STATUS,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<AccountTraidingStatusModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync(
            Dictionary<string, object> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.TRADE_FEE,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<IEnumerable<TradeFeeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CoinModel>> GetAllCoinsInformationAsync(
            Dictionary<string, object> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                 BinanceEndpoints.ALL_COINS_INFORMATION,
                 HttpMethod.Get,
                 query: query,
                 cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<List<CoinModel>>(result);
        }

        #endregion
    }
}
