using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using ExchangeLibrary.Binance.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders.Impl
{
    /// <inheritdoc cref="ISpotAccountTradeSender"/>
    internal class SpotAccountTradeSender : ISpotAccountTradeSender
    {
        #region Fields

        private readonly IBinanceClient _client;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="SpotAccountTradeSender" />
        public SpotAccountTradeSender(IBinanceClient client)
        {
            _client = client;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<FullOrderResponseModel> SendNewTestOrderAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.NEW_TEST_ORDER,
                HttpMethod.Post,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<FullOrderResponseModel> SendNewOrderAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.NEW_ORDER,
                HttpMethod.Post,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<FullOrderResponseModel>(result);
        }

        #endregion
    }
}
