using BinanceExchange.Enums;
using BinanceExchange.Models;
using Common.Enums;
using Common.Models;
using NLog;
using Redis;
using System;
using System.Collections.Generic;

namespace BinanceExchange.RedisRateLimits
{
    /// <summary>
    ///     Содержит общие методы для работы с редисом
    /// </summary>
    internal static class RedisHelper
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static readonly RateLimitStorage _rateLimitsStorage = new();

        /// <summary>
        ///     Проверяет на достижение лимита
        /// </summary>
        internal static bool CheckLimit(IRedisDatabase redisDatabase, ApiType type, out RedisKeyValue<int> keyValue)
        {
            var rateLimit = GetRateLimit(type);
            var key = GetRedisKey(rateLimit.Type);
            if (redisDatabase.TryGetIntValue(key, out keyValue) && keyValue.Value >= rateLimit.Limit)
            {
                Logger.Warn($"Too many request '{keyValue.Value}' for '{keyValue.Key}' endpoint. Expiration limit '{keyValue.Expiration?.ToString("s")}'");

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Возвращает Redis-ключ для лимита
        /// </summary>
        internal static string GetRedisKey(RateLimitType type) => $"tc_binance_{type.ToString().ToLower()}";

        /// <summary>
        ///     Возвращает модель ограничения скорости
        /// </summary>
        internal static RateLimit GetRateLimit(ApiType type) =>
            type switch
            {
                ApiType.Sapi => _rateLimitsStorage.SapiWeightLimit,
                ApiType.Api => _rateLimitsStorage.ApiWeightLimit,
                _ => throw new Exception($"Unknown type of {nameof(ApiType)}")
            };

        /// <summary>
        ///     Увеличивает кол-во сделанных вызовов или создает новую пару значений
        /// </summary>
        internal static void IncrementCallsMade(IRedisDatabase redisDatabase, RequestWeightModel requestWeight, string weightParamKey)
        {
            var rateLimit = GetRateLimit(requestWeight.Type);
            var key = GetRedisKey(rateLimit.Type);

            if (requestWeight.Weights.TryGetValue(weightParamKey, out var value))
            {
                if (redisDatabase.TryIncrementOrCreateKeyValue(key, rateLimit.Interval, value))
                {
                    return;
                }

                throw new Exception($"Failed to increment or create ketValue: key='{key}' interval='{rateLimit.Interval}' value='{value}'");
            }

            throw new KeyNotFoundException($"Key '{weightParamKey}' not found.");
        }
    }
}
