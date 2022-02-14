using Common.Enums;
using System;

namespace Common.Models
{
    /// <summary>
    ///     Модель ограничителя скорости
    /// </summary>
    public class RateLimit
    {
        #region .ctor

        /// <inheritdoc cref="RateLimit"/>
        public RateLimit(RateLimitType type, TimeSpan interval, int limit)
        {
            Type = type;
            Interval = interval;
            Limit = limit;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Тип ограничения скорости
        /// </summary>
        public RateLimitType Type { get; }

        /// <summary>
        ///     Временной интервал, на котором действует ограничение
        /// </summary>
        public TimeSpan Interval { get; }

        /// <summary>
        ///     Ограничение скорости
        /// </summary>
        public int Limit { get; }

        #endregion

        /// <inheritdoc />
        public override string ToString() => $"RateType: {Type}|Interval: {Interval}|Limit: {Limit}";
    }
}
