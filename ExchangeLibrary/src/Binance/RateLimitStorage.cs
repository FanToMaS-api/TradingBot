using Common.Enums;
using Common.Models;
using ExchangeLibrary.Binance.Models;
using System;

namespace ExchangeLibrary.Binance
{
    /// <summary>
    ///     Хранилище лимитов для Binance
    /// </summary>
    internal class RateLimitStorage
    {
        /// <summary>
        ///     Единый лимит на совокупный вес всех запросов в минуту для IP для всех конечных точек /sapi/*
        /// </summary>
        public RateLimit SapiWeightLimit { get; set; } = 
            new RateLimit(RateLimitType.SAPI_REQUEST, TimeSpan.FromMinutes(1), 12000);

        /// <summary>
        ///     Единый лимит на совокупный вес всех запросов в минуту для IP для всех конечных точек /api/*
        /// </summary>
        public RateLimit ApiWeightLimit { get; set; } = 
            new RateLimit(RateLimitType.API_REQUEST, TimeSpan.FromMinutes(1), 1200);
    }
}
