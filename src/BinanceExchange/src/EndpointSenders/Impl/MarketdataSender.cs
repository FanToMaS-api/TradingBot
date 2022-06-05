using BinanceExchange.Client;
using BinanceExchange.JsonConverters;
using BinanceExchange.Models;
using Common.JsonConvertWrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.EndpointSenders.Impl
{
    internal class MarketdataSender : IMarketdataSender
    {
        #region Fields

        private readonly IBinanceClient _client;
        private readonly JsonDeserializerWrapper _converter;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataSender" />
        public MarketdataSender(IBinanceClient client)
        {
            _client = client;
            _converter = new JsonDeserializerWrapper();
            _converter.AddConverter(new OrderBookModelConverter());
            _converter.AddConverter(new CandlestickModelEnumerableConverter());
            _converter.AddConverter(new ExchangeInfoModelConverter());;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<ExchangeInfoModel> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.EXCHANGE_INFO,
                HttpMethod.Get,
                cancellationToken: cancellationToken);

            return _converter.Deserialize<ExchangeInfoModel>(result);
        }

        /// <inheritdoc />
        public async Task<OrderBookModel> GetOrderBookAsync(
            Dictionary<string, object> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.ORDER_BOOK,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            return _converter.Deserialize<OrderBookModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeModel>> GetRecentTradesAsync(
            Dictionary<string, object> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.RECENT_TRADES,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            return _converter.Deserialize<IEnumerable<TradeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeModel>> GetOldTradesAsync(
            Dictionary<string, object> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.OLD_TRADES,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            return _converter.Deserialize<IEnumerable<TradeModel>>(result);
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

            return _converter.Deserialize<IEnumerable<CandlestickModel>>(result);
        }

        /// <inheritdoc />
        public async Task<AveragePriceModel> GetAveragePriceAsync(
            Dictionary<string, object> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _client.SendPublicAsync(
                BinanceEndpoints.AVERAGE_PRICE,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken);

            return _converter.Deserialize<AveragePriceModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            var isNull = string.IsNullOrEmpty(symbol);
            var response = await _client.SendPublicAsync(
                BinanceEndpoints.DAY_PRICE_CHANGE,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                },
                cancellationToken: cancellationToken);

            if (!isNull)
            {
                var result = new List<DayPriceChangeModel>
                {
                    _converter.Deserialize<DayPriceChangeModel>(response)
                };

                return result;
            }

            return _converter.Deserialize<IEnumerable<DayPriceChangeModel>>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolPriceTickerModel>> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var isNull = string.IsNullOrEmpty(symbol);
            var response = await _client.SendPublicAsync(
                BinanceEndpoints.SYMBOL_PRICE_TICKER,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                },
                cancellationToken: cancellationToken);

            if (!isNull)
            {
                var result = new List<SymbolPriceTickerModel>
                {
                    _converter.Deserialize<SymbolPriceTickerModel>(response)
                };

                return result;
            }

            return _converter.Deserialize<IEnumerable<SymbolPriceTickerModel>>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolOrderBookTickerModel>> GetSymbolOrderBookTickerAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            var isNull = string.IsNullOrEmpty(symbol);
            var response = await _client.SendPublicAsync(
                BinanceEndpoints.SYMBOL_ORDER_BOOK_TICKER,
                HttpMethod.Get,
                query: new Dictionary<string, object>
                {
                    { "symbol", symbol },
                },
                cancellationToken: cancellationToken);

            if (!isNull)
            {
                var result = new List<SymbolOrderBookTickerModel>
                {
                    _converter.Deserialize<SymbolOrderBookTickerModel>(response)
                };

                return result;
            }

            return _converter.Deserialize<IEnumerable<SymbolOrderBookTickerModel>>(response);
        }

        #endregion
    }
}
