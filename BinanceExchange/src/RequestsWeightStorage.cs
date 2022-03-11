﻿using BinanceExchange.Models;

namespace BinanceExchange
{
    /// <summary>
    ///     Хранилище весов запросов для Binance
    /// </summary>
    internal class RequestsWeightStorage
    {
        #region Wallet's requests weights

        /// <summary>
        ///     Вес запроса статуса системы
        /// </summary>
        public RequestWeightModel SistemStatusWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса статуса аккаунта
        /// </summary>
        public RequestWeightModel AccountStatusWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса информации обо всех монетах
        /// </summary>
        public RequestWeightModel AllCoinsInfoWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 10 }
            });

        /// <summary>
        ///     Вес запроса таксы за торговлю определенными монетами
        /// </summary>
        public RequestWeightModel TradeFeeWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        #endregion

        #region Marketdata requests weights

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
        public RequestWeightModel CandleStickDataWeight { get; } = new RequestWeightModel(
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

        #endregion

        #region Spot Account/Trade weights

        /// <summary>
        ///     Вес запроса на создание нового ордера (включая тестовый)
        /// </summary>
        public RequestWeightModel NewOrderWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса на отмену активного ордера по паре (включая отмену всех оредров по паре)
        /// </summary>
        public RequestWeightModel CancelOrderWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса на проверку состояния ордера по паре
        /// </summary>
        public RequestWeightModel CheckOrderWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 2 }
            });

        /// <summary>
        ///     Вес запроса на проверку состояния всех открытых ордеров или только по паре
        /// </summary>
        public RequestWeightModel CheckAllOpenOrdersWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 3 },
                { "null", 40 }
            });

        /// <summary>
        ///     Вес запроса на получение всех ордеров аккаунта по паре
        /// </summary>
        public RequestWeightModel AllOrdersWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 10 }
            });

        #endregion
    }
}
