using ExchangeLibrary.Binance.Enums;
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
        ///     Получить ордера из стакана по определенной монете
        /// </summary>
        /// <param name="symbol"> Наименование пары </param>
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
        Task<OrderBookModel> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последние сделки по паре
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="limit"> Кол-во сделок (максимум 1000, по умолчанию 500) </param>
        Task<IEnumerable<RecentTradeModel>> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает исторические сделки по паре
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="fromId"> Нижняя граница выгрузки </param>
        /// <param name="limit"> Кол-во сделок (максимум 1000, по умолчанию 500) </param>
        Task<IEnumerable<RecentTradeModel>> GetOldTradesAsync(string symbol, long fromId, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает свечи по определенной паре
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="interval"> Период свечи </param>
        /// <param name="startTime"> Время начала построения </param>
        /// <param name="endTime"> Окончание периода </param>
        /// <param name="limit"> Кол-во свечей (максимум 1000, по умолчанию 500) </param>
        Task<IEnumerable<CandlestickModel>> GetCandleStickAsync(
            string symbol,
            CandleStickIntervalType interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает текущую среднюю цену пары
        /// </summary>
        Task<AveragePriceModel> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает 24 статистику о цене для пары или для всех пар (если <code><paramref name="symbol" /> = null or ""</code>)
        /// </summary>
        Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последнюю цену для пары или для всех пар (если <code><paramref name="symbol" /> = null or ""</code>)
        /// </summary>
        Task<IEnumerable<SymbolPriceTickerModel>> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает лучшую цену/количество в стакане для символа или символов
        /// </summary>
        Task<IEnumerable<SymbolOrderBookTickerModel>> GetSymbolOrderBookTickerAsync(
            string symbol,
            CancellationToken cancellationToken = default);
    }
}
