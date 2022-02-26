using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Строитель запрсов новых ордеров
    /// </summary>
    internal class Builder : IBuilder
    {
        #region Fields

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private OrderQueryModel _newQueryModel;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Builder"/>
        public Builder() => _newQueryModel = new OrderQueryModel();

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void Reset() => _newQueryModel = new OrderQueryModel();

        /// <inheritdoc />
        public void SetPrice(double price)
        {
            if (price <= 0)
            {
                Log.Warn($"Failed to set price with value={price}");

                return;
            }

            _newQueryModel.Price.IsUse = true;
            _newQueryModel.Price.ValueStr = price.ToString();
        }

        /// <inheritdoc />
        public void SetQuantity(double quantity)
        {
            if (quantity <= 0)
            {
                Log.Warn($"Failed to set quantity with value={quantity}");

                return;
            }

            _newQueryModel.Quantity.IsUse = true;
            _newQueryModel.Quantity.ValueStr = quantity.ToString();
        }

        /// <inheritdoc />
        public void SetOrderId(long orderId)
        {
            if (orderId <= 0)
            {
                Log.Warn($"Failed to set orderId with value={orderId}");

                return;
            }

            _newQueryModel.OrderId.IsUse = true;
            _newQueryModel.OrderId.ValueStr = orderId.ToString();
        }

        /// <inheritdoc />
        public void SetStartTime(long startTime)
        {
            if (startTime <= 0)
            {
                Log.Warn($"Failed to set startTime with value={startTime}");

                return;
            }

            _newQueryModel.StartTime.IsUse = true;
            _newQueryModel.StartTime.ValueStr = startTime.ToString();
        }

        /// <inheritdoc />
        public void SetCandlestickInterval(string candlestickInterval)
        {
            // выдает ошибку если неверное значение
            var interval = candlestickInterval.ConvertToCandleStickIntervalType();

            _newQueryModel.CandlestickInterval.IsUse = true;
            _newQueryModel.CandlestickInterval.ValueStr = interval.ToUrl();
        }

        /// <inheritdoc />
        public void SetCandlestickInterval(CandlestickIntervalType candlestickInterval)
        {
            _newQueryModel.CandlestickInterval.IsUse = true;
            _newQueryModel.CandlestickInterval.ValueStr = candlestickInterval.ToUrl();
        }

        /// <inheritdoc />
        public void SetEndTime(long endTime)
        {
            if (endTime <= 0 || !_newQueryModel.StartTime.IsUse)
            {
                Log.Warn($"Failed to set endTime with value={endTime}");

                return;
            }

            _newQueryModel.EndTime.IsUse = true;
            _newQueryModel.EndTime.ValueStr = endTime.ToString();
        }

        /// <inheritdoc />
        public void SetLimit(int limit)
        {
            if (limit <= 0)
            {
                Log.Warn($"Failed to set limit with value={limit}");

                return;
            }

            _newQueryModel.Limit.IsUse = true;
            _newQueryModel.Limit.ValueStr = limit.ToString();
        }

        /// <inheritdoc />
        public void SetFromId(long fromId)
        {
            if (fromId <= 0)
            {
                Log.Warn($"Failed to set fromId with value={fromId}");

                return;
            }

            _newQueryModel.FromId.IsUse = true;
            _newQueryModel.FromId.ValueStr = fromId.ToString();
        }

        /// <inheritdoc />
        public void SetOrigClientOrderId(string origClientOrderId)
        {
            if (string.IsNullOrEmpty(origClientOrderId))
            {
                Log.Warn($"Failed to set origClientOrderId with value={origClientOrderId}");

                return;
            }

            _newQueryModel.OrigClientOrderId.IsUse = true;
            _newQueryModel.OrigClientOrderId.ValueStr = origClientOrderId;
        }

        /// <inheritdoc />
        public void SetIcebergQuantity(double icebergQty)
        {
            if (icebergQty <= 0 || !_newQueryModel.IcebergQty.CanSet)
            {
                Log.Warn($"Failed to set icebergQty with value={icebergQty}");

                return;
            }

            _newQueryModel.IcebergQty.IsUse = true;
            _newQueryModel.IcebergQty.ValueStr = icebergQty.ToString();

            SetTimeInForce(TimeInForceType.GTC);
            _newQueryModel.TimeInForce.CanSet = false;
        }

        /// <inheritdoc />
        public void SetOrderResponseType(OrderResponseType orderResponseType)
        {
            if (!_newQueryModel.OrderResponseType.CanSet)
            {
                Log.Warn($"Failed to set OrderResponseType with value={orderResponseType}, because it is not allowed");

                return;
            }

            _newQueryModel.OrderResponseType.IsUse = true;
            _newQueryModel.OrderResponseType.ValueStr = orderResponseType.ToUrl();
        }

        /// <inheritdoc />
        public void SetOrderSideType(OrderSideType sideType)
        {
            if (!_newQueryModel.SideType.CanSet)
            {
                Log.Warn($"Failed to set OrderSideType with value={sideType}, because it is not allowed");

                return;
            }

            _newQueryModel.SideType.IsUse = true;
            _newQueryModel.SideType.ValueStr = sideType.ToUrl();
        }

        /// <inheritdoc />
        public void SetOrderSideType(string side)
        {
            var sideType = side.ConvertToOrderSideType();
            if (!_newQueryModel.SideType.CanSet)
            {
                Log.Warn($"Failed to set OrderSideType with value={sideType}, because it is not allowed");

                return;
            }

            _newQueryModel.SideType.IsUse = true;
            _newQueryModel.SideType.ValueStr = sideType.ToUrl();
        }

        /// <inheritdoc />
        public void SetOrderType(string order)
        {
            var orderType = order.ConvertToOrderType();
            SetOrderResponseType(orderType);
            SetStopPrice(orderType);
            SetIcebergQty(orderType);

            _newQueryModel.OrderType.IsUse = true;
            _newQueryModel.OrderType.ValueStr = orderType.ToUrl();
        }

        /// <inheritdoc />
        public void SetOrderType(OrderType orderType)
        {
            SetOrderResponseType(orderType);
            SetStopPrice(orderType);
            SetIcebergQty(orderType);

            _newQueryModel.OrderType.IsUse = true;
            _newQueryModel.OrderType.ValueStr = orderType.ToUrl();
        }

        /// <inheritdoc />
        public void SetRecvWindow(long recvWindow)
        {
            _newQueryModel.RecvWindow.IsUse = true;
            _newQueryModel.RecvWindow.ValueStr = recvWindow.ToString();
        }

        /// <inheritdoc />
        public void SetStopPrice(double stopPrice)
        {
            if (stopPrice <= 0 || !_newQueryModel.StopPrice.CanSet)
            {
                Log.Warn($"Failed to set stop price with value={stopPrice}");

                return;
            }

            _newQueryModel.StopPrice.IsUse = true;
            _newQueryModel.StopPrice.ValueStr = stopPrice.ToString();
        }

        /// <inheritdoc />
        public void SetSymbol(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            _newQueryModel.Symbol.IsUse = true;
            _newQueryModel.Symbol.ValueStr = symbol;
        }

        /// <inheritdoc />
        public void SetTimeInForce(string timeInForce)
        {
            var timeInForceType = timeInForce.ConvertToTimeInForceType();
            if (!_newQueryModel.TimeInForce.CanSet)
            {
                Log.Warn($"Failed to set time in force, because it is not allowed");

                return;
            }

            _newQueryModel.TimeInForce.IsUse = true;
            _newQueryModel.TimeInForce.ValueStr = timeInForceType.ToUrl();
        }

        /// <inheritdoc />
        public void SetTimeInForce(TimeInForceType timeInForce)
        {
            if (!_newQueryModel.TimeInForce.CanSet)
            {
                Log.Warn($"Failed to set time in force, because it is not allowed");

                return;
            }

            _newQueryModel.TimeInForce.IsUse = true;
            _newQueryModel.TimeInForce.ValueStr = timeInForce.ToUrl();
        }

        /// <summary>
        ///     Установить текущее время отправки
        /// </summary>
        public void SetTimeStamp()
        {
            _newQueryModel.TimeStamp.IsUse = true;
            _newQueryModel.TimeStamp.ValueStr = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        }

        /// <summary>
        ///     Возвращает результат работы строителя
        /// </summary>
        public OrderQueryModel GetResult()
        {
            // Принудительно ставлю полный ответ на запрос
            SetOrderResponseType(OrderResponseType.Full);
            SetTimeStamp();
            return _newQueryModel;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Устанавливает <see cref="OrderResponseType"/> в зависимости от <see cref="OrderType"/>
        /// </summary>
        private void SetOrderResponseType(OrderType orderType)
        {
            if (orderType == OrderType.Limit || orderType == OrderType.Market)
            {
                SetOrderResponseType(OrderResponseType.Full);
                _newQueryModel.OrderResponseType.CanSet = false;
                return;
            }

            _newQueryModel.OrderResponseType.CanSet = true;
        }

        /// <summary>
        ///     Устанавливает разрешения на запись <see cref="OrderQueryModel.StopPrice"/> в зависимости от <see cref="OrderType"/>
        /// </summary>
        private void SetStopPrice(OrderType orderType)
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
                return;
            }

            _newQueryModel.StopPrice.CanSet = true;
        }

        /// <summary>
        ///     Устанавливает разрешения на запись <see cref="OrderQueryModel.IcebergQty"/> в зависимости от <see cref="OrderType"/>
        /// </summary>
        private void SetIcebergQty(OrderType orderType)
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
                return;
            }

            _newQueryModel.IcebergQty.CanSet = true;
        }

        #endregion
    }
}
