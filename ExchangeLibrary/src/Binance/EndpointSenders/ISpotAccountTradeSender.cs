using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Models.SpotAccountTrade;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders
{
    /// <summary>
    ///     Отвечает за отправку запросов к конечным точкам Spot Account/Trade
    /// </summary>
    internal interface ISpotAccountTradeSender
    {
        /// <summary>
        ///     Отправить новый ТЕСТОВЫЙ ордер
        /// </summary>  
        /// <param name="symbol"> Пара </param>
        /// <param name="sideType"> Тип ордера </param>
        /// <param name="orderType"> Тп ордера </param>
        /// <param name="timeInForce"> По умолчанию GTC </param>
        /// <param name="price"> Цена </param>
        /// <param name="quantity"> Кол-во</param>
        /// <param name="stopPrice"> Стоп-цена, если тип ордера STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, или TAKE_PROFIT_LIMIT </param>
        /// <param name="icebergQty"> Кол-во для ордера-айсберга, если тип ордера LIMIT, STOP_LOSS_LIMIT, или TAKE_PROFIT_LIMIT </param>
        /// <param name="recvWindow"> Кол-во миллисекунд, которое прибавляется к timestamp и формирует окно действия запроса </param>
        /// <param name="orderResponseType"> Информация возврата, если удалось создать ордер. Допустимые значения ACK, RESULT, или FULL, по умолчанию RESULT </param>
        Task<NewOrderModelBase> SendNewTestOrderAsync<T>(
            string symbol,
            OrderSideType sideType,
            OrderType orderType,
            TimeInForceType timeInForce = TimeInForceType.GTC,
            double? price = null,
            double? quantity = null,
            double? stopPrice = null,
            double? icebergQty = null,
            double recvWindow = 5000,
            OrderResponseType orderResponseType = OrderResponseType.RESULT,
            CancellationToken cancellationToken = default)
            where T : NewOrderModelBase;

        /// <summary>
        ///     Отправить новый ордер
        /// </summary>  
        /// <param name="symbol"> Пара </param>
        /// <param name="sideType"> Тип ордера </param>
        /// <param name="orderType"> Тп ордера </param>
        /// <param name="timeInForce"> По умолчанию GTC </param>
        /// <param name="price"> Цена </param>
        /// <param name="quantity"> Кол-во</param>
        /// <param name="stopPrice"> Стоп-цена, если тип ордера STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, или TAKE_PROFIT_LIMIT </param>
        /// <param name="icebergQty"> Кол-во для ордера-айсберга, если тип ордера LIMIT, STOP_LOSS_LIMIT, или TAKE_PROFIT_LIMIT </param>
        /// <param name="recvWindow"> Кол-во миллисекунд, которое прибавляется к timestamp и формирует окно действия запроса </param>
        /// <param name="orderResponseType"> Информация возврата, если удалось создать ордер. Допустимые значения ACK, RESULT, или FULL, по умолчанию RESULT </param>
        Task<NewOrderModelBase> SendNewOrderAsync<T>(
            string symbol,
            OrderSideType sideType,
            OrderType orderType,
            TimeInForceType timeInForce = TimeInForceType.GTC,
            double? price = null,
            double? quantity = null,
            double? stopPrice = null,
            double? icebergQty = null,
            double recvWindow = 5000,
            OrderResponseType orderResponseType = OrderResponseType.RESULT,
            CancellationToken cancellationToken = default)
            where T : NewOrderModelBase;
    }
}
