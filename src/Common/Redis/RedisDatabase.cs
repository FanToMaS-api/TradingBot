using NLog;
using StackExchange.Redis;
using System;

namespace Common.Redis
{
    /// <inheritdoc cref="IRedisDatabase"/>
    public class RedisDatabase : IRedisDatabase
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private const int ReconnectionCount = 2;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _database;

        #endregion

        #region .ctor

        /// <inheritdoc cref="IRedisDatabase"/>
        public RedisDatabase(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();
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
        public bool TryIncrementOrCreateKeyValue(string key, TimeSpan expiration, int value = 1)
        {
            var redisValue = ExecuteRequest(() => _database.StringGetWithExpiry(key));

            if (redisValue.Value.HasValue)
            {
                ExecuteRequest(() => _database.StringIncrement(key, value));

                // Случаются моменты, когда ключ создаётся без экспирации
                // Вручную проставляем экспирацию, поверх имеющихся значений ключа
                if (!redisValue.Expiry.HasValue)
                {
                    if (!ExecuteRequest(() => _database.KeyExpire(key, expiration)))
                    {
                        _logger.Warn($"Failed to set KeyExpire: key='{key}', expiration={expiration:G}");

                        return false;
                    }
                }
            }
            else
            {
                if (!ExecuteRequest(() => _database.StringSet(key, value, expiration)))
                {
                    _logger.Warn($"Failed to set redis KeyValue: key='{key}', value={value}, expiration={expiration:G}");

                    return false;
                };
            }

            return true;
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

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose() => _connectionMultiplexer.Dispose();

        #endregion
    }
}
