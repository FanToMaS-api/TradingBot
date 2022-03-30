using AutoMapper;
using BinanceExchange.Client;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Enums;
using BinanceExchange.Exceptions;
using BinanceExchange.Models;
using BinanceExchange.RedisRateLimits;
using BinanceExchange.RequestWeights;
using Common.Enums;
using ExchangeLibrary;
using NLog;
using Redis;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.Impl
{
    /// <inheritdoc cref="ISpotTrade"/>
    internal class SpotTrade : ISpotTrade
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISpotTradeSender _tradeSender;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IMapper _mapper;
        private readonly SpotTradeWeightStorage _weightStorage = new();

        #endregion

        #region .ctor

        /// <inheritdoc cref="SpotTrade"/>
        public SpotTrade(IBinanceClient binanceClient, IRedisDatabase redisDatabase, IMapper mapper)
        {
            _tradeSender = new SpotTradeSender(binanceClient);
            _redisDatabase = redisDatabase;
            _mapper = mapper;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewLimitOrderAsync(
            string symbol,
            OrderSideType sideType,
            string forceType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.NewOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.Limit);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetTimeInForce(forceType);
            builder.SetPrice(price);
            builder.SetQuantity(quantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewMarketOrderAsync(
            string symbol,
            OrderSideType sideType,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.NewOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.Market);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetQuantity(quantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewStopLossOrderAsync(
            string symbol,
            OrderSideType sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.NewOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.StopLoss);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetStopPrice(stopPrice);
            builder.SetQuantity(quantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewStopLossLimitOrderAsync(
            string symbol,
            OrderSideType sideType,
            string forceType,
            double price,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.NewOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.StopLossLimit);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetTimeInForce(forceType);
            builder.SetPrice(price);
            builder.SetQuantity(quantity);
            builder.SetStopPrice(stopPrice);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewTakeProfitOrderAsync(
            string symbol,
            OrderSideType sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.NewOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.TakeProfit);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetQuantity(quantity);
            builder.SetStopPrice(stopPrice);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewTakeProfitLimitOrderAsync(
            string symbol,
            OrderSideType sideType,
            string forceType,
            double price,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.NewOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.TakeProfitLimit);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetTimeInForce(forceType);
            builder.SetPrice(price);
            builder.SetQuantity(quantity);
            builder.SetStopPrice(stopPrice);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewLimitMakerOrderAsync(
            string symbol,
            OrderSideType sideType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.NewOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.LimitMaker);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetPrice(price);
            builder.SetQuantity(quantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.CancelOrderResponseModel> CancelOrderAsync(
            string symbol,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.CancelOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetRecvWindow(recvWindow);
            if (orderId.HasValue)
            {
                builder.SetOrderId(orderId.Value);
            }

            if (!string.IsNullOrEmpty(origClientOrderId))
            {
                builder.SetOrigClientOrderId(origClientOrderId);
            }

            var query = builder.GetResult().GetQuery();

            var result = await _tradeSender.CancelOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.CancelOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CancelOrderResponseModel>> CancelAllOrdersAsync(
            string symbol,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.CancelOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = await _tradeSender.CancelAllOrdersAsync(query, cancellationToken);
            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.CancelOrderResponseModel>>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.CheckOrderResponseModel> CheckOrderAsync(
            string symbol,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.CheckOrderWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetRecvWindow(recvWindow);
            if (orderId.HasValue)
            {
                builder.SetOrderId(orderId.Value);
            }

            if (!string.IsNullOrEmpty(origClientOrderId))
            {
                builder.SetOrigClientOrderId(origClientOrderId);
            }

            var query = builder.GetResult().GetQuery();
            var result = await _tradeSender.CheckOrderAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.CheckOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CheckOrderResponseModel>> CheckAllOpenOrdersAsync(
            string symbol,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.CheckAllOpenOrdersWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetRecvWindow(recvWindow);
            var isSymbolOmitted = string.IsNullOrEmpty(symbol);
            if (!isSymbolOmitted)
            {
                builder.SetSymbol(symbol);
            }

            var query = builder.GetResult().GetQuery();
            var result = await _tradeSender.CheckAllOpenOrdersAsync(query, cancellationToken);

            var key = isSymbolOmitted ? "null" : RequestWeightModel.GetDefaultKey();
            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, key);

            return _mapper.Map<IEnumerable<Common.Models.CheckOrderResponseModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CheckOrderResponseModel>> GetAllOrdersAsync(
            string symbol,
            long? orderId = null,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.GetAllOrdersWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetRecvWindow(recvWindow);
            builder.SetLimit(limit);
            if (orderId.HasValue)
            {
                builder.SetOrderId(orderId.Value);
            }

            if (startTime.HasValue)
            {
                builder.SetStartTime(startTime.Value);
            }

            if (endTime.HasValue)
            {
                builder.SetEndTime(endTime.Value);
            }

            var query = builder.GetResult().GetQuery();
            var result = await _tradeSender.GetAllOrdersAsync(query, cancellationToken);
            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.CheckOrderResponseModel>>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.AccountInformationModel> GetAccountInformationAsync(CancellationToken cancellationToken)
        {
            var requestWeight = _weightStorage.AccountInformationWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var query = new Builder().GetResult().GetQuery();
            var result = await _tradeSender.GetAccountInformationAsync(query, cancellationToken);
            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.AccountInformationModel>(result);
        }

        #endregion
    }
}
