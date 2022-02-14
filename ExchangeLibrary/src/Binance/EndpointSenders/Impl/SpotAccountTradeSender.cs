using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Models.SpotAccountTrade;
using NLog;
using System;
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
        public async Task<NewOrderModelBase> SendNewOrderAsync(
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
        public async Task<NewOrderModelBase> SendNewTestOrderAsync(
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
