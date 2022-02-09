using Common.Enums;
using Common.Models;
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
        ///     Вернуть статус системы
        /// </summary>
        Task<string> GetSystemStatusAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Вернуть статус аккаунта
        /// </summary>
        Task<string> GetAccountTraidingStatusAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить всю информацию о монетах
        /// </summary>
        Task<IEnumerable<ITrade>> GetAllCoinsInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить информацию о таксе за все монеты или за определенную
        /// </summary>
        /// <param name="symbol"> Обозначение монеты </param>
        Task<string> GetTradeFeeAsync(string symbol = null, long recvWindow = 5000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить книгу ордеров по определенной паре
        /// </summary>
        Task<string> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последние сделки по паре
        /// </summary>
        Task<string> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает исторические сделки
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="fromId"> Нижняя граница выгрузки </param>
        /// <param name="limit"> Кол-во сделок (максимум 1000, по умолчанию 500) </param>
        Task<string> GetOldTradesAsync(string symbol, long fromId, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает свечи по определенной паре
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="interval"> Период свечи </param>
        /// <param name="startTime"> Время начала построения </param>
        /// <param name="endTime"> Окончание периода </param>
        /// <param name="limit"> Кол-во свечей (максимум 1000, по умолчанию 500) </param>
        Task<string> GetCandleStickAsync(
            string symbol,
            CandleStickIntervalType interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает текущую среднюю цену пары
        /// </summary>
        Task<string> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает 24 статистику о цене для пары или для всех пар если <code><paramref name="symbol" /> = null or ""</code>)
        /// </summary>
        Task<string> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последнюю цену для пары или для всех пар (если <code><paramref name="symbol" /> = null or ""</code>)
        /// </summary>
        Task<string> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает лучшую цену/количество в стакане для символа или символов
        /// </summary>
        Task<string> GetSymbolOrderBookTickerAsync(string symbol, CancellationToken cancellationToken = default);
    }
}
