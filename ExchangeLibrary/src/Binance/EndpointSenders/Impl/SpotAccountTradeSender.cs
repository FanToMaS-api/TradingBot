using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using ExchangeLibrary.Binance.Models.SpotAccountTrade;
using ExchangeLibrary.Binance.Models.SpotAccountTrade.NewOrderQuery;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders.Impl
{
    /// <inheritdoc cref="ISpotAccountTradeSender"/>
    internal class SpotAccountTradeSender : ISpotAccountTradeSender
    {
        #region Fields

        private readonly IBinanceClient _client;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="SpotAccountTradeSender" />
        public SpotAccountTradeSender(IBinanceClient client)
        {
            _client = client;
        }

        #endregion

        #region Public methods

        /// TODO: Builder&Director!
        /// <inheritdoc />
        public async Task<OrderResponseModelBase> SendNewOrderAsync<T>(Builder builder, CancellationToken cancellationToken = default)
            where T : OrderResponseModelBase
        {
            var queryModel = builder.GetResult();

            var result = await _client.SendSignedAsync(
                BinanceEndpoints.NEW_TEST_ORDER,
                HttpMethod.Post,
                query: queryModel.GetQuery(),
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<T>(result);
        }

        /// <inheritdoc />
        public async Task<OrderResponseModelBase> SendNewTestOrderAsync<T>(Builder builder, CancellationToken cancellationToken = default)
            where T : OrderResponseModelBase
        {
            var queryModel = builder.GetResult();

            var result = await _client.SendSignedAsync(
                BinanceEndpoints.NEW_TEST_ORDER,
                HttpMethod.Post,
                query: queryModel.GetQuery(),
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<T>(result);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Формирует словарь параметров запроса
        /// </summary>
        /// <remarks>
        ///     https://binance-docs.github.io/apidocs/spot/en/#:~:text=Other%20info%3A,SELL%2C%20TAKE_PROFIT%20BUY
        /// </remarks>
        private Dictionary<string, object> CreateNewOrderRequestParams(
            string symbol,
            OrderSideType sideType,
            OrderType orderType,
            TimeInForceType timeInForce,
            double? price,
            double? quantity,
            double? stopPrice,
            double? icebergQty,
            double recvWindow,
            OrderResponseType orderResponseType)
        {
            var result = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "side", sideType.ToUrl() },
                { "type", orderType.ToUrl() },
                { "timeInForce", timeInForce.ToUrl() },
                { "recvWindow", recvWindow },
                { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
            };

            if (price is not null && price > 0)
            {
                result["price"] = price;
            }

            if (quantity is not null && quantity > 0)
            {
                result["quantity"] = quantity;
            }

            var stopPriceOrderTypes = new List<OrderType>()
            {
                OrderType.STOP_LOSS,
                OrderType.STOP_LOSS_LIMIT,
                OrderType.TAKE_PROFIT,
                OrderType.TAKE_PROFIT_LIMIT
            };
            if (stopPrice is not null && stopPrice > 0 && stopPriceOrderTypes.Any(_ => _ == orderType))
            {
                result["stopPrice"] = stopPrice;
            }

            var icebergQtyOrderTypes = new List<OrderType>()
            {
                OrderType.LIMIT,
                OrderType.STOP_LOSS_LIMIT,
                OrderType.TAKE_PROFIT_LIMIT
            };
            if (icebergQty is not null && icebergQty > 0 && icebergQtyOrderTypes.Any(_ => _ == orderType))
            {
                result["icebergQty"] = icebergQty;
                result["timeInForce"] = TimeInForceType.GTC.ToUrl();
            }

            if (orderType == OrderType.LIMIT || orderType == OrderType.MARKET)
            {
                result["newOrderRespType"] = OrderResponseType.FULL.ToUrl();
                return result;
            }

            result["newOrderRespType"] = orderResponseType.ToUrl();
            return result;
        }

        #endregion
    }
}
