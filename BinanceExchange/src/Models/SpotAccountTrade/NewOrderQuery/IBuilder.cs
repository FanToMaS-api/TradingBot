using BinanceExchange.Enums;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Интерфйес строителя запросов по созданию новых ордеров
    /// </summary>
    internal interface IBuilder
    {
        /// <summary>
        ///     Сброс строителя
        /// </summary>
        void Reset();

        /// <summary>
        ///     Установить пару
        /// </summary>
        void SetSymbol(string symbol);

        /// <summary>
        ///     Установить цену
        /// </summary>
        void SetPrice(double price);

        /// <summary>
        ///     Установить кол-во
        /// </summary>
        void SetQuantity(double quantity);

        /// <summary>
        ///     Установить id ордера (для отмены)
        /// </summary>
        void SetOrderId(long orderId);

        /// <summary>
        ///     Установить время начала построения свечей (для выгрузки)
        /// </summary>
        void SetStartTime(long startTime);

        /// <summary>
        ///     Установить интервал свечи
        /// </summary>
        void SetCandlestickInterval(string candlestickIntervalType);

        /// <summary>
        ///     Установить интервал свечи
        /// </summary>
        void SetCandlestickInterval(CandlestickIntervalType candlestickInterval);

        /// <summary>
        ///     Установить окончание периода построения свечей (для выгрузки)
        /// </summary>
        void SetEndTime(long endTime);

        /// <summary>
        ///     Установить глубину запроса (лимит выдачи данных)
        /// </summary>
        void SetLimit(int limit);

        /// <summary>
        ///     Установить нижнюю границу по id для выгрузки данных
        /// </summary>
        void SetFromId(long fromId);

        /// <summary>
        ///     Установить Идентификатор заказа клиента
        /// </summary>
        void SetOrigClientOrderId(string origClientOrderId);

        /// <summary>
        ///     Установить стоп цену
        /// </summary>
        void SetStopPrice(double stopPrice);

        /// <summary>
        ///     Установить тип ордера
        /// </summary>
        void SetOrderType(string orderType);

        /// <summary>
        ///     Установить тип ордера
        /// </summary>
        void SetOrderType(OrderType orderType);

        /// <summary>
        ///     Установить тип ордера (покупка, продажа)
        /// </summary>
        void SetOrderSideType(string sideType);

        /// <summary>
        ///     Установить тип ордера (покупка, продажа)
        /// </summary>
        void SetOrderSideType(OrderSideType sideType);

        /// <summary>
        ///     Установить кол-во для айсберг-ордера
        /// </summary>
        void SetIcebergQuantity(double icebergQty);

        /// <summary>
        ///     Установить сколько ордер будет активен
        /// </summary>
        void SetTimeInForce(TimeInForceType timeInForce);

        /// <summary>
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp и формирует окно действия запроса
        /// </summary>
        void SetRecvWindow(long recvWindow);

        /// <summary>
        ///     Установить формат ответа сервера на запрос
        /// </summary>
        void SetOrderResponseType(OrderResponseType orderResponseType);
    }
}
