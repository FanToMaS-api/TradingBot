﻿using Common.Enums;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.DTOs.Marketdata;
using ExchangeLibrary.Binance.Models;
using ExchangeLibrary.src.Binance.Enums.Helper;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.EndpointSenders.Impl
{
    internal class MarketdataSender : IMarketdataSender
    {
        #region Fields

        private readonly IBinanceClient _client;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataSender" />
        public MarketdataSender(IBinanceClient client)
        {
            _client = client;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<OrderBookDto> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.ORDER_BOOK,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                    { "limit", limit },
                },
                cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<OrderBookDto>(result, new OrderBookDtoConverter());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RecentTradeDto>> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.RECENT_TRADES,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                    { "limit", limit },
                },
                cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<RecentTradeDto>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RecentTradeDto>> GetOldTradesAsync(string symbol, long fromId, int limit = 500, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.OLD_TRADES,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                    { "limit", limit },
                    { "fromId", fromId },
                },
                cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<RecentTradeDto>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CandleStickDto>> GetCandleStickAsync(
            string symbol,
            CandleStickIntervalType interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var parameters =
                new Dictionary<string, object>
                {
                    { "symbol", symbol },
                    { "interval", interval.GetInterval() },
                    { "limit", limit },
                };
            if (startTime is not null && endTime is not null)
            {
                parameters["startTime"] = startTime;
                parameters["endTime"] = endTime;
            }

            var result = await _client.SendPublicAsync(
                BinanceEndpoints.CANDLESTICK_DATA,
                HttpMethod.Get,
                query: parameters,
                cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<CandleStickDto>>(result, new CandleStickDtoConverter());
        }

        #endregion
    }
}
