using ExchangeLibrary.Binance.Models;
using ExchangeLibrary.Binance.Enums;
using System;

namespace ExchangeLibrary.Binance
{
    /// <summary>
    ///     Настройки для Binance
    /// </summary>
    internal class RateLimitStorage
    {
        #region Properties

        /// <summary>
        ///     Лимит кол-ва запросов статуса кошелька
        /// </summary>
        public RateLimit StatusLimit { get; set; } = new RateLimit(RateLimitType.STATUS_INFO, TimeSpan.FromMinutes(1), 1);

        /// <summary>
        ///     Лимит кол-ва запросов информации обо всех монетах
        /// </summary>
        public RateLimit AllCoinsInfoLimit { get; set; } = new RateLimit(RateLimitType.ALL_COINS_INFO, TimeSpan.FromMinutes(1), 10);

        #endregion
    }
}
