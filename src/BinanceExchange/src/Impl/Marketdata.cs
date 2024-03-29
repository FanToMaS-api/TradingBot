﻿using AutoMapper;
using BinanceExchange.EndpointSenders;
using BinanceExchange.Exceptions;
using BinanceExchange.Models;
using BinanceExchange.RedisRateLimits;
using BinanceExchange.RequestWeights;
using Common.Models;
using ExchangeLibrary;
using Redis;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.Impl
{
    /// <inheritdoc cref="IMarketdata"/>
    internal class Marketdata : IMarketdata
    {
        #region Fields

        private readonly MarketdataRequestWeightStorage _weightStorage = new();
        private readonly IMarketdataSender _marketdataSender;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IMapper _mapper;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Marketdata"/>
        public Marketdata(
            IMarketdataSender marketdataSender,
            IRedisDatabase redisDatabase,
            IMapper mapper)
        {
            _marketdataSender = marketdataSender;
            _redisDatabase = redisDatabase;
            _mapper = mapper;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<IEnumerable<TradeObjectRuleTradingModel>> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.ExchangeInfoWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());
            var model = await _marketdataSender.GetExchangeInfoAsync(cancellationToken);

            return _mapper.Map<IEnumerable<TradeObjectRuleTradingModel>>(model.Symbols);
        }

        /// <inheritdoc />
        public async Task<Common.Models.OrderBookModel> GetOrderBookAsync(
            string symbol,
            int limit = 100,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.OrderBookWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder()
                .SetSymbol(symbol)
                .SetLimit(limit);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, limit.ToString());

            var parameters = builder.GetResult(false).GetRequestParameters();
            var result = await _marketdataSender.GetOrderBookAsync(parameters, cancellationToken);

            return _mapper.Map<Common.Models.OrderBookModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.TradeModel>> GetRecentTradesAsync(
            string symbol,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.RecentTradesWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder()
                .SetSymbol(symbol)
                .SetLimit(limit);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            var parameters = builder.GetResult(false).GetRequestParameters();
            var result = await _marketdataSender.GetRecentTradesAsync(parameters, cancellationToken);

            return _mapper.Map<IEnumerable<Common.Models.TradeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.TradeModel>> GetOldTradesAsync(
            string symbol,
            long? fromId = null,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.OldTradesWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder()
                .SetSymbol(symbol)
                .SetLimit(limit);
            if (fromId.HasValue)
            {
                builder.SetFromId(fromId.Value);
            }

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            var parameters = builder.GetResult(false).GetRequestParameters();
            var result = await _marketdataSender.GetOldTradesAsync(parameters, cancellationToken);

            return _mapper.Map<IEnumerable<Common.Models.TradeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CandlestickModel>> GetCandlestickAsync(
            string symbol,
            string interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.CandlestickDataWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder()
                .SetSymbol(symbol)
                .SetLimit(limit)
                .SetCandlestickInterval(interval);
            if (startTime.HasValue)
            {
                builder.SetStartTime(startTime.Value);
            }

            if (endTime.HasValue)
            {
                builder.SetEndTime(endTime.Value);
            }

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            var parameters = builder.GetResult(false).GetRequestParameters();
            var result = await _marketdataSender.GetCandlestickAsync(parameters, cancellationToken);

            return _mapper.Map<IEnumerable<Common.Models.CandlestickModel>>(result);
        }

        /// <inheritdoc />
        public async Task<double> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.CurrentAveragePriceWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder()
                .SetSymbol(symbol);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            var parameters = builder.GetResult(false).GetRequestParameters();
            var result = await _marketdataSender.GetAveragePriceAsync(parameters, cancellationToken);

            return result.AveragePrice;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.DayPriceChangeModel>> GetDayPriceChangeAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.DayTickerPriceChangeWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, key);
            var result = await _marketdataSender.GetDayPriceChangeAsync(symbol, cancellationToken);

            return _mapper.Map<IEnumerable<Common.Models.DayPriceChangeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeObjectNamePriceModel>> GetSymbolPriceTickerAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.SymbolPriceTickerWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, key);
            var result = await _marketdataSender.GetSymbolPriceTickerAsync(symbol, cancellationToken);

            return _mapper.Map<IEnumerable<TradeObjectNamePriceModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BestSymbolOrderModel>> GetBestSymbolOrdersAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.SymbolOrderBookTickerWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, key);
            var result = await _marketdataSender.GetSymbolOrderBookTickerAsync(symbol, cancellationToken);

            return _mapper.Map<IEnumerable<BestSymbolOrderModel>>(result);
        }

        #endregion
    }
}
