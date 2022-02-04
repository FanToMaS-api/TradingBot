using ExchangeLibrary.Binance.Models;
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
        ///     Лимит кол-ва запросов информации обо всех монетах
        /// </summary>
        public RateLimit AllCoinsInfoLimit { get; set; } = new RateLimit(Enums.RateLimitType.ALL_COINS_INFO, TimeSpan.FromMinutes(1), 10);

        #endregion
    }
}
