﻿using ExchangeLibrary.Binance.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель запроса на создание нового ордера
    /// </summary>
    internal class OrderQueryModel
    {
        #region .ctor

        /// <<inheritdoc cref="OrderQueryModel"/>
        public OrderQueryModel()
        {
            Symbol = new OrderParamWrapper("symbol", false, true);
            SideType = new OrderParamWrapper("side", false, true);
            OrderType = new OrderParamWrapper("type", false, true);
            TimeInForce = new OrderParamWrapper("timeInForce", false, true);
            Price = new OrderParamWrapper("price", false, true);
            Quantity = new OrderParamWrapper("quantity", false, true);
            StopPrice = new OrderParamWrapper("stopPrice", false, true);
            IcebergQty = new OrderParamWrapper("icebergQty", false, true);
            RecvWindow = new OrderParamWrapper("recvWindow", false, true);
            OrderResponseType = new OrderParamWrapper("newOrderRespType", false, true);
            TimeStamp = new OrderParamWrapper("timestamp", false, true);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Пара
        /// </summary>
        public OrderParamWrapper Symbol { get; set; }

        /// <inheritdoc cref="OrderSideType"/>
        public OrderParamWrapper SideType { get; set; }

        /// <inheritdoc cref="OrderType"/>
        public OrderParamWrapper OrderType { get; set; }

        /// <inheritdoc cref="TimeInForceType"/>
        public OrderParamWrapper TimeInForce { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        public OrderParamWrapper Price { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        public OrderParamWrapper Quantity { get; set; }

        /// <summary>
        ///     Стоп цена
        /// </summary>
        public OrderParamWrapper StopPrice { get; set; }

        /// <summary>
        ///     Кол-во для ордера-айсберга
        /// </summary>
        public OrderParamWrapper IcebergQty { get; set; }

        /// <summary>
        ///     Кол-во миллисекунд, которое прибавляется к timestamp и формирует окно действия запроса
        /// </summary>
        public OrderParamWrapper RecvWindow { get; set; }

        /// <summary>
        ///     Информация возврата, если удалось создать ордер
        /// </summary>
        public OrderParamWrapper OrderResponseType { get; set; }

        /// <summary>
        ///     Время отправки
        /// </summary>
        public OrderParamWrapper TimeStamp { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Возвращает словрь параметров для запроса
        /// </summary>
        public Dictionary<string, object> GetQuery()
        {
            var neededParams = typeof(OrderQueryModel)
                .GetProperties()
                .Where(_ => ((OrderParamWrapper)_.GetValue(this)).IsUse)
                .ToArray();

            var query = new Dictionary<string, object>();
            foreach (var param in neededParams)
            {
                var paramWrapper = (OrderParamWrapper)param.GetValue(this);
                query[paramWrapper.Url] = paramWrapper.ValueStr;
            }

            return query;
        }

        #endregion
    }
}