using Common.Enums;
using Common.Models;
using Common.WebSocket;
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
        ///     Получить всю информацию об объектах торговли
        /// </summary>
        Task<IEnumerable<TradeObject>> GetAllTradeObjectInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить информацию о таксе за все объекты торговли или за определенный
        /// </summary>
        /// <param name="name"> Объект торговли </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync(string name = null, long recvWindow = 5000, CancellationToken cancellationToken = default);

        #endregion

        #region Marketdata

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
        Task<OrderBookModel> GetOrderBookAsync(string name, int limit = 100, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последние сделки по паре
        /// </summary>
        Task<IEnumerable<TradeModel>> GetRecentTradesAsync(string name, int limit = 500, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает исторические сделки по объекту торговли
        /// </summary>
        /// <param name="name"> Объект торговли  </param>
        /// <param name="fromId"> Идентификатор сделки для получения. По умолчанию получают самые последние сделки </param>
        /// <param name="limit"> Кол-во сделок (максимум 1000, по умолчанию 500) </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<IEnumerable<TradeModel>> GetOldTradesAsync(string name, long? fromId = null, int limit = 500, CancellationToken cancellationToken = default);

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
        Task<double> GetAveragePriceAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает 24 статистику о цене для объекта торговли или объектовр, если <code><paramref name="name" /> = null or ""</code>
        /// </summary>
        Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает последнюю цену для пары или для всех объектов торговли, если <code><paramref name="name" /> = null or ""</code>
        /// </summary>
        Task<IEnumerable<TradeObjectNamePriceModel>> GetSymbolPriceTickerAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает лучшую цену/количество в стакане для объекта торговли или объектов
        /// </summary>
        Task<IEnumerable<BestSymbolOrderModel>> GetBestSymbolOrdersAsync(string name, CancellationToken cancellationToken = default);

        #endregion

        #region Marketdata Streams

        /// <summary>
        ///     Подписывается на стрим данных
        /// </summary>
        /// <typeparam name="T"> Объект для работы с данными полученными со стрима </typeparam>
        /// <param name="name"> Пара </param>
        /// <param name="streamType"> Тип стрима </param>
        /// <remarks> 
        ///     Возможные значения стримов для Binance:
        ///     <br/>
        ///     @aggTrade - торговая информация для одного ордера тейкера (Модель <see cref="AggregateTradeStreamModel"/>)
        ///     <br/>
        ///     @bookTicker - лучшая цена, количество для указанного объекта торговли (Модель <see cref="BookTickerStreamModel"/>)
        ///     <br/>
        ///     @miniTicker - выборка информации о статистике бегущего окна за 24 часа для объекта торговли (Модель <see cref="MiniTickerStreamModel"/>)
        ///     <br/>
        ///     @ticker - информация о статистике бегущего окна за 24 часа для объекта торговли (Модель <see cref="TradeObjectStreamModel"/>)
        ///     <br/>
        ///     @trade - информация о торговле объектом (Модель <see cref="TradeStreamModel"/>)
        /// </remarks>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="T"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeNewStream<T>(
            string name,
            string streamType,
            Func<T, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим данных по свечам для опред пары
        /// </summary>
        /// <param name="name"> Наименование объекта торговли </param>
        /// <param name="candleStickInterval"> Интервал свечей </param>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="CandlestickStreamModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeCandlestickStream(
            string name,
            string candleStickInterval,
            Func<CandlestickStreamModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим статистики всех мини-тикеров за 24 часа
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="TradeObjectStreamModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeAllMarketTickersStream(
            Func<IEnumerable<TradeObjectStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим обновлений лучшей цены покупки или продажи или количество
        ///     в режиме реального времени для всех символов
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="BookTickerStreamModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeAllBookTickersStream(
            Func<BookTickerStreamModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим статистики всех мини-тикеров за 24 часа
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатывающая данные объекта <see cref="MiniTickerStreamModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeAllMarketMiniTickersStream(
            Func<IEnumerable<MiniTickerStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим лучших ордеров спроса и предложений
        /// </summary>
        /// <param name="name"> Название объекта торговли </param>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="OrderBookModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        /// <param name="levels"> Кол-во ордеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        IWebSocket SubscribePartialBookDepthStream(
            string name,
            Func<OrderBookModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null,
            int levels = 10,
            bool activateFastReceive = false);

        #endregion

        #region Spot Account/Trade

        /// <summary>
        ///     Создать новый лимитный ордер
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="sideType"> Купить или продать </param>
        /// <param name="timeOnForceType">
        ///     Время активности ордера
        /// <remarks> 
        ///     Возможные значения для Binance:
        ///     <br/>
        ///     GTC - Good Til Canceled - ордер будет висеть до тех пор, пока его не отменят (по-умолчанию)
        ///     <br/>
        ///     IOC - Immediate Or Cancel - будет куплено то количество, которое можно купить немедленно. Все, что не удалось купить, будет отменено
        ///     <br/>
        ///     FOK - Fill or Kill - либо будет куплено все указанное количество немедленно, либо не будет куплено вообще ничего, ордер отменится
        /// </remarks>
        /// </param>
        /// <param name="price"> Цена </param>
        /// <param name="quantity"> Количество (объем) </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="isTest"> Тестовый ли запрос </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<FullOrderResponseModel> CreateNewLimitOrderAsync(
            string name,
            OrderSideType sideType,
            string timeOnForceType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый рыночный ордер
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="sideType"> Купить или продать </param>
        /// <param name="quantity"> Количество (объем) </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="isTest"> Тестовый ли запрос </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<FullOrderResponseModel> CreateNewMarketOrderAsync(
            string name,
            OrderSideType sideType,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый стоп-лосс ордер
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="sideType"> Купить или продать </param>
        /// <param name="quantity"> Количество (объем) </param>
        /// <param name="stopPrice"> Стоп цена </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="isTest"> Тестовый ли запрос </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<FullOrderResponseModel> CreateNewStopLossOrderAsync(
            string name,
            OrderSideType sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый лимитный стоп-лосс ордер
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="sideType"> Купить или продать </param>
        /// <param name="timeOnForceType">
        ///     Время активности ордера
        /// <remarks> 
        ///     Возможные значения для Binance:
        ///     <br/>
        ///     GTC - Good Til Canceled - ордер будет висеть до тех пор, пока его не отменят (по-умолчанию)
        ///     <br/>
        ///     IOC - Immediate Or Cancel - будет куплено то количество, которое можно купить немедленно. Все, что не удалось купить, будет отменено
        ///     <br/>
        ///     FOK - Fill or Kill - либо будет куплено все указанное количество немедленно, либо не будет куплено вообще ничего, ордер отменится
        /// </remarks>
        /// </param>
        /// <param name="price"> Цена </param>
        /// <param name="quantity"> Количество (объем) </param>
        /// <param name="stopPrice"> Стоп цена </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="isTest"> Тестовый ли запрос </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<FullOrderResponseModel> CreateNewStopLossLimitOrderAsync(
            string name,
            OrderSideType sideType,
            string timeOnForceType,
            double price,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый TakeProfit ордер
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="quantity"> Количество (объем) </param>
        /// <param name="sideType"> Купить или продать </param>
        /// <param name="stopPrice"> Стоп цена </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="isTest"> Тестовый ли запрос </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<FullOrderResponseModel> CreateNewTakeProfitOrderAsync(
            string name,
            OrderSideType sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый TakeProfitLimit ордер
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="quantity"> Количество (объем) </param>
        /// <param name="sideType"> Купить или продать </param>
        /// <param name="timeOnForceType">
        ///     Время активности ордера
        /// <remarks> 
        ///     Возможные значения для Binance:
        ///     <br/>
        ///     GTC - Good Til Canceled - ордер будет висеть до тех пор, пока его не отменят (по-умолчанию)
        ///     <br/>
        ///     IOC - Immediate Or Cancel - будет куплено то количество, которое можно купить немедленно. Все, что не удалось купить, будет отменено
        ///     <br/>
        ///     FOK - Fill or Kill - либо будет куплено все указанное количество немедленно, либо не будет куплено вообще ничего, ордер отменится
        /// </remarks>
        /// </param>
        /// <param name="price"> Цена </param>
        /// <param name="stopPrice"> Стоп цена </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="isTest"> Тестовый ли запрос </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<FullOrderResponseModel> CreateNewTakeProfitLimitOrderAsync(
            string name,
            OrderSideType sideType,
            string timeOnForceType,
            double price,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать новый LimitMaker ордер
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="quantity"> Количество (объем) </param>
        /// <param name="price"> Цена </param>
        /// <param name="sideType"> Купить или продать </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="isTest"> Тестовый ли запрос </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<FullOrderResponseModel> CreateNewLimitMakerOrderAsync(
            string name,
            OrderSideType sideType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отменяет ордер по объекту торговли
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="orderId"> Id ордера из системы </param>
        /// <param name="origClientOrderId"> Id ордера, который указывал пользователь </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<CancelOrderResponseModel> CancelOrderAsync(
            string name,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отменяет все ордера по объекту торговли
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<IEnumerable<CancelOrderResponseModel>> CancelAllOrdersAsync(
            string name,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Проверить состояние ордера по объекту торговли
        /// </summary>
        /// <param name="name"> Наименование объекта </param>
        /// <param name="orderId"> Id ордера из системы </param>
        /// <param name="origClientOrderId"> Id ордера, который указывал пользователь </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<CheckOrderResponseModel> CheckOrderAsync(
            string name,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Проверить сосотояние всех открытов ордеров (или ордеров по паре)
        /// </summary>
        /// <param name="name"> Наименование объекта возможно null </param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<IEnumerable<CheckOrderResponseModel>> CheckAllOpenOrdersAsync(
            string name,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить все ордера по объекту торговли
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orderId"> Будут возвращены ордера >= orderId </param>
        /// <remarks>
        ///     Если указаны startTime и/или endTime, orderId не требуется
        /// </remarks>
        /// <param name="startTime"> Время начала поиска ордеров </param>
        /// <param name="endTime"> Время окончания поиска ордеров </param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"> 
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp
        ///     и формирует окно действия запроса
        /// </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<IEnumerable<CheckOrderResponseModel>> GetAllOrdersAsync(
            string name,
            long? orderId = null,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить информацию об аккаунте
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<AccountInformationModel> GetAccountInformationAsync(CancellationToken cancellationToken);

        #endregion
    }
}
