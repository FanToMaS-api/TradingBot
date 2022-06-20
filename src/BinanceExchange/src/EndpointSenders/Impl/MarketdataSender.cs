using BinanceExchange.Client;
using BinanceExchange.Client.Http.Request.Builders;
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
            _converter.AddConverter(new ExchangeInfoModelConverter()); ;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<ExchangeInfoModel> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.EXCHANGE_INFO)
                .SetHttpMethod(HttpMethod.Get)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

            return _converter.Deserialize<ExchangeInfoModel>(response);
        }

        /// <inheritdoc />
        public async Task<OrderBookModel> GetOrderBookAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.ORDER_BOOK)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

            return _converter.Deserialize<OrderBookModel>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeModel>> GetRecentTradesAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.RECENT_TRADES)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

            return _converter.Deserialize<IEnumerable<TradeModel>>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeModel>> GetOldTradesAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.OLD_TRADES)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

            return _converter.Deserialize<IEnumerable<TradeModel>>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CandlestickModel>> GetCandlestickAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.CANDLESTICK_DATA)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

            return _converter.Deserialize<IEnumerable<CandlestickModel>>(response);
        }

        /// <inheritdoc />
        public async Task<AveragePriceModel> GetAveragePriceAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.AVERAGE_PRICE)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

            return _converter.Deserialize<AveragePriceModel>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            var isNull = string.IsNullOrEmpty(symbol);
            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
            };
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.DAY_PRICE_CHANGE)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

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
            var builder = new HttpRequestUrlBuilder();
            if (!isNull)
            {
                builder.AddParameter("symbol", symbol);
            }

            var request = builder
                .SetEndpoint(BinanceEndpoints.SYMBOL_PRICE_TICKER)
                .SetHttpMethod(HttpMethod.Get)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

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
            var builder = new HttpRequestUrlBuilder();
            if (!isNull)
            {
                builder.AddParameter("symbol", symbol);
            }

            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.SYMBOL_ORDER_BOOK_TICKER)
                .SetHttpMethod(HttpMethod.Get)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

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
