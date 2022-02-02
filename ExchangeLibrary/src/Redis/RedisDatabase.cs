using NLog;
using StackExchange.Redis;
using System;

namespace ExchangeLibrary.Redis
{
    /// <inheritdoc cref="IRedisDatabase"/>
    internal class RedisDatabase : IRedisDatabase
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private const int ReconnectionCount = 2;
        static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
           new ConfigurationOptions
           {
               EndPoints = { "localhost:6379" }
           });
        private readonly IDatabase _database;

        #endregion

        #region .ctor

        /// <inheritdoc cref="IRedisDatabase"/>
        public RedisDatabase()
        {
            _database = redis.GetDatabase();
        }

        #endregion

        /// <inheritdoc />
        public bool TryGetIntValue(string key, out RedisKeyValue<int> model)
        {
            model = new RedisKeyValue<int>(key);
            var redisValue = ExecuteRequest(() => _database.StringGetWithExpiry(key));

            if (redisValue.Value.HasValue && redisValue.Value.TryParse(out int value))
            {
                model.Value = value;
                model.Expiration = DateTime.UtcNow + redisValue.Expiry;

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void IncrementOrCreateKeyValue(string key, int defaultValue, TimeSpan expiration)
        {
            var redisValue = ExecuteRequest(() => _database.StringGetWithExpiry(key));

            if (redisValue.Value.HasValue)
            {
                ExecuteRequest(() => _database.StringIncrement(key));

                // Случаются моменты, когда ключ создаётся без экспирации
                // Вручную проставляем экспирацию, поверх имеющихся значений ключа
                if (!redisValue.Expiry.HasValue)
                {
                    ExecuteRequest(() => _database.KeyExpire(key, expiration));
                }
            }
            else
            {
                ExecuteRequest(() => _database.StringSet(key, defaultValue, expiration));
            }
        }

        #region Private Methods

        /// <summary>
        ///     Выполнение запроса
        /// </summary>
        private T ExecuteRequest<T>(Func<T> func)
        {
            var exception = new Exception("Limit reconnection");

            for (var i = 0; i < ReconnectionCount; i++)
            {
                try
                {
                    return func.Invoke();
                }
                catch (RedisTimeoutException ex)
                {
                    _logger.Warn(ex, "Timeout execute request");

                    exception = new Exception(ex.Message, ex);
                }
            }

            throw exception;
        }

        #endregion
    }
}
