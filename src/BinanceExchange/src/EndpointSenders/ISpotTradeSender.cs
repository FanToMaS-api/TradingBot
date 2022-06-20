using BinanceExchange.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.EndpointSenders
{
    /// <summary>
    ///     Отвечает за отправку запросов к конечным точкам Spot Account/Trade
    /// </summary>
    internal interface ISpotTradeSender
    {
        /// <summary>
        ///     Отправить новый ТЕСТОВЫЙ ордер
        /// </summary>  
        Task<FullOrderResponseModel> SendNewTestOrderAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отправить новый ордер
        /// </summary>  
        Task<FullOrderResponseModel> SendNewOrderAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отменить ордер
        /// </summary>
        Task<CancelOrderResponseModel> CancelOrderAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отменить все ордера по определенной паре
        /// </summary>
        Task<IEnumerable<CancelOrderResponseModel>> CancelAllOrdersAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Проверить текущее состояние ордера
        /// </summary>
        Task<CheckOrderResponseModel> CheckOrderAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Проверить статус всех открытых ордеров (или всех ордеров по конкретному символу)
        /// </summary>
        Task<IEnumerable<CheckOrderResponseModel>> CheckAllOpenOrdersAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить все ордера по паре (открытые, завершенные, отмененные)
        /// </summary>
        Task<IEnumerable<CheckOrderResponseModel>> GetAllOrdersAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить информацию об аккаунте
        /// </summary>
        Task<AccountInformationModel> GetAccountInformationAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);
    }
}
