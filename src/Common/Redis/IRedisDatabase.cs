using System;

namespace Common.Redis
{
    /// <summary>
    ///     База данных Redis
    /// </summary>
    public interface IRedisDatabase
    {
        /// <summary>
        ///     Возвращает int значение по ключу
        /// </summary>
        bool TryGetIntValue(string key, out RedisKeyValue<int> model);

        /// <summary>
        ///     Увеличивить значение или создать новое по ключу с значением по умолчанию <paramref name="value"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiration"> Время экспирации ключа </param>
        /// <param name="value"> На сколько увеличить значение  </param>
        void IncrementOrCreateKeyValue(string key, TimeSpan expiration, long value = 1);
    }
}
