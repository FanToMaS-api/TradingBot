using ExchangeLibrary.Binance.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.Models
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
        public RateLimitType Type { get; set; }

        /// <summary>
        ///     Временной интервал, на котором действует ограничение
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        ///     Ограничение скорости
        /// </summary>
        public int Limit { get; set; }

        #endregion

        /// <inheritdoc />
        public override string ToString() => $"RateType: {Type}|Interval: {Interval}|Limit: {Limit}";
    }
}
