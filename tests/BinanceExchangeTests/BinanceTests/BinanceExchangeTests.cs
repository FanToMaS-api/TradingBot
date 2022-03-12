using Common.Models;
using Common.Redis;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BinanceExchangeTests.BinanceTests
{
    /// <summary>
    ///     Класс, тестирующий <see cref="BinanceExchange.BinanceExchange"/>
    /// </summary>
    public class BinanceExchangeTests
    {
        #region Limit exceeding test
        
        /// <summary>
        ///     Проверка работы отслеживания частоты запросов
        /// </summary>
        [Fact]
        public void LimitExceeding_Test()
        {
            var redisDatabase = Substitute.For<IRedisDatabase>();

            // Установить новый лимит
            var key = "rateLimitTest";
            var rateLimit = new RateLimit(Common.Enums.RateLimitType.API_REQUEST, TimeSpan.FromSeconds(5), 5);
            redisDatabase.IncrementOrCreateKeyValue(key, rateLimit.Interval, 0);

            // Увеличить его пару раз без ошибок
            for (var i = 0; i < 4; i++)
            {
                redisDatabase.IncrementOrCreateKeyValue(key, rateLimit.Interval, 1);
                Assert.False(IsLimitExceeded());
            }

            // Увеличить с получением ошибки
            redisDatabase.IncrementOrCreateKeyValue(key, rateLimit.Interval, 1);
            Assert.True(IsLimitExceeded());

            // подождать опред время пока время действия ограничения пройдет
            // и попробовать снова увеличить, не получив ошибку
            Task.Delay(rateLimit.Interval).Wait();
            redisDatabase.IncrementOrCreateKeyValue(key, rateLimit.Interval, 4);
            Assert.False(IsLimitExceeded());

            // Проверят на достижение лимита
            bool IsLimitExceeded()
            {
                return redisDatabase.TryGetIntValue(key, out var keyValue) && keyValue.Value >= rateLimit.Limit;
            }

        }

        #endregion
    }
}
