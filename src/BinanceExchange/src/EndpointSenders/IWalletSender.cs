using BinanceExchange.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.EndpointSenders
{
    /// <summary>
    ///     Отвечает за отправку запросов к конечным точкам Wallet
    /// </summary>
    internal interface IWalletSender 
    {
        /// <summary>
        ///     Вернуть статус системы
        /// </summary>
        Task<SystemStatusModel> GetSystemStatusAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Вернуть статус аккаунта
        /// </summary>
        Task<AccountTradingStatusModel> GetAccountTradingStatusAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить всю информацию о монетах
        /// </summary>
        Task<IEnumerable<CoinModel>> GetAllCoinsInformationAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Запрос таксы по всем парам или по конктретной
        /// </summary>
        Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default);
    }
}
