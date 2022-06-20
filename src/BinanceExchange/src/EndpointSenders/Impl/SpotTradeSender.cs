using BinanceExchange.Client;
using BinanceExchange.Client.Http.Request.Builders;
using BinanceExchange.JsonConverters;
using BinanceExchange.Models;
using Common.JsonConvertWrapper;
using Common.JsonConvertWrapper.Converters;
using NLog;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.EndpointSenders.Impl
{
    /// <inheritdoc cref="ISpotTradeSender"/>
    internal class SpotTradeSender : ISpotTradeSender
    {
        #region Fields

        private readonly IBinanceClient _client;
        private readonly JsonDeserializerWrapper _converter;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="SpotTradeSender" />
        public SpotTradeSender(IBinanceClient client)
        {
            _client = client;
            _converter = new JsonDeserializerWrapper();
            _converter.AddConverter(new FullOrderResponseModelConverter());
            _converter.AddConverter(new CancelOrderResponseModelConverter());
            _converter.AddConverter(new CheckOrderResponseModelConverter());
            _converter.AddConverter(new EnumerableDeserializer<CancelOrderResponseModel>());
            _converter.AddConverter(new EnumerableDeserializer<CheckOrderResponseModel>());
            _converter.AddConverter(new AccountInformationModelConverter());
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<FullOrderResponseModel> SendNewTestOrderAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.NEW_TEST_ORDER)
                .SetHttpMethod(HttpMethod.Post)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<FullOrderResponseModel>(response);
        }

        /// <inheritdoc />
        public async Task<FullOrderResponseModel> SendNewOrderAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.NEW_ORDER)
                .SetHttpMethod(HttpMethod.Post)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<FullOrderResponseModel>(response);
        }

        /// <inheritdoc />
        public async Task<CancelOrderResponseModel> CancelOrderAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.CANCEL_ORDER)
                .SetHttpMethod(HttpMethod.Delete)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<CancelOrderResponseModel>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CancelOrderResponseModel>> CancelAllOrdersAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.CANCEL_All_ORDERS)
                .SetHttpMethod(HttpMethod.Delete)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            // (TODO: OCO ордера не обрабатываются)
            return _converter.Deserialize<IEnumerable<CancelOrderResponseModel>>(response);
        }

        /// <inheritdoc />
        public async Task<CheckOrderResponseModel> CheckOrderAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.CHECK_ORDER)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<CheckOrderResponseModel>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CheckOrderResponseModel>> CheckAllOpenOrdersAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.CHECK_ALL_OPEN_ORDERS)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<IEnumerable<CheckOrderResponseModel>>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CheckOrderResponseModel>> GetAllOrdersAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.GET_ALL_ORDERS)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<IEnumerable<CheckOrderResponseModel>>(response);
        }

        /// <inheritdoc />
        public async Task<AccountInformationModel> GetAccountInformationAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.ACCOUNT_INFORMATION)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<AccountInformationModel>(response);
        }

        #endregion
    }
}
