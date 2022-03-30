using Common.Enums;
using Common.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary
{
    /// <summary>
    ///     Управляет торговлей
    /// </summary>
    public interface ISpotTrade
    {
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
    }
}
