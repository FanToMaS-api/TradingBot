using Common.Enums;
using Common.Models;
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
        ///     Лимит кол-ва запросов статуса системы
        /// </summary>
        public RateLimit SistemStatusLimit { get; set; } = new RateLimit(RateLimitType.SISTEM_STATUS_INFO, TimeSpan.FromMinutes(1), 1);
        
        /// <summary>
        ///     Лимит кол-ва запросов статуса аккаунта
        /// </summary>
        public RateLimit AccountStatusLimit { get; set; } = new RateLimit(RateLimitType.ACCOUNT_STATUS_INFO, TimeSpan.FromMinutes(1), 1);

        /// <summary>
        ///     Лимит кол-ва запросов информации обо всех монетах
        /// </summary>
        public RateLimit AllCoinsInfoLimit { get; set; } = new RateLimit(RateLimitType.ALL_COINS_INFO, TimeSpan.FromMinutes(1), 10);

        #endregion
    }
}
