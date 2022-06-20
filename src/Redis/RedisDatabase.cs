using Logger;
using StackExchange.Redis;
using System;

namespace Redis
{
    /// <inheritdoc cref="IRedisDatabase"/>
    public class RedisDatabase : IRedisDatabase
    {
        #region Fields

        private const int ReconnectionCount = 2;
        private static readonly ILoggerDecorator Log = LoggerManager.CreateDefaultLogger();
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
        
        #region Implmentation of IRedisDatabase

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
                        Log.WarnAsync($"Failed to set KeyExpire: key='{key}', expiration={expiration:G}");

                        return false;
                    }
                }
            }
            else
            {
                if (!ExecuteRequest(() => _database.StringSet(key, value, expiration)))
                {
                    Log.WarnAsync($"Failed to set redis KeyValue: key='{key}', value={value}, expiration={expiration:G}");

                    return false;
                };
            }

            return true;
        }
        
        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose() => _connectionMultiplexer.Dispose();

        #endregion

        #region Private Methods

        /// <summary>
        ///     Выполнение запроса
        /// </summary>
        private static T ExecuteRequest<T>(Func<T> func)
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
                    Log.WarnAsync(ex, "Timeout execute request");

                    exception = new Exception(ex.Message, ex);
                }
            }

            throw exception;
        }

        #endregion
    }
}
