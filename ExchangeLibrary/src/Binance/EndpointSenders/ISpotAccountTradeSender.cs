using ExchangeLibrary.Binance.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders
{
    /// <summary>
    ///     Отвечает за отправку запросов к конечным точкам Spot Account/Trade
    /// </summary>
    internal interface ISpotAccountTradeSender
    {
        /// <summary>
        ///     Отправить новый ТЕСТОВЫЙ ордер
        /// </summary>  
        Task<FullOrderResponseModel> SendNewTestOrderAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отправить новый ордер
        /// </summary>  
        Task<FullOrderResponseModel> SendNewOrderAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default);
    }
}
