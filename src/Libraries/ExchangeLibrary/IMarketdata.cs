using Common.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary
{
    /// <summary>
    ///     Возвращает маркетдату
    /// </summary>
    public interface IMarketdata
    {
        /// <summary>
        ///     Получить информацию о правилах торговли на бирже
        /// </summary>
        Task<IEnumerable<TradeObjectRuleTradingModel>> GetExchangeInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить книгу ордеров по определенному объекту торговли
        /// </summary>
        /// <param name="name"> Объект торговли </param>
        /// <param name="limit"> 
        ///     Необходимое кол-во ордеров.
        ///     Возможные значения: 5, 10, 20, 50, 100, 500, 1000, 5000
        /// </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<OrderBookModel> GetOrderBookAsync(
            string name,
            int limit = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последние сделки по паре
        /// </summary>
        Task<IEnumerable<TradeModel>> GetRecentTradesAsync(
            string name,
            int limit = 500,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает исторические сделки по объекту торговли
        /// </summary>
        /// <param name="name"> Объект торговли  </param>
        /// <param name="fromId"> Идентификатор сделки для получения. По умолчанию получают самые последние сделки </param>
        /// <param name="limit"> Кол-во сделок (максимум 1000, по умолчанию 500) </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<IEnumerable<TradeModel>> GetOldTradesAsync(
            string name,
            long? fromId = null,
            int limit = 500,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает свечи по определенному объекту торговли
        /// </summary>
        /// <param name="name"> Объект торговли </param>
        /// <param name="interval"> Период свечи </param>
        /// <param name="startTime"> Время начала построения </param>
        /// <param name="endTime"> Окончание периода </param>
        /// <param name="limit"> Кол-во свечей (максимум 1000, по умолчанию 500) </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<IEnumerable<CandlestickModel>> GetCandlestickAsync(
            string name,
            string interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает текущую среднюю цену объекта торговли
        /// </summary>
        Task<double> GetAveragePriceAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает 24 статистику о цене для объекта торговли или объектов,
        ///     если <code><paramref name="name" /> = <see langword="null"/> or <see langword="''"/></code>
        /// </summary>
        Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последнюю цену для пары или для всех объектов торговли,
        ///     если <code><paramref name="name" /> = <see langword="null"/> or <see langword="''"/></code>
        /// </summary>
        Task<IEnumerable<TradeObjectNamePriceModel>> GetSymbolPriceTickerAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает лучшую цену/количество в стакане для объекта торговли или объектов
        /// </summary>
        Task<IEnumerable<BestSymbolOrderModel>> GetBestSymbolOrdersAsync(
            string name,
            CancellationToken cancellationToken = default);
    }
}
