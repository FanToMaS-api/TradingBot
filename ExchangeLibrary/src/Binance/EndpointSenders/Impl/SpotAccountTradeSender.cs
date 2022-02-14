using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.DTOs.SpotAccountTrade;
using ExchangeLibrary.Binance.Enums;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /// <inheritdoc />
        public async Task<NewOrderDtoBase> SendNewOrderAsync(
            string symbol,
            OrderSideType sideType,
            OrderType orderType,
            TimeInForceType timeInForce = TimeInForceType.GTC,
            double? price = null,
            double? quantity = null,
            double? stopPrice = null,
            double? icebergQty = null,
            double recvWindow = 5000,
            OrderResponseType orderResponseType = OrderResponseType.RESULT)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<NewOrderDtoBase> SendNewTestOrderAsync(
            string symbol,
            OrderSideType sideType,
            OrderType orderType,
            TimeInForceType timeInForce = TimeInForceType.GTC,
            double? price = null,
            double? quantity = null,
            double? stopPrice = null,
            double? icebergQty = null,
            double recvWindow = 5000,
            OrderResponseType orderResponseType = OrderResponseType.RESULT)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
