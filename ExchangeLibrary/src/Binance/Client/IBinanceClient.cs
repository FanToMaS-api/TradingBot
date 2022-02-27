using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.Client
{
    /// <summary>
    ///     Базовый клиент Binance для разделов REST API.
    /// </summary>
    internal interface IBinanceClient
    {
        /// <summary>
        ///     Отправить запрос без подписи
        /// </summary>
        Task<string> SendPublicAsync(
            string requestUri,
            HttpMethod httpMethod,
            Dictionary<string, object> query = null,
            object content = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отправить подписанный запрос
        /// </summary>
        Task<string> SendSignedAsync(
            string requestUri,
            HttpMethod httpMethod,
            Dictionary<string, object> query = null,
            object content = null,
            CancellationToken cancellationToken = default);
    }
}
