using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.DTOs.Marketdata;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
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

            var converter = new JsonConvertWrapper();
            converter.AddConverter(new OrderBookDtoConverter());
            return converter.Deserialize<OrderBookDto>(result);
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

            var converter = new JsonConvertWrapper();
            return converter.Deserialize<IEnumerable<RecentTradeDto>>(result);
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

            var converter = new JsonConvertWrapper();
            return converter.Deserialize<IEnumerable<RecentTradeDto>>(result);
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

            var converter = new JsonConvertWrapper();
            converter.AddConverter(new CandleStickDtoEnumerableConverter());
            return converter.Deserialize<IEnumerable<CandleStickDto>>(result);
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

            var converter = new JsonConvertWrapper();
            return converter.Deserialize<AveragePriceDto>(result);
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

            var converter = new JsonConvertWrapper();
            if (!isNull)
            {
                var result = new List<DayPriceChangeDto>
                {
                    converter.Deserialize<DayPriceChangeDto>(responce)
                };

                return result;
            }

            return converter.Deserialize<IEnumerable<DayPriceChangeDto>>(responce);
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

            var converter = new JsonConvertWrapper();
            if (!isNull)
            {
                var result = new List<SymbolPriceTickerDto>
                {
                    converter.Deserialize<SymbolPriceTickerDto>(responce)
                };

                return result;
            }

            return converter.Deserialize<IEnumerable<SymbolPriceTickerDto>>(responce);
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

            var converter = new JsonConvertWrapper();
            if (!isNull)
            {
                var result = new List<SymbolOrderBookTickerDto>
                {
                    converter.Deserialize<SymbolOrderBookTickerDto>(responce)
                };

                return result;
            }

            return converter.Deserialize<IEnumerable<SymbolOrderBookTickerDto>>(responce);
        }

        #endregion
    }
}
