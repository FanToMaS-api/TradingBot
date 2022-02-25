using System;

namespace ExchangeLibrary.Binance.Exceptions
{
    /// <summary>
    ///     Исключение о превышении количества запросов
    /// </summary>
    internal sealed class TooManyRequestsException : Exception
    {
        #region .ctor

        /// <inheritdoc cref="TooManyRequestsException"/>
        public TooManyRequestsException(DateTime? until, int limit, string key) : base()
        {
            Until = until;
            Limit = limit;
            Key = key;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     До какого времени превышен лимит
        /// </summary>
        public DateTime? Until { get; }

        /// <summary>
        ///     Лимит, который достигнут
        /// </summary>
        public int Limit { get; }

        /// <summary>
        ///     Информационный ключ лимита
        /// </summary>
        public string Key { get; }

        #endregion
    }
}
