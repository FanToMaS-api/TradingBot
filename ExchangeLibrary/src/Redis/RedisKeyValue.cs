using System;

namespace ExchangeLibrary.Redis
{
    /// <summary>
    ///     Пара ключ значение
    /// </summary>
    public class RedisKeyValue<T>
    {
        #region .ctor

        /// <inheritdoc cref="RedisKeyValue" />
        public RedisKeyValue(string key)
        {
            Key = key;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Значение
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        ///     Дата и время экспирации ключа
        /// </summary>
        public DateTime? Expiration { get; set; }

        #endregion
    }
}
