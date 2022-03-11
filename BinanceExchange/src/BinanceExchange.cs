﻿using AutoMapper;
using BinanceExchange.Client;
using BinanceExchange.Client.Impl;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Enums;
using BinanceExchange.Enums.Helper;
using BinanceExchange.Exceptions;
using BinanceExchange.Models;
using BinanceExchange.WebSocket;
using BinanceExchange.WebSocket.Marketdata;
using Common.Enums;
using Common.JsonConvertWrapper;
using Common.JsonConvertWrapper.Converters;
using Common.Models;
using Common.Redis;
using ExchangeLibrary;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange
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

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BinanceMapperProfile>();
            });
            _mapper = new Mapper(config);
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
        public async Task<bool> GetSystemStatusAsync(CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SistemStatusWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _walletSender.GetSystemStatusAsync(cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return result.Status == 0 && result.Message == "normal";
        }

        /// <inheritdoc />
        public async Task<TradingAccountInfoModel> GetAccountTradingStatusAsync(long recvWindow = 5000, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.AccountStatusWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();
            var result = await _walletSender.GetAccountTradingStatusAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<TradingAccountInfoModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.TradeFeeModel> GetTradeFeeAsync(
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

            return _mapper.Map<Common.Models.TradeFeeModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CoinModel>> GetAllCoinsInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default)
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

            return _mapper.Map<IEnumerable<Common.Models.CoinModel>>(result);
        }

        #endregion

        #region Marketdata

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolRuleTradingModel>> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.ExchangeInfoWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var model = await _marketdataSender.GetExchangeInfoAsync(new Dictionary<string, object>(), cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());
            var result = new List<SymbolRuleTradingModel>();
            foreach (var symbol in model.Symbols)
            {
                result.Add(_mapper.Map<SymbolRuleTradingModel>(symbol));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<Common.Models.OrderBookModel> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.OrderBookWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetLimit(limit);
            var query = builder.GetResult(false).GetQuery();
            var result = await _marketdataSender.GetOrderBookAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, limit.ToString());

            return _mapper.Map<Common.Models.OrderBookModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.TradeModel>> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.RecentTradesWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetLimit(limit);
            var query = builder.GetResult(false).GetQuery();
            var models = await _marketdataSender.GetRecentTradesAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            var result = new List<Common.Models.TradeModel>();
            foreach (var model in models)
            {
                result.Add(_mapper.Map<Common.Models.TradeModel>(model));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.TradeModel>> GetOldTradesAsync(
            string symbol,
            long? fromId = null,
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
            if (fromId.HasValue)
            {
                builder.SetFromId(fromId.Value);
            }

            var query = builder.GetResult(false).GetQuery();
            var models = await _marketdataSender.GetOldTradesAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            var result = new List<Common.Models.TradeModel>();
            foreach (var model in models)
            {
                result.Add(_mapper.Map<Common.Models.TradeModel>(model));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CandlestickModel>> GetCandlstickAsync(
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

            var query = builder.GetResult(false).GetQuery();
            var models = await _marketdataSender.GetCandlestickAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            var result = new List<Common.Models.CandlestickModel>();
            foreach (var model in models)
            {
                result.Add(_mapper.Map<Common.Models.CandlestickModel>(model));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<double> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default)
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

            return result.AveragePrice;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.DayPriceChangeModel>> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.DayTickerPriceChangeWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var models = await _marketdataSender.GetDayPriceChangeAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);
            var result = new List<Common.Models.DayPriceChangeModel>();
            foreach (var item in models)
            {
                result.Add(_mapper.Map<Common.Models.DayPriceChangeModel>(item));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolPriceModel>> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SymbolPriceTickerWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var models = await _marketdataSender.GetSymbolPriceTickerAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);
            var result = new List<SymbolPriceModel>();
            foreach (var item in models)
            {
                result.Add(_mapper.Map<SymbolPriceModel>(item));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BestSymbolOrderModel>> GetSymbolOrderBookTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SymbolOrderBookTickerWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var models = await _marketdataSender.GetSymbolOrderBookTickerAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);
            var result = new List<BestSymbolOrderModel>();
            foreach (var item in models)
            {
                result.Add(_mapper.Map<BestSymbolOrderModel>(item));
            }

            return result;
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
                       MarketdataStreamType.AggregateTradeStream => converter.Deserialize<Models.AggregateSymbolTradeStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolBookTickerStream => converter.Deserialize<Models.BookTickerStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolMiniTickerStream => converter.Deserialize<Models.MiniTickerStreamModel>(content),
                       MarketdataStreamType.TradeStream => converter.Deserialize<Models.SymbolTradeStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolTickerStream => converter.Deserialize<Models.TickerStreamModel>(content),
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
        public async Task SubscribeCandlestickStreamAsync(
            string symbol,
            string candleStickInterval,
            Func<Common.Models.CandlestickStreamModel, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default)
        {
            var interval = candleStickInterval.ConvertToCandleStickIntervalType();
            var webSoket = MarketdataWebSocket.CreateCandlestickStream(symbol, interval);
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var model = converter.Deserialize<Models.CandlestickStreamModel>(content);
                   var neededModel = _mapper.Map<Common.Models.CandlestickStreamModel>(model);
                   onMessageReceivedFunc?.Invoke(neededModel);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribeAllMarketTickersStreamAsync(
            Func<IEnumerable<Common.Models.TickerStreamModel>, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default)
        {
            var webSoket = MarketdataWebSocket.CreateAllMarketMiniTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<Models.TickerStreamModel>());

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var models = converter.Deserialize<IEnumerable<Models.TickerStreamModel>>(content);
                   var neededModels = new List<Common.Models.TickerStreamModel>();
                   foreach (var model in models)
                   {
                       neededModels.Add(_mapper.Map<Common.Models.TickerStreamModel>(model));
                   }

                   onMessageReceivedFunc?.Invoke(neededModels);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribeAllBookTickersStreamAsync(
            Func<IEnumerable<Common.Models.BookTickerStreamModel>, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default)
        {
            var webSoket = MarketdataWebSocket.CreateAllBookTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<Models.BookTickerStreamModel>());

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var models = converter.Deserialize<IEnumerable<Models.BookTickerStreamModel>>(content);
                   var neededModels = new List<Common.Models.BookTickerStreamModel>();
                   foreach (var model in models)
                   {
                       neededModels.Add(_mapper.Map<Common.Models.BookTickerStreamModel>(model));
                   }

                   onMessageReceivedFunc?.Invoke(neededModels);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribeAllMarketMiniTickersStreamAsync(
            Func<IEnumerable<Common.Models.MiniTickerStreamModel>, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken = default)
        {
            var webSoket = MarketdataWebSocket.CreateAllMarketMiniTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<Models.MiniTickerStreamModel>());

            webSoket.AddOnMessageReceivedFunc(
               content =>
               {
                   var models = converter.Deserialize<IEnumerable<Models.MiniTickerStreamModel>>(content);
                   var neededModels = new List<Common.Models.MiniTickerStreamModel>();
                   foreach (var model in models)
                   {
                       neededModels.Add(_mapper.Map<Common.Models.MiniTickerStreamModel>(model));
                   }

                   onMessageReceivedFunc?.Invoke(neededModels);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubscribePartialBookDepthStreamAsync(
            string symbol,
            Func<Common.Models.OrderBookModel, Task> onMessageReceivedFunc,
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
                   var model = converter.Deserialize<Models.OrderBookModel>(content);
                   var neededModel = _mapper.Map<Common.Models.OrderBookModel>(model);
                   onMessageReceivedFunc?.Invoke(neededModel);
                   return Task.CompletedTask;
               },
               cancellationToken);

            await webSoket.ConnectAsync(cancellationToken);
        }

        #endregion

        #region Spot Account/Trade

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewLimitOrderAsync(
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

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewMarketOrderAsync(
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

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewStopLossOrderAsync(
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

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewStopLossLimitOrderAsync(
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

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewTakeProfitOrderAsync(
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

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewTakeProfitLimitOrderAsync(
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

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewLimitMakerOrderAsync(
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

            return _mapper.Map<Common.Models.FullOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.CancelOrderResponseModel> CancelOrderAsync(
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
            builder.SetRecvWindow(recvWindow);
            if (orderId.HasValue)
            {
                builder.SetOrderId(orderId.Value);
            }

            if (!string.IsNullOrEmpty(origClientOrderId))
            {
                builder.SetOrigClientOrderId(origClientOrderId);
            }

            var query = builder.GetResult().GetQuery();

            var result = await _tradeSender.CancelOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.CancelOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CancelOrderResponseModel>> CancelAllOrdersAsync(
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

            var models = await _tradeSender.CancelAllOrdersAsync(query, cancellationToken);
            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            var result = new List<Common.Models.CancelOrderResponseModel>();
            foreach(var model in models)
            {
                result.Add(_mapper.Map<Common.Models.CancelOrderResponseModel>(model));
            }
                
            return result;
        }

        /// <inheritdoc />
        public async Task<Common.Models.CheckOrderResponseModel> CheckOrderAsync(
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
            builder.SetRecvWindow(recvWindow);
            if (orderId.HasValue)
            {
                builder.SetOrderId(orderId.Value);
            }

            if (!string.IsNullOrEmpty(origClientOrderId))
            {
                builder.SetOrigClientOrderId(origClientOrderId);
            }

            var query = builder.GetResult().GetQuery();
            var result = await _tradeSender.CheckOrderAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.CheckOrderResponseModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CheckOrderResponseModel>> CheckAllOpenOrdersAsync(
            string symbol,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CheckAllOpenOrdersWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetRecvWindow(recvWindow);
            var isSymbolOmitted = string.IsNullOrEmpty(symbol);
            if (!isSymbolOmitted)
            {
                builder.SetSymbol(symbol);
            }

            var query = builder.GetResult().GetQuery();
            var models = await _tradeSender.CheckAllOpenOrdersAsync(query, cancellationToken);

            var key = isSymbolOmitted ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            var result = new List<Common.Models.CheckOrderResponseModel>();
            foreach (var model in models)
            {
                result.Add(_mapper.Map<Common.Models.CheckOrderResponseModel>(model));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CheckOrderResponseModel>> GetAllOrdersAsync(
            string symbol,
            long? orderId = null,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.AllOrdersWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder();
            builder.SetSymbol(symbol);
            builder.SetRecvWindow(recvWindow);
            builder.SetLimit(limit);
            if (orderId.HasValue)
            {
                builder.SetOrderId(orderId.Value);
            }

            if (startTime.HasValue)
            {
                builder.SetStartTime(startTime.Value);
            }

            if (endTime.HasValue)
            {
                builder.SetEndTime(endTime.Value);
            }

            var query = builder.GetResult().GetQuery();
            var models = await _tradeSender.GetAllOrdersAsync(query, cancellationToken);
            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            var result = new List<Common.Models.CheckOrderResponseModel>();
            foreach (var model in models)
            {
                result.Add(_mapper.Map<Common.Models.CheckOrderResponseModel>(model));
            }

            return result;
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
            _logger.Error($"WebSocket: {webSocket} was closed");
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
