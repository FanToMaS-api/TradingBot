using AutoMapper;
using Common.Enums;
using Common.JsonConvertWrapper;
using Common.JsonConvertWrapper.Converters;
using Common.Models;
using Common.Redis;
using ExchangeLibrary;
using ExchangeLibrary.Binance;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Client.Impl;
using ExchangeLibrary.Binance.EndpointSenders;
using ExchangeLibrary.Binance.EndpointSenders.Impl;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using ExchangeLibrary.Binance.Exceptions;
using ExchangeLibrary.Binance.Models;
using ExchangeLibrary.Binance.WebSocket;
using ExchangeLibrary.Binance.WebSocket.Marketdata;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TraidingBot.Exchanges.Binance
{
    /// <summary>
    ///     Binance биржа
    /// </summary>
    internal class BinanceExchange : IExchange, IDisposable
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IBinanceClient _client;
        private readonly IWalletSender _walletSender;
        private readonly IMarketdataSender _marketdataSender;
        private readonly ISpotAccountTradeSender _tradeSender;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private bool _isDisposed;
        private readonly RateLimitStorage _rateLimitsStorage = new();
        private readonly RequestsWeightStorage _requestsWeightStorage = new();

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceExchange"/>
        public BinanceExchange(BinanceExchangeOptions exchangeOptions)
        {
            _httpClient = new();
            _redisDatabase = new RedisDatabase();
            _client = new BinanceClient(_httpClient, exchangeOptions.ApiKey, exchangeOptions.SecretKey);
            _walletSender = new WalletSender(_client);
            _marketdataSender = new MarketdataSender(_client);
            _tradeSender = new SpotAccountTradeSender(_client);
        }

        /// <inheritdoc cref="BinanceExchange"/>
        public BinanceExchange(
            IWalletSender walletSender,
            IMarketdataSender marketdataSender,
            ISpotAccountTradeSender tradeSender,
            IRedisDatabase redisDatabase,
            IMapper mapper)
        {
            _redisDatabase = redisDatabase;
            _walletSender = walletSender;
            _marketdataSender = marketdataSender;
            _tradeSender = tradeSender;
            _mapper = mapper;
        }

        #endregion

        #region Wallet

        /// <inheritdoc />
        public async Task<string> GetSystemStatusAsync(CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SistemStatusWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _walletSender.GetSystemStatusAsync(cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetAccountTraidingStatusAsync(long recvWindow = 5000, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.AccountStatusWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();
            var result = await _walletSender.GetAccountTraidingStatusAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetTradeFeeAsync(
            string symbol = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.TradeFeeWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();
            var result = await _walletSender.GetTradeFeeAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ITrade>> GetAllCoinsInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.AllCoinsInfoWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();
            var result = await _walletSender.GetAllCoinsInformationAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<ITrade>>(result);
        }

        #endregion

        #region Marketdata

        /// <inheritdoc />
        public async Task<string> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.OrderBookWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetLimit(limit);
            var query = builder.GetResult().GetQuery();
            var result = await _marketdataSender.GetOrderBookAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, limit.ToString());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.RecentTradesWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetLimit(limit);
            var query = builder.GetResult().GetQuery();
            var result = await _marketdataSender.GetRecentTradesAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetOldTradesAsync(
            string symbol,
            long fromId,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.OldTradesWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetLimit(limit);
            builder.SetFromId(fromId);
            var query = builder.GetResult().GetQuery();
            var result = await _marketdataSender.GetOldTradesAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetCandlstickAsync(
            string symbol,
            string interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CandleStickDataWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetLimit(limit);
            builder.SetCandlestickInterval(interval);
            if (startTime.HasValue)
            {
                builder.SetStartTime(startTime.Value);
            }

            if (endTime.HasValue)
            {
                builder.SetEndTime(endTime.Value);
            }

            var query = builder.GetResult().GetQuery();
            var result = await _marketdataSender.GetCandlestickAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CurrentAveragePriceWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }
            var builder = new Builder();
            builder.SetSymbol(symbol);
            var query = builder.GetResult().GetQuery();
            var result = await _marketdataSender.GetAveragePriceAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.DayTickerPriceChangeWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetDayPriceChangeAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SymbolPriceTickerWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetSymbolPriceTickerAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> GetSymbolOrderBookTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SymbolOrderBookTickerWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetSymbolOrderBookTickerAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return "TODO";
        }

        #endregion

        #region Marketdata Streams

        /// <inheritdoc />
        public async Task SubscribeNewStreamAsync<T>(
            string symbol,
            Func<T, Task> onMessageReceivedFunc,
            string stream,
            CancellationToken cancellationToken = default)
        {
            var streamType = stream.ConvertToMarketdataStreamType();
            var webSoket = new MarketdataWebSocket(symbol, streamType);
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   IMarketdataStreamModel model = streamType switch
                   {
                       MarketdataStreamType.AggregateTradeStream => converter.Deserialize<AggregateSymbolTradeStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolBookTickerStream => converter.Deserialize<BookTickerStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolMiniTickerStream => converter.Deserialize<MiniTickerStreamModel>(content),
                       MarketdataStreamType.TradeStream => converter.Deserialize<SymbolTradeStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolTickerStream => converter.Deserialize<TickerStreamModel>(content),
                       _ => throw new InvalidOperationException($"Unknow Marketdata Stream Type {streamType}. " +
                       "Failed to deserialize content")
                   };

                   var neededModel = _mapper.Map<T>(model);
                   onMessageReceivedFunc?.Invoke(neededModel);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribeCandlestickStreamAsync<T>(
            string symbol,
            string candleStickInterval,
            Func<T, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default)
        {
            var interval = candleStickInterval.ConvertToCandleStickIntervalType();
            var webSoket = MarketdataWebSocket.CreateCandlestickStream(symbol, interval);
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var model = converter.Deserialize<CandlestickStreamModel>(content);
                   var neededModel = _mapper.Map<T>(model);
                   onMessageReceivedFunc?.Invoke(neededModel);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribeAllMarketTickersStreamAsync<T>(
            Func<T, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default)
        {
            var webSoket = MarketdataWebSocket.CreateAllMarketMiniTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<TickerStreamModel>());

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var model = converter.Deserialize<IEnumerable<TickerStreamModel>>(content);
                   var neededModel = _mapper.Map<T>(model);
                   onMessageReceivedFunc?.Invoke(neededModel);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribeAllBookTickersStreamAsync<T>(
            Func<T, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default)
        {
            var webSoket = MarketdataWebSocket.CreateAllBookTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<BookTickerStreamModel>());

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var model = converter.Deserialize<IEnumerable<BookTickerStreamModel>>(content);
                   var neededModel = _mapper.Map<T>(model);
                   onMessageReceivedFunc?.Invoke(neededModel);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribeAllMarketMiniTickersStreamAsync<T>(
            Func<T, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default)
        {
            var webSoket = MarketdataWebSocket.CreateAllMarketMiniTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<MiniTickerStreamModel>());

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var model = converter.Deserialize<IEnumerable<MiniTickerStreamModel>>(content);
                   var neededModel = _mapper.Map<T>(model);
                   onMessageReceivedFunc?.Invoke(neededModel);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribePartialBookDepthStreamAsync<T>(
            string symbol,
            Func<T, Task> onMessageReceivedFunc,
            int levels = 10,
            bool activateFastReceive = false,
            CancellationToken cancellationToken = default)
        {
            var webSoket = MarketdataWebSocket.CreatePartialBookDepthStream(symbol, levels, activateFastReceive);
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new OrderBookModelConverter());

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var model = converter.Deserialize<OrderBookModel>(content);
                   var neededModel = _mapper.Map<T>(model);
                   onMessageReceivedFunc?.Invoke(neededModel);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        #endregion

        #region Spot Account/Trade

        /// <inheritdoc />
        public async Task<string> CreateNewLimitOrderAsync(
            string symbol,
            string sideType,
            string forceType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.Limit);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetTimeInForce(forceType);
            builder.SetPrice(price);
            builder.SetQuantity(quantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CreateNewMarketOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.Market);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetQuantity(quantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CreateNewStopLossOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.StopLoss);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetStopPrice(stopPrice);
            builder.SetQuantity(quantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CreateNewStopLossLimitOrderAsync(
            string symbol,
            string sideType,
            string forceType,
            double price,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.StopLossLimit);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetTimeInForce(forceType);
            builder.SetPrice(price);
            builder.SetQuantity(quantity);
            builder.SetStopPrice(stopPrice);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CreateNewTakeProfitOrderAsync(
            string symbol,
            string sideType,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.TakeProfit);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetQuantity(quantity);
            builder.SetStopPrice(stopPrice);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CreateNewTakeProfitLimitOrderAsync(
            string symbol,
            string sideType,
            string forceType,
            double price,
            double quantity,
            double stopPrice,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.TakeProfitLimit);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetTimeInForce(forceType);
            builder.SetPrice(price);
            builder.SetQuantity(quantity);
            builder.SetStopPrice(stopPrice);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CreateNewLimitMakerOrderAsync(
            string symbol,
            string sideType,
            double price,
            double quantity,
            long recvWindow = 5000,
            bool isTest = true,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.NewOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetOrderType(OrderType.LimitMaker);
            builder.SetOrderSideType(sideType);
            builder.SetSymbol(symbol);
            builder.SetPrice(price);
            builder.SetQuantity(quantity);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = isTest
                ? await _tradeSender.SendNewTestOrderAsync(query, cancellationToken)
                : await _tradeSender.SendNewOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CancelOrderAsync(
            string symbol,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CancelOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetOrigClientOrderId(origClientOrderId);
            builder.SetRecvWindow(recvWindow);
            if (orderId.HasValue)
            {
                builder.SetOrderId(orderId.Value);
            }

            var query = builder.GetResult().GetQuery();

            var result = await _tradeSender.CancelOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CancelAllOrdersAsync(
            string symbol,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CancelOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();

            var result = await _tradeSender.CancelAllOrdersAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        /// <inheritdoc />
        public async Task<string> CheckOrderAsync(
            string symbol,
            long? orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CheckOrderWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetOrigClientOrderId(origClientOrderId);
            builder.SetRecvWindow(recvWindow);
            if (orderId.HasValue)
            {
                builder.SetOrderId(orderId.Value);
            }

            var query = builder.GetResult().GetQuery();
            var result = await _tradeSender.CheckOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return "TODO";
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверяет на достижение лимита
        /// </summary>
        private bool CheckLimit(ApiType type, out RedisKeyValue<int> keyValue)
        {
            var rateLimit = GetRateLimit(type);
            var key = GetRedisKey(rateLimit.Type);
            if (_redisDatabase.TryGetIntValue(key, out keyValue) && keyValue.Value >= rateLimit.Limit)
            {
                _logger.Warn($"Too many request '{keyValue.Value}' for '{keyValue.Key}' endpoint. Expiration limit '{keyValue.Expiration?.ToString("s")}'");

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Возвращает Redis-ключ для лимита
        /// </summary>
        private string GetRedisKey(RateLimitType type) => $"tc_binance_{type.ToString().ToLower()}";

        private RateLimit GetRateLimit(ApiType type) =>
            type switch
            {
                ApiType.Sapi => _rateLimitsStorage.SapiWeightLimit,
                ApiType.Api => _rateLimitsStorage.ApiWeightLimit,
                _ => throw new Exception($"Unknown type of {nameof(ApiType)}")
            };

        /// <summary>
        ///     Увеличивает кол-во сделанных вызовов или создает новую пару значений
        /// </summary>
        private void IncrementCallsMade(RequestWeightModel requestWeight, string weightParamKey)
        {
            var rateLimit = GetRateLimit(requestWeight.Type);
            var key = GetRedisKey(rateLimit.Type);

            if (requestWeight.Weights.TryGetValue(weightParamKey, out var value))
            {
                _redisDatabase.IncrementOrCreateKeyValue(key, rateLimit.Interval, value);
                return;
            }

            throw new KeyNotFoundException($"Key '{weightParamKey}' not found.");
        }

        /// <summary>
        ///     Обработчик закрытия стрима
        /// </summary>
        private Task OnCloseHandler(BinanceWebSocket webSocket, CancellationToken cancellationToken = default)
        {
            _logger.Error($"{webSocket} was closed");
            webSocket.Dispose();
            return Task.CompletedTask;
        }

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _httpClient.Dispose();
        }

        #endregion
    }
}
