﻿using Common.Models;
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
        #region Wallet

        /// <summary>
        ///     Вернуть статус системы
        /// </summary>
        Task<bool> GetSystemStatusAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Вернуть статус аккаунта
        /// </summary>
        Task<TradingAccountInfoModel> GetAccountTradingStatusAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить всю информацию о монетах
        /// </summary>
        Task<IEnumerable<CoinModel>> GetAllCoinsInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить информацию о таксе за все монеты или за определенную
        /// </summary>
        /// <param name="symbol"> Обозначение пары </param>
        Task<TradeFeeModel> GetTradeFeeAsync(string symbol = null, long recvWindow = 5000, CancellationToken cancellationToken = default);

        #endregion

        #region Marketdata

        /// <summary>
        ///     Получить информацию о правилах торговли парами на бирже
        /// </summary>
        Task<IEnumerable<SymbolRuleTradingModel>> GetExchangeInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить книгу ордеров по определенной паре
        /// </summary>
        /// <param name="limit"> Глубина запроса (кол-во записей) </param>
        Task<OrderBookModel> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последние сделки по паре
        /// </summary>
        Task<IEnumerable<TradeModel>> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает исторические сделки
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="fromId"> Идентификатор сделки для получения. По умолчанию получают самые последние сделки </param>
        /// <param name="limit"> Кол-во сделок (максимум 1000, по умолчанию 500) </param>
        Task<IEnumerable<TradeModel>> GetOldTradesAsync(string symbol, long? fromId = null, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает свечи по определенной паре
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="interval"> Период свечи </param>
        /// <param name="startTime"> Время начала построения </param>
        /// <param name="endTime"> Окончание периода </param>
        /// <param name="limit"> Кол-во свечей (максимум 1000, по умолчанию 500) </param>
        Task<IEnumerable<CandlestickModel>> GetCandlstickAsync(
            string symbol,
            string interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает текущую среднюю цену пары
        /// </summary>
        Task<double> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает 24 статистику о цене для пары или для всех пар если <code><paramref name="symbol" /> = null or ""</code>)
        /// </summary>
        Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последнюю цену для пары или для всех пар (если <code><paramref name="symbol" /> = null or ""</code>)
        /// </summary>
        Task<IEnumerable<SymbolPriceModel>> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает лучшую цену/количество в стакане для символа или символов
        /// </summary>
        Task<IEnumerable<BestSymbolOrderModel>> GetSymbolOrderBookTickerAsync(string symbol, CancellationToken cancellationToken = default);

        #endregion

        #region Marketdata Streams

        /// <summary>
        ///     Подписывается на стрим данных
        /// </summary>
        /// <typeparam name="T"> Объект для работы с данными полученными со стрима </typeparam>
        /// <param name="symbol"> Пара </param>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="{T}"/> </param>
        /// <param name="streamType"> Тип стрима </param>
        Task SubscribeNewStreamAsync<T>(
            string symbol,
            Func<T, Task> onMessageReceivedFunc,
            string streamType,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим данных по свечам для опред пары
        /// </summary>
        /// <param name="symbol"> Пара </param>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="CandlestickStreamModel"/> </param>
        /// <param name="candleStickInterval"> Интервал свечей </param>
        Task SubscribeCandlestickStreamAsync(
            string symbol,
            string candleStickInterval,
            Func<CandlestickStreamModel, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим статистики всех мини-тикеров за 24 часа
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="TickerStreamModel"/> </param>
        Task SubscribeAllMarketTickersStreamAsync(
            Func<IEnumerable<TickerStreamModel>, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим обновлений лучшей цены покупки или продажи или количество
        ///     в режиме реального времени для всех символов
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="BookTickerStreamModel"/> </param>
        Task SubscribeAllBookTickersStreamAsync(
            Func<IEnumerable<BookTickerStreamModel>, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим статистики всех мини-тикеров за 24 часа
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатывающая данные объекта <see cref="MiniTickerStreamModel"/> </param>
        Task SubscribeAllMarketMiniTickersStreamAsync(
            Func<IEnumerable<MiniTickerStreamModel>, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим лучших ордеров спроса и предложений
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="OrderBookModel"/> </param>
        /// <param name="levels"> Кол-во оредеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        Task SubscribePartialBookDepthStreamAsync(
            string symbol,
            Func<OrderBookModel, Task> onMessageReceivedFunc,
            int levels = 10,
            bool activateFastReceive = false,
            CancellationToken cancellationToken = default);

        #endregion

        #region Spot Account/Trade

        /// <summary>
        ///     Создать новый лимитный ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<FullOrderResponseModel> CreateNewLimitOrderAsync(
            string symbol,
            string sideType,
            string forceType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый рыночный ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<FullOrderResponseModel> CreateNewMarketOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый стоп-лосс ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<FullOrderResponseModel> CreateNewStopLossOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый лимитный стоп-лосс ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<FullOrderResponseModel> CreateNewStopLossLimitOrderAsync(
            string symbol,
            string sideType,
            string forceType,
            double price,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый TakeProfit ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<FullOrderResponseModel> CreateNewTakeProfitOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый TakeProfitLimit ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<FullOrderResponseModel> CreateNewTakeProfitLimitOrderAsync(
            string symbol,
            string sideType,
            string forceType,
            double price,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый LimitMaker ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<FullOrderResponseModel> CreateNewLimitMakerOrderAsync(
            string symbol,
            string sideType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отменяет ордер по паре
        /// </summary>
        Task<CancelOrderResponseModel> CancelOrderAsync(
            string symbol,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отменяет все ордера по паре
        /// </summary>
        Task<IEnumerable<CancelOrderResponseModel>> CancelAllOrdersAsync(
            string symbol,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Проверить состояние ордера по паре
        /// </summary>
        Task<CheckOrderResponseModel> CheckOrderAsync(
            string symbol,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Проверить сосотояние всех открытов ордеров (или ордеров по паре)
        /// </summary>
        /// <param name="symbol"> Возможно null </param>
        Task<IEnumerable<CheckOrderResponseModel>> CheckAllOpenOrdersAsync(
            string symbol,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить все ордера по паре
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="orderId"> Будут возвращены ордера >= orderId </param>
        /// <remarks>
        ///     Если указаны startTime и/или endTime, orderId не требуется
        /// </remarks>
        Task<IEnumerable<CheckOrderResponseModel>> GetAllOrdersAsync(
            string symbol,
            long? orderId = null,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        #endregion
    }
}