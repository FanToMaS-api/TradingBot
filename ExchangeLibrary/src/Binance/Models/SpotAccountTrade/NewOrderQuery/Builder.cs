using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExchangeLibrary.Binance.Models.SpotAccountTrade.NewOrderQuery
{
    /// <summary>
    ///     Строитель запрсоов новых ордеров
    /// </summary>
    internal class Builder : IBuilder
    {
        #region Fields

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private OrderQueryModel _newQueryModel;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void Reset() => _newQueryModel = new OrderQueryModel();

        /// <inheritdoc />
        public void SetPrice(double? price)
        {
            if (price is null || price <= 0)
            {
                Log.Warn($"Failed to set price with value={price}");

                return;
            }

            _newQueryModel.Price = price.Value;
            SetIsUse(nameof(OrderQueryModel.Price), true, price.ToString());
        }

        /// <inheritdoc />
        public void SetQuantity(double? quantity)
        {
            if (quantity is null || quantity <= 0)
            {
                Log.Warn($"Failed to set quantity with value={quantity}");

                return;
            }

            _newQueryModel.Quantity = quantity.Value;
            SetIsUse(nameof(OrderQueryModel.Quantity), true, quantity.ToString());
        }

        /// <inheritdoc />
        public void SetIcebergQuantity(double? icebergQty)
        {
            if (icebergQty is null || icebergQty <= 0 || IsCanSet(nameof(OrderQueryModel.IcebergQty)))
            {
                Log.Warn($"Failed to set icebergQty with value={icebergQty}");

                return;
            }

            _newQueryModel.IcebergQty = icebergQty.Value;
            SetIsUse(nameof(OrderQueryModel.IcebergQty), true, icebergQty.ToString());

            SetTimeInForce(TimeInForceType.GTC);
            SetCanSet(nameof(OrderQueryModel.TimeInForce), false);
        }

        /// <inheritdoc />
        public void SetOrderResponseType(OrderResponseType orderResponseType)
        {
            if (IsCanSet(nameof(OrderQueryModel.OrderResponseType)))
            {
                Log.Warn($"Failed to set OrderResponseType with value={orderResponseType}, because IsCanSet=False");

                return;
            }

            _newQueryModel.OrderResponseType = orderResponseType;
            SetIsUse(nameof(OrderQueryModel.OrderResponseType), true, orderResponseType.ToUrl());
        }

        /// <inheritdoc />
        public void SetOrderSideType(OrderSideType sideType)
        {
            if (IsCanSet(nameof(OrderQueryModel.SideType)))
            {
                Log.Warn($"Failed to set OrderSideType with value={sideType}, because IsCanSet=False");

                return;
            }

            _newQueryModel.SideType = sideType;
            SetIsUse(nameof(OrderQueryModel.SideType), true, sideType.ToUrl());
        }

        /// <inheritdoc />
        public void SetOrderType(OrderType orderType)
        {
            SetOrderResponseType(orderType);
            SetStopPrice(orderType);
            SetIcebergQty(orderType);

            _newQueryModel.OrderType = orderType;
            SetIsUse(nameof(OrderQueryModel.OrderType), true, orderType.ToUrl());
        }

        /// <inheritdoc />
        public void SetRecvWindow(long recvWindow)
        {
            _newQueryModel.RecvWindow = recvWindow;
            SetIsUse(nameof(OrderQueryModel.RecvWindow), true, recvWindow.ToString());
        }

        /// <inheritdoc />
        public void SetStopPrice(double? stopPrice)
        {
            if (stopPrice is null || stopPrice <= 0 || IsCanSet(nameof(OrderQueryModel.StopPrice)))
            {
                Log.Warn($"Failed to set stop price with value={stopPrice}");

                return;
            }

            _newQueryModel.StopPrice = stopPrice.Value;
            SetIsUse(nameof(OrderQueryModel.StopPrice), true, stopPrice.ToString());
        }

        /// <inheritdoc />
        public void SetSymbol(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            _newQueryModel.Symbol = symbol;
            SetIsUse(nameof(OrderQueryModel.Symbol), true, symbol);
        }

        /// <inheritdoc />
        public void SetTimeInForce(TimeInForceType timeInForce)
        {
            if (IsCanSet(nameof(OrderQueryModel.TimeInForce)))
            {
                Log.Warn($"Failed to set time in force, because IsCanSet=False");

                return;
            }

            _newQueryModel.TimeInForce = timeInForce;
            SetIsUse(nameof(OrderQueryModel.TimeInForce), true, timeInForce.ToUrl());
        }

        /// <summary>
        ///     Установить текущее время отправки
        /// </summary>
        public void SetTimeStamp()
        {
            _newQueryModel.TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            SetIsUse(nameof(OrderQueryModel.TimeInForce), true, _newQueryModel.TimeStamp.ToString());
        }

        /// <summary>
        ///     Возвращает результат работы строителя
        /// </summary>
        public OrderQueryModel GetResult()
        {
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
            if (orderType == OrderType.LIMIT || orderType == OrderType.MARKET)
            {
                SetOrderResponseType(OrderResponseType.FULL);
                SetCanSet(nameof(OrderQueryModel.OrderResponseType), false);
                return;
            }

            SetCanSet(nameof(OrderQueryModel.OrderResponseType), true);
        }

        /// <summary>
        ///     Устанавливает разрешения на запись <see cref="OrderQueryModel.StopPrice"/> в зависимости от <see cref="OrderType"/>
        /// </summary>
        private void SetStopPrice(OrderType orderType)
        {
            var stopPriceOrderTypes = new List<OrderType>()
            {
                OrderType.STOP_LOSS,
                OrderType.STOP_LOSS_LIMIT,
                OrderType.TAKE_PROFIT,
                OrderType.TAKE_PROFIT_LIMIT
            };
            if (!stopPriceOrderTypes.Any(_ => _ == orderType))
            {
                SetCanSet(nameof(OrderQueryModel.StopPrice), false);
                return;
            }

            SetCanSet(nameof(OrderQueryModel.StopPrice), true);
        }

        /// <summary>
        ///     Устанавливает разрешения на запись <see cref="OrderQueryModel.IcebergQty"/> в зависимости от <see cref="OrderType"/>
        /// </summary>
        private void SetIcebergQty(OrderType orderType)
        {
            var icebergQtyOrderTypes = new List<OrderType>()
            {
                OrderType.LIMIT,
                OrderType.STOP_LOSS_LIMIT,
                OrderType.TAKE_PROFIT_LIMIT
            };
            if (!icebergQtyOrderTypes.Any(_ => _ == orderType))
            {
                SetCanSet(nameof(OrderQueryModel.IcebergQty), false);
                return;
            }

            SetCanSet(nameof(OrderQueryModel.IcebergQty), true);
        }

        /// <summary>
        ///     Устанавливает значение атрибутов <see cref="OrderParamAttribute.IsUse"/>
        ///     и <see cref="OrderParamAttribute.Value"/> в необходимые значения
        /// </summary>
        private void SetIsUse(string nameOfProperty, bool isUse, string value)
        {
            var property = typeof(OrderQueryModel)
                .GetProperties()
                .Where(_ => _.Name == nameOfProperty)
                .First();

            var attribute = property.GetCustomAttribute<OrderParamAttribute>();
            attribute.IsUse = isUse;
            if (!string.IsNullOrEmpty(value))
            {
                attribute.Value = value;
            }
        }

        /// <summary>
        ///     Устанавливает значение атрибута <see cref="OrderParamAttribute.CanSet"/> в необходимое значение
        /// </summary>
        private void SetCanSet(string nameOfProperty, bool value)
        {
            var property = typeof(OrderQueryModel)
                .GetProperties()
                .Where(_ => _.Name == nameOfProperty)
                .First();

            property.GetCustomAttribute<OrderParamAttribute>().CanSet = value;
        }

        /// <summary>
        ///     Возвращает значение атрибута <see cref="OrderParamAttribute.CanSet"/>
        /// </summary>
        private bool IsCanSet(string nameOfProperty)
        {
            var property = typeof(OrderQueryModel)
                .GetProperties()
                .Where(_ => _.Name == nameOfProperty)
                .First();

            return property.GetCustomAttribute<OrderParamAttribute>().CanSet;
        }

        #endregion
    }
}
