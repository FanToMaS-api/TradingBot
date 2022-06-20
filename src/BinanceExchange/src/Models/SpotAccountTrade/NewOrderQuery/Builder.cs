using BinanceExchange.Enums;
using BinanceExchange.Enums.Helper;
using Common.Enums;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Строитель запросов новых ордеров
    /// </summary>
    internal class Builder
    {
        #region Fields

        private static readonly ILoggerDecorator Log = LoggerManager.CreateDefaultLogger();
        private OrderQueryModel _newQueryModel;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Builder"/>
        public Builder() => _newQueryModel = new OrderQueryModel();

        #endregion

        #region Public methods

        /// <summary>
        ///     Сброс строителя
        /// </summary>
        public Builder Reset()
        {
            _newQueryModel = new OrderQueryModel();

            return this;
        }

        /// <summary>
        ///     Установить цену
        /// </summary>
        public Builder SetPrice(double price)
        {
            if (price <= 0)
            {
                throw new Exception($"Failed to set price with value={price}");
            }

            _newQueryModel.Price.IsUse = true;
            _newQueryModel.Price.ValueStr = price.ToString();

            return this;
        }

        /// <summary>
        ///     Установить кол-во
        /// </summary>
        public Builder SetQuantity(double quantity)
        {
            if (quantity <= 0)
            {
                throw new Exception($"Failed to set quantity with value={quantity}");
            }

            _newQueryModel.Quantity.IsUse = true;
            _newQueryModel.Quantity.ValueStr = quantity.ToString();

            return this;
        }

        /// <summary>
        ///     Установить id ордера (для отмены)
        /// </summary>
        public Builder SetOrderId(long orderId)
        {
            if (orderId <= 0)
            {
                throw new Exception($"Failed to set orderId with value={orderId}");
            }

            _newQueryModel.OrderId.IsUse = true;
            _newQueryModel.OrderId.ValueStr = orderId.ToString();

            return this;
        }

        /// <summary>
        ///     Установить время начала построения свечей (для выгрузки)
        /// </summary>
        public Builder SetStartTime(long startTime)
        {
            if (startTime <= 0)
            {
                throw new Exception($"Failed to set startTime with value={startTime}");
            }

            _newQueryModel.StartTime.IsUse = true;
            _newQueryModel.StartTime.ValueStr = startTime.ToString();

            return this;
        }

        /// <summary>
        ///     Установить интервал свечи
        /// </summary>
        public Builder SetCandlestickInterval(string candlestickInterval)
        {
            // выдает ошибку если неверное значение
            var interval = candlestickInterval.ConvertToCandleStickIntervalType();

            _newQueryModel.CandlestickInterval.IsUse = true;
            _newQueryModel.CandlestickInterval.ValueStr = interval.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить интервал свечи
        /// </summary>
        public Builder SetCandlestickInterval(CandlestickIntervalType candlestickInterval)
        {
            _newQueryModel.CandlestickInterval.IsUse = true;
            _newQueryModel.CandlestickInterval.ValueStr = candlestickInterval.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить окончание периода построения свечей (для выгрузки)
        /// </summary>
        public Builder SetEndTime(long endTime)
        {
            if (endTime <= 0 || !_newQueryModel.StartTime.IsUse)
            {
                throw new Exception($"Failed to set endTime with value={endTime}");
            }

            _newQueryModel.EndTime.IsUse = true;
            _newQueryModel.EndTime.ValueStr = endTime.ToString();

            return this;
        }

        /// <summary>
        ///     Установить глубину запроса (лимит выдачи данных)
        /// </summary>
        public Builder SetLimit(int limit)
        {
            if (limit <= 0)
            {
                throw new Exception($"Failed to set limit with value={limit}");
            }

            _newQueryModel.Limit.IsUse = true;
            _newQueryModel.Limit.ValueStr = limit.ToString();

            return this;
        }

        /// <summary>
        ///     Установить нижнюю границу по id для выгрузки данных
        /// </summary>
        public Builder SetFromId(long fromId)
        {
            if (fromId <= 0)
            {
                throw new Exception($"Failed to set fromId with value={fromId}");
            }

            _newQueryModel.FromId.IsUse = true;
            _newQueryModel.FromId.ValueStr = fromId.ToString();

            return this;
        }

        /// <summary>
        ///     Установить Идентификатор заказа клиента
        /// </summary>
        public Builder SetOrigClientOrderId(string origClientOrderId)
        {
            _newQueryModel.OrigClientOrderId.IsUse = true;
            _newQueryModel.OrigClientOrderId.ValueStr = origClientOrderId;

            return this;
        }

        /// <summary>
        ///     Установить кол-во для айсберг-ордера
        /// </summary>
        public Builder SetIcebergQuantity(double icebergQty)
        {
            if (icebergQty <= 0 || !_newQueryModel.IcebergQty.CanSet)
            {
                Log.WarnAsync($"Failed to set icebergQty with value={icebergQty}");

                return this;
            }

            _newQueryModel.IcebergQty.IsUse = true;
            _newQueryModel.IcebergQty.ValueStr = icebergQty.ToString();

            SetTimeInForce(TimeInForceType.GTC);
            _newQueryModel.TimeInForce.CanSet = false;

            return this;
        }

        /// <summary>
        ///     Установить формат ответа сервера на запрос
        /// </summary>
        public Builder SetOrderResponseType(OrderResponseType orderResponseType)
        {
            if (!_newQueryModel.OrderResponseType.CanSet)
            {
                Log.WarnAsync($"Failed to set OrderResponseType with value={orderResponseType}, because it is not allowed");

                return this;
            }

            _newQueryModel.OrderResponseType.IsUse = true;
            _newQueryModel.OrderResponseType.ValueStr = orderResponseType.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить тип ордера (покупка, продажа)
        /// </summary>
        public Builder SetOrderSideType(OrderSideType sideType)
        {
            if (!_newQueryModel.SideType.CanSet)
            {
                Log.WarnAsync($"Failed to set OrderSideType with value={sideType}, because it is not allowed");

                return this;
            }

            _newQueryModel.SideType.IsUse = true;
            _newQueryModel.SideType.ValueStr = sideType.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить тип ордера (покупка, продажа)
        /// </summary>
        public Builder SetOrderSideType(string side)
        {
            var sideType = side.ConvertToOrderSideType();
            if (!_newQueryModel.SideType.CanSet)
            {
                Log.WarnAsync($"Failed to set OrderSideType with value={sideType}, because it is not allowed");

                return this;
            }

            _newQueryModel.SideType.IsUse = true;
            _newQueryModel.SideType.ValueStr = sideType.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить тип ордера
        /// </summary>
        public Builder SetOrderType(string order)
        {
            var orderType = order.ConvertToOrderType();

            // специальный хардкод, для получения полной информации
            SetOrderResponseType(OrderResponseType.Full);
            SetStopPrice(orderType);
            SetIcebergQty(orderType);

            _newQueryModel.OrderType.IsUse = true;
            _newQueryModel.OrderType.ValueStr = orderType.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить тип ордера
        /// </summary>
        public Builder SetOrderType(OrderType orderType)
        {
            // специальный хардкод, для получения полной информации
            SetOrderResponseType(OrderResponseType.Full);
            SetStopPrice(orderType);
            SetIcebergQty(orderType);

            _newQueryModel.OrderType.IsUse = true;
            _newQueryModel.OrderType.ValueStr = orderType.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить кол-во миллисекунд, которое прибавляется к timestamp и формирует окно действия запроса
        /// </summary>
        public Builder SetRecvWindow(long recvWindow)
        {
            _newQueryModel.RecvWindow.IsUse = true;
            _newQueryModel.RecvWindow.ValueStr = recvWindow.ToString();

            return this;
        }

        /// <summary>
        ///     Установить стоп цену
        /// </summary>
        public Builder SetStopPrice(double stopPrice)
        {
            if (stopPrice <= 0 || !_newQueryModel.StopPrice.CanSet)
            {
                Log.WarnAsync($"Failed to set stop price with value={stopPrice}");

                return this;
            }

            _newQueryModel.StopPrice.IsUse = true;
            _newQueryModel.StopPrice.ValueStr = stopPrice.ToString();

            return this;
        }

        /// <summary>
        ///     Установить пару
        /// </summary>
        public Builder SetSymbol(string symbol, bool canBeNull = false)
        {
            if (!canBeNull && string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            _newQueryModel.Symbol.IsUse = true;
            _newQueryModel.Symbol.ValueStr = symbol;

            return this;
        }

        /// <summary>
        ///     Установить сколько ордер будет активен
        /// </summary>
        public Builder SetTimeInForce(string timeInForce)
        {
            var timeInForceType = timeInForce.ConvertToTimeInForceType();
            if (!_newQueryModel.TimeInForce.CanSet)
            {
                Log.WarnAsync($"Failed to set time in force, because it is not allowed");

                return this;
            }

            _newQueryModel.TimeInForce.IsUse = true;
            _newQueryModel.TimeInForce.ValueStr = timeInForceType.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить сколько ордер будет активен
        /// </summary>
        public Builder SetTimeInForce(TimeInForceType timeInForce)
        {
            if (!_newQueryModel.TimeInForce.CanSet)
            {
                Log.WarnAsync($"Failed to set time in force, because it is not allowed");

                return this;
            }

            _newQueryModel.TimeInForce.IsUse = true;
            _newQueryModel.TimeInForce.ValueStr = timeInForce.ToUrl();

            return this;
        }

        /// <summary>
        ///     Установить текущее время отправки
        /// </summary>
        public Builder SetTimeStamp()
        {
            _newQueryModel.TimeStamp.IsUse = true;
            _newQueryModel.TimeStamp.ValueStr = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            return this;
        }

        /// <summary>
        ///     Возвращает результат работы строителя
        /// </summary>
        /// <param name="setTimeStamp"> Устанавливать ли время запроса </param>
        public OrderQueryModel GetResult(bool setTimeStamp = true)
        {
            if (setTimeStamp)
            {
                SetTimeStamp();
            }

            return _newQueryModel;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Устанавливает <see cref="OrderResponseType"/> в зависимости от <see cref="OrderType"/>
        /// </summary>
        /// <remarks>
        ///     Временно не используется, для получения только полной информации о ордере
        /// </remarks>
        private Builder SetOrderResponseType(OrderType orderType)
        {
            if (orderType is OrderType.Limit or OrderType.Market)
            {
                SetOrderResponseType(OrderResponseType.Full);
                _newQueryModel.OrderResponseType.CanSet = false;

                return this;
            }

            _newQueryModel.OrderResponseType.CanSet = true;

            return this;
        }

        /// <summary>
        ///     Устанавливает разрешения на запись <see cref="OrderQueryModel.StopPrice"/> в зависимости от <see cref="OrderType"/>
        /// </summary>
        private Builder SetStopPrice(OrderType orderType)
        {
            var stopPriceOrderTypes = new List<OrderType>()
            {
                OrderType.StopLoss,
                OrderType.StopLossLimit,
                OrderType.TakeProfit,
                OrderType.TakeProfitLimit
            };
            if (!stopPriceOrderTypes.Any(_ => _ == orderType))
            {
                _newQueryModel.StopPrice.CanSet = false;
                
                return this;
            }

            _newQueryModel.StopPrice.CanSet = true;

            return this;
        }

        /// <summary>
        ///     Устанавливает разрешения на запись <see cref="OrderQueryModel.IcebergQty"/> в зависимости от <see cref="OrderType"/>
        /// </summary>
        private Builder SetIcebergQty(OrderType orderType)
        {
            var icebergQtyOrderTypes = new List<OrderType>()
            {
                OrderType.Limit,
                OrderType.StopLossLimit,
                OrderType.TakeProfitLimit
            };
            if (!icebergQtyOrderTypes.Any(_ => _ == orderType))
            {
                _newQueryModel.IcebergQty.CanSet = false;

                return this;
            }

            _newQueryModel.IcebergQty.CanSet = true;

            return this;
        }

        #endregion
    }
}
