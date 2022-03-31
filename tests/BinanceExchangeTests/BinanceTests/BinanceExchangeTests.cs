using Common.Enums;
using Common.Models;
using NSubstitute;
using Redis;
using StackExchange.Redis;
using System;
using Xunit;

namespace BinanceExchangeTests.BinanceTests
{
    /// <summary>
    ///     Класс, тестирующий <see cref="BinanceExchange.BinanceExchange"/>
    /// </summary>
    public class BinanceExchangeTests
    {
        #region Speed limit Test

        /// <summary>
        ///     Проверка работы отслеживания частоты запросов
        /// </summary>
        [Fact(DisplayName = "Speed limit Test")]
        public void LimitExceeding_Test()
        {
            var (database, redisDatabase) = CreateMockRedisDatabase();
            var expiry = TimeSpan.FromSeconds(5); // время экспирации ключа
            var valueLimit = 5; // ограничение на скорость запроса
            var currentValue = 1; // текущее значение кол-ва скорости запроса
            var key = "rateLimitTest";
            var rateLimit = new RateLimit(RateLimitType.API_REQUEST, expiry, valueLimit);

            // сначала возвращаю null, так как база пуста
            database.StringGetWithExpiry(Arg.Any<RedisKey>()).Returns(new RedisValueWithExpiry(new RedisValue(string.Empty), null));
            var actualKey = "";
            var actualValueLimit = 0;
            var actualExpiry = TimeSpan.FromSeconds(0);
            database.StringSet(key, valueLimit, expiry).Returns(callInfo =>
            {
                actualKey = callInfo.ArgAt<RedisKey>(0);
                actualValueLimit = (int)callInfo.ArgAt<RedisValue>(1);
                actualExpiry = callInfo.ArgAt<TimeSpan>(2);

                return true;
            });

            // Установить новый лимит так как его еще нет
            redisDatabase.TryIncrementOrCreateKeyValue(key, rateLimit.Interval, rateLimit.Limit);
            Assert.Equal(key, actualKey);
            Assert.Equal(valueLimit, actualValueLimit);
            Assert.Equal(expiry, actualExpiry);

            // устанавливаю действие на функцию увеличения ключа 
            database.StringIncrement(key, 1).Returns(callInfo =>
            {
                currentValue++;
                return currentValue;
            });

            // Увеличить его пару раз без ошибок
            for (var i = 0; i < 4; i++)
            {
                // теперь возвращаем уже со значением, каждый раз с новым
                database.StringGetWithExpiry(key).Returns(new RedisValueWithExpiry(new RedisValue(currentValue.ToString()), expiry));

                redisDatabase.TryIncrementOrCreateKeyValue(key, rateLimit.Interval, 1);
                Assert.False(IsLimitExceeded());
            }

            // возвращаем уже с новым значением
            database.StringGetWithExpiry(key).Returns(new RedisValueWithExpiry(new RedisValue(currentValue.ToString()), expiry));

            // Увеличить с получением ошибки
            redisDatabase.TryIncrementOrCreateKeyValue(key, rateLimit.Interval, 1);
            Assert.True(IsLimitExceeded());

            // Проверяет на достижение лимита
            bool IsLimitExceeded()
            {
                return redisDatabase.TryGetIntValue(key, out var keyValue) && keyValue.Value >= rateLimit.Limit;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Создает мок Redis базы данных
        /// </summary>
        private (IDatabase database, RedisDatabase redisDatabase) CreateMockRedisDatabase()
        {
            var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
            var database = Substitute.For<IDatabase>();
            connectionMultiplexer.GetDatabase().Returns(database);
            var redisDatabase = new RedisDatabase(connectionMultiplexer);

            return (database, redisDatabase);
        }

        #endregion
    }
}
