using BinanceExchange.Client.Http.Request.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.Client
{
    /// <summary>
    ///     Базовый клиент Binance для разделов REST API
    /// </summary>
    internal interface IBinanceClient
    {
        /// <summary>
        ///     Отправить запрос без подписи
        /// </summary>
        Task<string> SendPublicAsync(
            IRequestModel requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отправить подписанный запрос
        /// </summary>
        Task<string> SendSignedAsync(
            IRequestModel requestModel,
            CancellationToken cancellationToken = default);
    }
}
