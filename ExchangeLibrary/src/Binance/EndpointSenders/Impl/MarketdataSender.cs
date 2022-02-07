using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.DTOs.Marketdata;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        #endregion
    }
}
