using AutoMapper;
using Common.Enums;
using Common.Models;
using Common.Redis;
using ExchangeLibrary;
using ExchangeLibrary.Binance;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Client.Impl;
using ExchangeLibrary.Binance.EndpointSenders;
using ExchangeLibrary.Binance.EndpointSenders.Impl;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Exceptions;
using ExchangeLibrary.Binance.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TraidingBot.Exchanges.Binance
{
    /// <summary>
    ///     Binance биржа
    /// </summary>
    internal class BinanceExchange : IExchange, IDisposable
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IBinanceClient _client;
        private readonly IWalletSender _walletSender;
        private readonly IMarketdataSender _marketdataSender;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private bool _isDisposed;
        private readonly RateLimitStorage _rateLimitsStorage = new();
        private readonly RequestsWeightStorage _requestsWeightStorage = new();

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceExchange"/>
        public BinanceExchange(string apiKey, string secretKey, IMapper mapper)
        {
            _httpClient = new();
            _mapper = mapper;
            _redisDatabase = new RedisDatabase();
            _client = new BinanceClient(_httpClient, apiKey, secretKey);
            _walletSender = new WalletSender(_client);
            _marketdataSender = new MarketdataSender(_client);
        }

        #endregion

        #region Wallet

        /// <inheritdoc />
        public async Task<string> GetSystemStatusAsync(CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SistemStatusWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _walletSender.GetSystemStatusAsync(cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetAccountTraidingStatusAsync(long recvWindow = 5000, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.AccountStatusWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _walletSender.GetAccountTraidingStatusAsync(recvWindow, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetTradeFeeAsync(
            string symbol = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.TradeFeeWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _walletSender.GetTradeFeeAsync(symbol, recvWindow, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ITrade>> GetAllCoinsInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.AllCoinsInfoWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _walletSender.GetAllCoinsInformationAsync(recvWindow, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<ITrade>>(result);
        }

        #endregion

        #region Marketdata

        /// <inheritdoc />
        public async Task<string> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.OrderBookWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetOrderBookAsync(symbol, limit, cancellationToken);

            IncrementCallsMade(requestWeight, limit.ToString());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.RecentTradesWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetRecentTradesAsync(symbol, limit, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetOldTradesAsync(
            string symbol,
            long fromId,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.OldTradesWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetOldTradesAsync(symbol, fromId, limit, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetCandleStickAsync(
            string symbol,
            CandleStickIntervalType interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CandleStickDataWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetCandleStickAsync(symbol, interval, startTime, endTime, limit, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CurrentAveragePriceWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetAveragePriceAsync(symbol, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.DayTickerPriceChangeWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetDayPriceChangeAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SymbolPriceTickerWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetSymbolPriceTickerAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetSymbolOrderBookTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SymbolOrderBookTickerWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetSymbolOrderBookTickerAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return "TODO";
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверяет на достижение лимита
        /// </summary>
        private bool CheckLimit(ApiType type, out RedisKeyValue<int> keyValue)
        {
            var rateLimit = GetRateLimit(type);
            var key = GetRedisKey(rateLimit.Type);
            if (_redisDatabase.TryGetIntValue(key, out keyValue) && keyValue.Value >= rateLimit.Limit)
            {
                _logger.Warn($"Too many request '{keyValue.Value}' for '{keyValue.Key}' endpoint. Expiration limit '{keyValue.Expiration?.ToString("s")}'");

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Возвращает Redis-ключ для лимита
        /// </summary>
        private string GetRedisKey(RateLimitType type) => $"tc_binance_{type.ToString().ToLower()}";

        private RateLimit GetRateLimit(ApiType type) =>
            type switch
            {
                ApiType.Sapi => _rateLimitsStorage.SapiWeightLimit,
                ApiType.Api => _rateLimitsStorage.ApiWeightLimit,
                _ => throw new Exception($"Unknown type of {nameof(ApiType)}")
            };

        /// <summary>
        ///     Увеличивает кол-во сделанных вызовов или создает новую пару значений
        /// </summary>
        private void IncrementCallsMade(RequestWeightModel requestWeight, string weightParamKey)
        {
            var rateLimit = GetRateLimit(requestWeight.Type);
            var key = GetRedisKey(rateLimit.Type);

            if (requestWeight.Weights.TryGetValue(weightParamKey, out var value))
            {
                _redisDatabase.IncrementOrCreateKeyValue(key, rateLimit.Interval, value);
                return;
            }

            throw new KeyNotFoundException($"Key '{weightParamKey}' not found.");
        }

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _httpClient.Dispose();
        }

        #endregion
    }
}
