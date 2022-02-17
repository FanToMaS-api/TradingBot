using ExchangeLibrary.Binance.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExchangeLibrary.Binance.Models.SpotAccountTrade.NewOrderQuery
{
    /// <summary>
    ///     Модель запроса на создание нового ордера
    /// </summary>
    internal class OrderQueryModel
    {
        #region Properties

        /// <summary>
        ///     Пара
        /// </summary>
        [OrderParam("symbol", false, true)]
        public string Symbol { get; set; }

        /// <inheritdoc cref="OrderSideType"/>
        [OrderParam("side", false, true)]
        public OrderSideType SideType { get; set; }

        /// <inheritdoc cref="OrderType"/>
        [OrderParam("type", false, true)]
        public OrderType OrderType { get; set; }

        /// <inheritdoc cref="TimeInForceType"/>
        [OrderParam("timeInForce", false, true)]
        public TimeInForceType TimeInForce { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        [OrderParam("price", false, true)]
        public double? Price { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        [OrderParam("quantity", false, true)]
        public double? Quantity { get; set; }

        /// <summary>
        ///     Стоп цена
        /// </summary>
        [OrderParam("stopPrice", false, true)]
        public double? StopPrice { get; set; }

        /// <summary>
        ///     Кол-во для ордера-айсберга
        /// </summary>
        [OrderParam("icebergQty", false, true)]
        public double? IcebergQty { get; set; }

        /// <summary>
        ///     Кол-во миллисекунд, которое прибавляется к timestamp и формирует окно действия запроса
        /// </summary>
        [OrderParam("recvWindow", false, true)]
        public double RecvWindow { get; set; }

        /// <summary>
        ///     Информация возврата, если удалось создать ордер
        /// </summary>
        [OrderParam("newOrderRespType", false, true)]
        public OrderResponseType OrderResponseType { get; set; }

        /// <summary>
        ///     Время отправки
        /// </summary>
        [OrderParam("timeStamp", false, true)]
        public long TimeStamp { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Возвращает словрь параметров для запроса
        /// </summary>
        public Dictionary<string, object> GetQuery()
        {
            var neededParams = typeof(OrderQueryModel)
                .GetProperties()
                .Where(v => v.GetCustomAttribute<OrderParamAttribute>().IsUse)
                .ToArray();

            var query = new Dictionary<string, object>();
            foreach (var param in neededParams)
            {
                var attribute = param.GetCustomAttribute<OrderParamAttribute>();
                query[attribute.Url] = attribute.Value;
            }

            return query;
        }

        #endregion
    }
}