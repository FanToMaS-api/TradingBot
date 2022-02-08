﻿using ExchangeLibrary.Binance.Models;

namespace ExchangeLibrary.Binance
{
    /// <summary>
    ///     Хранилище весов запросов для Binance
    /// </summary>
    internal class RequestsWeightStorage
    {
        #region Wallet's requests weight

        /// <summary>
        ///     Вес запроса статуса системы
        /// </summary>
        public RequestWeightModel SistemStatusWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса статуса аккаунта
        /// </summary>
        public RequestWeightModel AccountStatusWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса информации обо всех монетах
        /// </summary>
        public RequestWeightModel AllCoinsInfoWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 10 }
            });

        /// <summary>
        ///     Вес запроса таксы за торговлю определенными монетами
        /// </summary>
        public RequestWeightModel TradeFeeWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        #endregion

        #region Marketdata requests weight

        /// <summary>
        ///     Вес запроса книги ордеров
        /// </summary>
        public RequestWeightModel OrderBookWeight { get; set; } = new RequestWeightModel(
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
        public RequestWeightModel RecentTradesWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса списка недавних сделок
        /// </summary>
        public RequestWeightModel OldTradesWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 5 }
            });

        /// <summary>
        ///     Вес запроса свечей для монеты
        /// </summary>
        public RequestWeightModel CandleStickDataWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса текущей средней цены монеты
        /// </summary>
        public RequestWeightModel CurrentAveragePriceWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса изменения цены пары за 24 часа
        /// </summary>
        public RequestWeightModel DayTickerPriceChangeWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 },
                { "null", 40 },
            });

        #endregion
    }
}
