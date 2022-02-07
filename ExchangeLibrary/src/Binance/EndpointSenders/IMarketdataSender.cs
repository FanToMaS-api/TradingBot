using Common.Enums;
using ExchangeLibrary.Binance.DTOs.Marketdata;
using ExchangeLibrary.Binance.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders
{
    /// <summary>
    ///     Отвечает за отправку запросов к конечным точкам маркетадаты
    /// </summary>
    public interface IMarketdataSender
    {
        /// <summary>
        ///     Получить книгу ордеров по определенной монете
        /// </summary>
        /// <param name="symbol"> Наименование монеты </param>
        /// <param name="limit"> 
        ///     Необходимое кол-во ордеров.
        ///     Возможные значения: 5, 10, 20, 50, 100, 500, 1000, 5000
        /// </param>
        /// <remarks>
        ///     Возможные значения <paramref name="limit"/>: 5, 10, 20, 50, 100, 500, 1000, 5000
        ///     Веса для глубины запроса соответственно:
        ///     1 - для 5:100;
        ///     5 - 500;
        ///     10 - 1000;
        ///     50 - 5000;
        /// </remarks>
        Task<OrderBookDto> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последние сделки по монете
        /// </summary>
        /// <param name="symbol"> Монета </param>
        /// <param name="limit"> Кол-во сделок (максимум 1000, по умолчанию 500) </param>
        Task<IEnumerable<RecentTradeDto>> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает исторические сделки по монете
        /// </summary>
        /// <param name="symbol"> Монета </param>
        /// <param name="fromId"> Нижняя граница выгрузки </param>
        /// <param name="limit"> Кол-во сделок (максимум 1000, по умолчанию 500) </param>
        Task<IEnumerable<RecentTradeDto>> GetOldTradesAsync(string symbol, long fromId, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает свечи по определенной паре
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="interval"> Период свечи </param>
        /// <param name="startTime"> Время начала построения </param>
        /// <param name="endTime"> Окончание периода </param>
        /// <param name="limit"> Кол-во свечей (максимум 1000, по умолчанию 500) </param>
        Task<IEnumerable<CandleStickDto>> GetCandleStickAsync(
            string symbol,
            CandleStickIntervalType interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default);
    }
}
