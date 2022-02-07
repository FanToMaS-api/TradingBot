using ExchangeLibrary.Binance.DTOs.Marketdata;
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
        /// <param name="symbol"></param>
        /// <param name="limit"> Кол-во сделокю Принимает значения от 500 до 1000 </param>
        /// <remarks>
        ///    <paramref name="limit"> Принимает значения от 500 до 1000 </paramref>
        /// </remarks>
        Task<IEnumerable<RecentTradeDto>> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default);
    }
}
