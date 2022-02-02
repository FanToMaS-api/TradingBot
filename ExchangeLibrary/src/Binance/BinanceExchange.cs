using ExchangeLibrary;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Client.Helpers;
using ExchangeLibrary.Binance.Client.Impl;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Exceptions;
using ExchangeLibrary.Binance.Models;
using ExchangeLibrary.Redis;
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
        private readonly IRedisDatabase _redisDatabase;
        private readonly HttpClient _httpClient;
        private bool _isDisposed;
        private readonly RateLimitStorage _rateLimits = new();

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceExchange"/>
        public BinanceExchange(string apiKey, string secretKey)
        {
            _httpClient = new();
            _redisDatabase = new RedisDatabase();
            _client = new BinanceClient(_httpClient, apiKey, secretKey);
        }

        #endregion

        /// <inheritdoc />
        public async Task<string> GetAllCoinsInformationAsync(long recvWindow, CancellationToken cancellationToken)
        {
            var rateModel = _rateLimits.AllCoinsInfoLimit;
            if (CheckLimit(rateModel, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _client.SendSignedAsync(
              BinanceEndpoints.ALL_COINS_INFORMATION,
              HttpMethod.Get,
              query: new Dictionary<string, object>
              {
                    { "recvWindow", recvWindow },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
              });

            IncrementCallsMade(rateModel);

            return result;
        }

        #region Private methods

        /// <summary>
        ///     Проверяет на достижение лимита
        /// </summary>
        private bool CheckLimit(RateLimit rateLimit, out RedisKeyValue<int> keyValue)
        {
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

        /// <summary>
        ///     Увеличивает кол-во сделанных вызовов
        /// </summary>
        private void IncrementCallsMade(RateLimit rateLimit)
        {
            var key = GetRedisKey(rateLimit.Type);

            _redisDatabase.IncrementOrCreateKeyValue(key, 1, rateLimit.Interval);
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
