using Common.Enums;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.DTOs.Marketdata;
using ExchangeLibrary.Binance.Enums.Helper;
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

        /// <inheritdoc />
        public async Task<AveragePriceDto> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.AVERAGE_PRICE,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                },
                cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<AveragePriceDto>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DayPriceChangeDto>> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var isNull = string.IsNullOrEmpty(symbol);
            var responce = await _client.SendPublicAsync(
                BinanceEndpoints.DAY_PRICE_CHANGE,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                },
                cancellationToken: cancellationToken);

            if (!isNull)
            {
                var result = new List<DayPriceChangeDto>();
                result.Add(JsonConvert.DeserializeObject<DayPriceChangeDto>(responce));

                return result;
            }

            return JsonConvert.DeserializeObject<IEnumerable<DayPriceChangeDto>>(responce);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolPriceTickerDto>> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var isNull = string.IsNullOrEmpty(symbol);
            var responce = await _client.SendPublicAsync(
                BinanceEndpoints.SYMBOL_PRICE_TICKER,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                },
                cancellationToken: cancellationToken);

            if (!isNull)
            {
                var result = new List<SymbolPriceTickerDto>();
                result.Add(JsonConvert.DeserializeObject<SymbolPriceTickerDto>(responce));

                return result;
            }

            return JsonConvert.DeserializeObject<IEnumerable<SymbolPriceTickerDto>>(responce);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolOrderBookTickerDto>> GetSymbolOrderBookTickerAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            var isNull = string.IsNullOrEmpty(symbol);
            var responce = await _client.SendPublicAsync(
                BinanceEndpoints.SYMBOL_ORDER_BOOK_TICKER,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                },
                cancellationToken: cancellationToken);

            if (!isNull)
            {
                var result = new List<SymbolOrderBookTickerDto>();
                result.Add(JsonConvert.DeserializeObject<SymbolOrderBookTickerDto>(responce));

                return result;
            }

            return JsonConvert.DeserializeObject<IEnumerable<SymbolOrderBookTickerDto>>(responce);
        }

        #endregion
    }
}
