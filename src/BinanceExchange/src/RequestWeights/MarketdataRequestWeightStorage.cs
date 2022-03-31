using BinanceExchange.Models;

namespace BinanceExchange.RequestWeights
{
    /// <summary>
    ///     Веса запросов к конечным точкам маркетдаты
    /// </summary>
    internal class MarketdataRequestWeightStorage
    {
        /// <summary>
        ///     Вес запроса информации о правилах торговли символами
        /// </summary>
        public RequestWeightModel ExchangeInfoWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 10 }
            });

        /// <summary>
        ///     Вес запроса книги ордеров
        /// </summary>
        public RequestWeightModel OrderBookWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { "5", 1 },
                { "10", 1 },
                { "20", 1 },
                { "50", 1 },
                { "100", 1 },
                { "500", 5 },
                { "1000", 10 },
                { "5000", 50 },
            });

        /// <summary>
        ///     Вес запроса списка недавних сделок
        /// </summary>
        public RequestWeightModel RecentTradesWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса списка недавних сделок
        /// </summary>
        public RequestWeightModel OldTradesWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 5 }
            });

        /// <summary>
        ///     Вес запроса свечей для монеты
        /// </summary>
        public RequestWeightModel CandlestickDataWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса текущей средней цены монеты
        /// </summary>
        public RequestWeightModel CurrentAveragePriceWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса изменения цены пары за 24 часа
        /// </summary>
        public RequestWeightModel DayTickerPriceChangeWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 },
                { "null", 40 },
            });

        /// <summary>
        ///     Вес запроса последней цены/цен пары/пар
        /// </summary>
        public RequestWeightModel SymbolPriceTickerWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 },
                { "null", 2 },
            });

        /// <summary>
        ///     Вес запроса лучшая цены/количества в стакане для символа или символов
        /// </summary>
        public RequestWeightModel SymbolOrderBookTickerWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 },
                { "null", 2 },
            });
    }
}
