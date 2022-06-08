using BinanceExchange.Client.Helpers;
using BinanceExchange.Client.Http.Request.Models;
using BinanceExchange.Enums;
using BinanceExchange.Exceptions;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.Client.Impl
{
    /// <summary>
    ///     Клиент Binance для разделов REST API
    /// </summary>
    internal class BinanceClient : IBinanceClient
    {
        #region Fields

        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceClient"/>
        public BinanceClient(IHttpClientFactory httpClientFactory, BinanceExchangeOptions options)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = options.ApiKey;
            _apiSecret = options.SecretKey;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<string> SendPublicAsync(
            IRequestModel requestModel,
            CancellationToken cancellationToken = default) =>
            await SendAsync(requestModel, cancellationToken: cancellationToken);

        /// <inheritdoc />
        public async Task<string> SendSignedAsync(
            IRequestModel requestModel,
            CancellationToken cancellationToken = default)
        {
            var signParameters = BinanceUrlHelper.Sign(requestModel.ParametersStr, _apiSecret);

            var signature = requestModel.ParametersStr.Any()
                ? $"&signature={signParameters}"
                : $"?signature={signParameters}";

            return await SendAsync(requestModel, signature, cancellationToken);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Осуществляет отправку запроса
        /// </summary>
        private async Task<string> SendAsync(
            IRequestModel requestModel,
            string signature = "",
            CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(requestModel.HttpMethod, $"{requestModel.Url}{signature}");
            if (requestModel.Body is not null)
            {
                request.Content = new ByteArrayContent(requestModel.Body);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(requestModel.ContentType));
            }

            if (_apiKey is not null)
            {
                request.Headers.Add("X-MBX-APIKEY", _apiKey);
            }

            using var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                await ProcessBadResponseAsync(response, cancellationToken);
            }

            using var responseContent = response.Content;
            return await responseContent.ReadAsStringAsync(cancellationToken);
        }

        /// <summary>
        ///     Обработка неверного ответа с Binance
        /// </summary>
        private static async Task ProcessBadResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            using var responseContent = response.Content;
            var statusCode = (int)response.StatusCode;
            var message = await response.Content.ReadAsStringAsync(cancellationToken);
            var httpException = statusCode switch
            {
                403 => new BinanceException(BinanceExceptionType.WAFLimit, message),
                429 => new BinanceException(BinanceExceptionType.RateLimit, message),
                418 => new BinanceException(BinanceExceptionType.Blocked, message),
                >= 500 => new BinanceException(BinanceExceptionType.ServerException, message),
                >= 400 => new BinanceException(BinanceExceptionType.InvalidRequest, message),
                _ => new BinanceException($"Unknown error type with code")
            };

            httpException.StatusCode = statusCode;
            httpException.Headers = response.Headers.ToDictionary(a => a.Key, a => a.Value);

            throw httpException;

        }

        #endregion
    }
}
