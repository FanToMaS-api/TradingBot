using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Models;
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
        public async Task<OrderBookModel> GetOrderBookAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.ORDER_BOOK,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new OrderBookModelConverter());
            return converter.Deserialize<OrderBookModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RecentTradeModel>> GetRecentTradesAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.RECENT_TRADES,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<IEnumerable<RecentTradeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RecentTradeModel>> GetOldTradesAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.OLD_TRADES,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<IEnumerable<RecentTradeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CandlestickModel>> GetCandlestickAsync(
            Dictionary<string, object> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.CANDLESTICK_DATA,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new CandleStickModelEnumerableConverter());
            return converter.Deserialize<IEnumerable<CandlestickModel>>(result);
        }

        /// <inheritdoc />
        public async Task<AveragePriceModel> GetAveragePriceAsync(Dictionary<string, object> query, CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.AVERAGE_PRICE,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            var converter = new JsonDeserializerWrapper();
            return converter.Deserialize<AveragePriceModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default)
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

            var converter = new JsonDeserializerWrapper();
            if (!isNull)
            {
                var result = new List<DayPriceChangeModel>
                {
                    converter.Deserialize<DayPriceChangeModel>(responce)
                };

                return result;
            }

            return converter.Deserialize<IEnumerable<DayPriceChangeModel>>(responce);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolPriceTickerModel>> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default)
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

            var converter = new JsonDeserializerWrapper();
            if (!isNull)
            {
                var result = new List<SymbolPriceTickerModel>
                {
                    converter.Deserialize<SymbolPriceTickerModel>(responce)
                };

                return result;
            }

            return converter.Deserialize<IEnumerable<SymbolPriceTickerModel>>(responce);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolOrderBookTickerModel>> GetSymbolOrderBookTickerAsync(
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

            var converter = new JsonDeserializerWrapper();
            if (!isNull)
            {
                var result = new List<SymbolOrderBookTickerModel>
                {
                    converter.Deserialize<SymbolOrderBookTickerModel>(responce)
                };

                return result;
            }

            return converter.Deserialize<IEnumerable<SymbolOrderBookTickerModel>>(responce);
        }

        #endregion
    }
}
