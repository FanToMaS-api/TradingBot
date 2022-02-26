using Common.Models;
using ExchangeLibrary.Binance.Enums;
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

        #endregion

        #region Marketdata

        /// <summary>
        ///     Получить книгу ордеров по определенной паре
        /// </summary>
        /// <param name="limit"> Глубина запроса (кол-во записей) </param>
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
        Task<string> GetCandlstickAsync(
            string symbol,
            string interval,
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
        /// <typeparam name="T"> Объект для работы с данными полученными со стрима </typeparam>
        /// <param name="symbol"> Пара </param>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="{T}"/> </param>
        /// <param name="candleStickInterval"> Интервал свечей </param>
        Task SubscribeCandlestickStreamAsync<T>(
            string symbol,
            string candleStickInterval,
            Func<T, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим статистики всех мини-тикеров за 24 часа
        /// </summary>
        /// <typeparam name="T"> Объект для работы с данными полученными со стрима </typeparam>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="{T}"/> </param>
        Task SubscribeAllMarketTickersStreamAsync<T>(
            Func<T, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим обновлений лучшей цены покупки или продажи или количество
        ///     в режиме реального времени для всех символов
        /// </summary>
        /// <typeparam name="T"> Объект для работы с данными полученными со стрима </typeparam>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="{T}"/> </param>
        Task SubscribeAllBookTickersStreamAsync<T>(
            Func<T, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим статистики всех мини-тикеров за 24 часа
        /// </summary>
        /// <typeparam name="T"> Объект для работы с данными полученными со стрима </typeparam>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="{T}"/> </param>
        Task SubscribeAllMarketMiniTickersStreamAsync<T>(
            Func<T, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Подписывается на стрим лучших ордеров спроса и предложений
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <typeparam name="T"> Объект для работы с данными полученными со стрима </typeparam>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="{T}"/> </param>
        /// <param name="levels"> Кол-во оредеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        Task SubscribePartialBookDepthStreamAsync<T>(
            string symbol,
            Func<T, Task> onMessageReceivedFunc,
            int levels = 10,
            bool activateFastReceive = false,
            CancellationToken cancellationToken = default);

        #endregion

        #region Spot Account/Trade

        /// <summary>
        ///     Создать новый <see cref="OrderType.Limit"/> ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<string> CreateNewLimitOrderAsync(
            string symbol,
            string sideType,
            string forceType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый <see cref="OrderType.Market"/> ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<string> CreateNewMarketOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый <see cref="OrderType.StopLoss"/> ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<string> CreateNewStopLossOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый <see cref="OrderType.StopLossLimit"/> ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<string> CreateNewStopLossLimitOrderAsync(
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
        ///     Создать новый <see cref="OrderType.TakeProfit"/> ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<string> CreateNewTakeProfitOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый <see cref="OrderType.TakeProfitLimit"/> ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<string> CreateNewTakeProfitLimitOrderAsync(
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
        ///     Создать новый <see cref="OrderType.LimitMaker"/> ордер
        /// </summary>
        /// <param name="isTest"> Тестовый ли запрос </param>
        Task<string> CreateNewLimitMakerOrderAsync(
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
        Task<string> CancelOrderAsync(
            string symbol,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отменяет все ордера по паре
        /// </summary>
        Task<string> CancelAllOrdersAsync(
            string symbol,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Проверить состояние ордера по паре
        /// </summary>
        Task<string> CheckOrderAsync(
            string symbol,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Проверить сосотояние всех открытов ордеров (или ордеров по паре)
        /// </summary>
        /// <param name="symbol"> Возможно null </param>
        Task<string> CheckAllOpenOrdersAsync(
            string symbol,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
