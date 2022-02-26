using Common.JsonConvertWrapper;
using Common.JsonConvertWrapper.Converters;
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
            _converter.AddConverter(new CancelOrderResponseModelConverter());
            _converter.AddConverter(new CheckOrderResponseModelConverter());
            _converter.AddConverter(new EnumerableDeserializer<CancelOrderResponseModel>());
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
        public async Task<IEnumerable<CancelOrderResponseModel>> CancelAllOrdersAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.CANCEL_All_ORDERS,
                HttpMethod.Delete,
                query: query,
                cancellationToken: cancellationToken);

            // (TODO: OCO ордера не обрабатываются)
            return _converter.Deserialize<IEnumerable<CancelOrderResponseModel>>(result);
        }

        /// <inheritdoc />
        public async Task<CheckOrderResponseModel> CheckOrderAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendSignedAsync(
                BinanceEndpoints.CHECK_ORDER,
                HttpMethod.Delete,
                query: query,
                cancellationToken: cancellationToken);

            return _converter.Deserialize<CheckOrderResponseModel>(result);
        }

        #endregion
    }
}
