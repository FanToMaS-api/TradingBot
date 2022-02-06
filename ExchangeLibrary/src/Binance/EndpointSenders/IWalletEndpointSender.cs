using Common.Models;
using ExchangeLibrary.Binance.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders
{
    public interface IWalletEndpointSender 
    {
        /// <summary>
        ///     Вернуть статус системы
        /// </summary>
        Task<string> GetSystemStatusAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Вернуть статус аккаунта
        /// </summary>
        Task<string> GetAccountStatusAsync(long recvWindow, CancellationToken cancellationToken);

        /// <summary>
        ///     Получить всю информацию о монетах
        /// </summary>
        Task<IEnumerable<CoinDTO>> GetAllCoinsInformationAsync(long recvWindow, CancellationToken cancellationToken);
    }
}
