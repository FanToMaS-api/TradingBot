﻿using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary
{
    /// <summary>
    ///     Общий интерфейс для всех бирж
    /// </summary>
    public interface IExchange
    {
        /// <summary>
        ///     Вернуть статус системы
        /// </summary>
        Task<string> GetSystemStatusAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Получить всю информацию о монетах
        /// </summary>
        Task<string> GetAllCoinsInformationAsync(long recvWindow, CancellationToken cancellationToken);
    }
}
