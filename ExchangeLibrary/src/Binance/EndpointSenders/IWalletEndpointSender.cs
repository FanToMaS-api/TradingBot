using ExchangeLibrary.Binance.DTOs.Wallet;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders
{
    /// <summary>
    ///     Отвечает за отправку запросов к Wallet
    /// </summary>
    public interface IWalletEndpointSender 
    {
        /// <summary>
        ///     Вернуть статус системы
        /// </summary>
        Task<SystemStatusDto> GetSystemStatusAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Вернуть статус аккаунта
        /// </summary>
        Task<AccountTraidingStatusDto> GetAccountTraidingStatusAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить всю информацию о монетах
        /// </summary>
        Task<IEnumerable<CoinDto>> GetAllCoinsInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Запрос таксы по всем монетам или по конктретной
        /// </summary>
        /// <param name="symbol"> Наименование конкретной монеты</param>
        Task<IEnumerable<TradeFeeDto>> GetTradeFeeAsync(string symbol = null, long recvWindow = 5000, CancellationToken cancellationToken = default);
    }
}
