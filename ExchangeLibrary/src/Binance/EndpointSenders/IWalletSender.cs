using ExchangeLibrary.Binance.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders
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
        Task<AccountTraidingStatusModel> GetAccountTraidingStatusAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить всю информацию о монетах
        /// </summary>
        Task<IEnumerable<CoinModel>> GetAllCoinsInformationAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Запрос таксы по всем парам или по конктретной
        /// </summary>
        /// <param name="symbol"> Наименование конкретной пары </param>
        Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default);
    }
}
