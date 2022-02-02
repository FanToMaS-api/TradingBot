using System;

namespace ExchangeLibrary.Redis
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
        ///     Увеличивить занчение или создать новое по ключу с значением по умолчанию <paramref name="defaultValue"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"> Значение по умолчанию </param>
        /// <param name="expiration"> Время экспирации ключа </param>
        void IncrementOrCreateKeyValue(string key, int defaultValue, TimeSpan expiration);
    }
}
