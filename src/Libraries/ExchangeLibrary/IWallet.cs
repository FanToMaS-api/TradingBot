using Common.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary
{
    /// <summary>
    ///     Возвращает общие данные о кошелке/бирже/объектаз торговли
    /// </summary>
    public interface IWallet
    {
        /// <summary>
        ///     Вернуть статус системы
        /// </summary>
        Task<bool> GetSystemStatusAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Вернуть статус аккаунта
        /// </summary>
        Task<TradingAccountInfoModel> GetAccountTradingStatusAsync(
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить всю информацию об объектах торговли
        /// </summary>
        Task<IEnumerable<TradeObject>> GetAllTradeObjectInformationAsync(
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить информацию о таксе за все объекты торговли или за определенный
        /// </summary>
        /// <param name="name"> Объект торговли </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync(
            string name = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);
    }
}
