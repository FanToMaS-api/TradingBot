using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Exceptions;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.Client.Impl
{
    /// <summary>
    ///     Клиент Binance для разделов REST API.
    /// </summary>
    internal class BinanceClient : IBinanceClient
    {
        #region Fields

        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly HttpClient _client;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _baseUrl = "https://api.binance.com";

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceClient"/>
        public BinanceClient(HttpClient httpClient, string apiKey, string apiSecret)
        {
            _client = httpClient;
            _apiKey = apiKey;
            _apiSecret = apiSecret;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<string> SendPublicAsync(
            string requestUri,
            HttpMethod httpMethod,
            Dictionary<string, object> query = null,
            object content = null,
            CancellationToken cancellationToken = default)
        {
            if (query is not null)
            {
                var queryStringBuilder = BinanceUrlHelper.BuildQueryString(query, new StringBuilder());

                if (queryStringBuilder.Length > 0)
                {
                    requestUri += "?" + queryStringBuilder.ToString();
                }
            }

            return await SendAsync(requestUri, httpMethod, content, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> SendSignedAsync(
            string requestUri,
            HttpMethod httpMethod,
            Dictionary<string, object> query = null,
            object content = null,
            CancellationToken cancellationToken = default)
        {
            var queryStringBuilder = new StringBuilder();

            if (query is not null)
            {
                BinanceUrlHelper.BuildQueryString(query, queryStringBuilder);
            }

            var signature = BinanceUrlHelper.Sign(queryStringBuilder.ToString(), _apiSecret);

            if (queryStringBuilder.Length > 0)
            {
                queryStringBuilder.Append("&");
            }

            queryStringBuilder.Append($"signature={signature}");

            requestUri += $"?{queryStringBuilder}";

            return await SendAsync(requestUri, httpMethod, content, cancellationToken: cancellationToken);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Осуществляет отправку запроса
        /// </summary>
        private async Task<string> SendAsync(string requestUri, HttpMethod httpMethod, object content = null, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(httpMethod, _baseUrl + requestUri))
            {
                if (content is not null)
                {
                    request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
                }

                if (_apiKey is not null)
                {
                    request.Headers.Add("X-MBX-APIKEY", _apiKey);
                }

                var response = await _client.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    await ProcessBadResponseAsync(response, cancellationToken);
                }

                using (var responseContent = response.Content)
                {
                    return await responseContent.ReadAsStringAsync(cancellationToken);
                }
            }
        }

        /// <summary>
        ///     Обработка неверного отвечета с Binance
        /// </summary>
        private async Task ProcessBadResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            using (HttpContent responseContent = response.Content)
            {
                var statusCode = (int)response.StatusCode;
                var message = await response.Content.ReadAsStringAsync(cancellationToken);
                BinanceException httpException = statusCode switch
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
}
