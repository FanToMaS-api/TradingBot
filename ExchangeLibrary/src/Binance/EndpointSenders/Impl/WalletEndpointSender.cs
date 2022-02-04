using ExchangeLibrary.Binance.Client;
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
        public async Task<string> GetAllCoinsInformationAsync(long recvWindow, CancellationToken cancellationToken) =>
            await _client.SendSignedAsync(
                BinanceEndpoints.ALL_COINS_INFORMATION,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "recvWindow", recvWindow },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                });

        #endregion
    }
}
