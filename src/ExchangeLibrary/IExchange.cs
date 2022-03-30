using Common.Enums;
using Common.Models;
using Common.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary
{
    /// <summary>
    ///     Общий интерфейс для всех бирж
    /// </summary>
    public interface IExchange
    {
        /// <summary>
        ///     Возвращает общие данные о кошелке/бирже/объектаз торговли
        /// </summary>
        IWallet Wallet { get; }

        /// <summary>
        ///     Возвращает маркетдату
        /// </summary>
        IMarketdata Marketdata { get; }

        /// <summary>
        ///     Создает подключения к стримам маркетдаты
        /// </summary>
        IMarketdataStreams MarketdataStreams { get; }

        /// <summary>
        ///     Управляет торговлей
        /// </summary>
        ISpotTrade SpotTrade { get; }
    }
}
