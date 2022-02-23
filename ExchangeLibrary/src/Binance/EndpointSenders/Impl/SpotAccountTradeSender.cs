using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Models;
using NLog;
using System.Collections.Generic;
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
        private readonly JsonDeserializerWrapper _converter;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="SpotAccountTradeSender" />
        public SpotAccountTradeSender(IBinanceClient client)
        {
            _client = client;
            _converter = new JsonDeserializerWrapper();
            _converter.AddConverter(new FullOrderResponseModelConverter());
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

            return _converter.Deserialize<FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<FullOrderResponseModel> SendNewOrderAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.NEW_ORDER,
                HttpMethod.Post,
                query: query,
                cancellationToken: cancellationToken);

            return _converter.Deserialize<FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<CancelOrderResponseModel> CancelOrderAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.CANCEL_ORDER,
                HttpMethod.Delete,
                query: query,
                cancellationToken: cancellationToken);

            return _converter.Deserialize<CancelOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<CancelOrderResponseModel> CancelAllOrdersAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.CANCEL_All_ORDERS,
                HttpMethod.Delete,
                query: query,
                cancellationToken: cancellationToken);

            // TODO: Здесь нужно придумать модель
            return _converter.Deserialize<CancelOrderResponseModel>(result);
        }

        #endregion
    }
}
