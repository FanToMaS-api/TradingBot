using Common.Models;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.DTOs;
using Newtonsoft.Json;
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
    internal class WalletEndpointSender : IWalletEndpointSender
    {
        #region Fields

        private readonly IBinanceClient _client;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="WalletEndpointSender" />
        public WalletEndpointSender(IBinanceClient client)
        {
            _client = client;
        }

        #endregion

        #region Public methods

        /// TODO: Здесь вместо string должны уже возвращаться модели или какие-то смысленные типы, незачем тащить string кучу уровней

        /// <inheritdoc />
        public async Task<string> GetSystemStatusAsync(CancellationToken cancellationToken) =>
             await _client.SendPublicAsync(
                BinanceEndpoints.SYSTEM_STATUS,
                HttpMethod.Get,
                cancellationToken: cancellationToken);

        /// <inheritdoc />
        public async Task<string> GetAccountStatusAsync(long recvWindow, CancellationToken cancellationToken) =>
             await _client.SendSignedAsync(
                 BinanceEndpoints.ACCOUNT_STATUS,
                 HttpMethod.Get,
                 query: new Dictionary<string, object>
                 {
                    { "recvWindow", recvWindow },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                 },
                 cancellationToken);


        /// <inheritdoc />
        public async Task<IEnumerable<CoinDTO>> GetAllCoinsInformationAsync(long recvWindow, CancellationToken cancellationToken)
        {
            var result = await _client.SendSignedAsync(
                 BinanceEndpoints.ALL_COINS_INFORMATION,
                 HttpMethod.Get,
                 query: new Dictionary<string, object>
                 {
                    { "recvWindow", recvWindow },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                 },
                 cancellationToken);

            return JsonConvert.DeserializeObject<List<CoinDTO>>(result);
        }


        #endregion
    }
}
