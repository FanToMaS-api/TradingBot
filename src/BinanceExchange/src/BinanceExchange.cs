using AutoMapper;
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
using Common.WebSocket;
using ExchangeLibrary;
using NLog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
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
        private readonly JsonDeserializerWrapper _converter = GetConverter();

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceExchange"/>
        public BinanceExchange(BinanceExchangeOptions exchangeOptions)
        {
            _httpClient = new();
            _redisDatabase = new RedisDatabase(
                ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = { "localhost:6379" }
                }));
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
            var requestWeight = _requestsWeightStorage.SystemStatusWeight;
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
        public async Task<IEnumerable<Common.Models.TradeFeeModel>> GetTradeFeeAsync(
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
            builder.SetSymbol(symbol, true);
            builder.SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();
            var result = await _walletSender.GetTradeFeeAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.TradeFeeModel>>(result);
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

            var model = await _marketdataSender.GetExchangeInfoAsync(cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<SymbolRuleTradingModel>>(model.Symbols);
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
            var result = await _marketdataSender.GetRecentTradesAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.TradeModel>>(result);
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
            var result = await _marketdataSender.GetOldTradesAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.TradeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.CandlestickModel>> GetCandlestickAsync(
            string symbol,
            string interval,
            long? startTime = null,
            long? endTime = null,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.CandlestickDataWeight;
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
            var result = await _marketdataSender.GetCandlestickAsync(query, cancellationToken);

            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.CandlestickModel>>(result);
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

            var result = await _marketdataSender.GetDayPriceChangeAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return _mapper.Map<IEnumerable<Common.Models.DayPriceChangeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SymbolPriceModel>> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SymbolPriceTickerWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetSymbolPriceTickerAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return _mapper.Map<IEnumerable<SymbolPriceModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BestSymbolOrderModel>> GetBestSymbolOrdersAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var requestWeight = _requestsWeightStorage.SymbolOrderBookTickerWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _marketdataSender.GetSymbolOrderBookTickerAsync(symbol, cancellationToken);

            var key = string.IsNullOrEmpty(symbol) ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return _mapper.Map<IEnumerable<BestSymbolOrderModel>>(result);
        }

        #endregion

        #region Marketdata Streams

        /// <inheritdoc />
        /// <remarks>
        ///     Проверено ручным тестированием, после изменений необходимы ручные проверки!
        /// </remarks>
        public IWebSocket SubscribeNewStream<T>(
            string symbol,
            string stream,
            Func<T, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null)
        {
            // TODO переписать или с фабрикой или с интерфейсом для удобного тестирования
            var streamType = stream.ConvertToMarketdataStreamType();
            var webSoket = new MarketdataWebSocket(symbol, streamType);
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   IMarketdataStreamModel model = streamType switch
                   {
                       MarketdataStreamType.AggregateTradeStream => _converter.Deserialize<Models.AggregateSymbolTradeStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolBookTickerStream => _converter.Deserialize<Models.BookTickerStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolMiniTickerStream => _converter.Deserialize<Models.MiniTickerStreamModel>(content),
                       MarketdataStreamType.TradeStream => _converter.Deserialize<Models.SymbolTradeStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolTickerStream => _converter.Deserialize<Models.TickerStreamModel>(content),
                       _ => throw new InvalidOperationException($"Unknown Marketdata Stream Type {streamType}. " +
                       "Failed to deserialize content")
                   };

                   var neededModel = _mapper.Map<T>(model);
                   await onMessageReceivedFunc?.Invoke(neededModel, cancellationToken);
               },
               cancellationToken);

            return webSoket;
        }

        /// <inheritdoc />
        /// <remarks>
        ///     Проверено ручным тестированием, после изменений необходимы ручные проверки!
        /// </remarks>
        public IWebSocket SubscribeCandlestickStream(
            string symbol,
            string candleStickInterval,
            Func<Common.Models.CandlestickStreamModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null)
        {
            // TODO переписать или с фабрикой или с интерфейсом для удобного тестирования
            var interval = candleStickInterval.ConvertToCandleStickIntervalType();
            var webSoket = MarketdataWebSocket.CreateCandlestickStream(symbol, interval);
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   try
                   {
                       var model = _converter.Deserialize<Models.CandlestickStreamModel>(content);
                       var neededModel = _mapper.Map<Common.Models.CandlestickStreamModel>(model);
                       await onMessageReceivedFunc?.Invoke(neededModel, cancellationToken);
                   }
                   catch (Exception ex)
                   {
                       _logger.Warn(ex, $"Failed to recieve {nameof(Common.Models.TickerStreamModel)}");
                   }
               },
               cancellationToken);

            return webSoket;
        }

        /// <inheritdoc />
        /// <remarks>
        ///     Проверено ручным тестированием, после изменений необходимы ручные проверки!
        /// </remarks>
        public IWebSocket SubscribeAllMarketTickersStream(
            Func<IEnumerable<Common.Models.TickerStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null)
        {
            // TODO переписать или с фабрикой или с интерфейсом для удобного тестирования
            var webSoket = MarketdataWebSocket.CreateAllTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   try
                   {
                       var models = _converter.Deserialize<IEnumerable<Models.TickerStreamModel>>(content);
                       var neededModels = _mapper.Map<IEnumerable<Common.Models.TickerStreamModel>>(models);
                       await onMessageReceivedFunc?.Invoke(neededModels, cancellationToken);
                   }
                   catch (Exception ex)
                   {
                       _logger.Warn(ex, $"Failed to recieve {nameof(Common.Models.TickerStreamModel)}");
                   }
               },
               cancellationToken);

            return webSoket;
        }

        /// <inheritdoc />
        /// <remarks>
        ///     Проверено ручным тестированием, после изменений необходимы ручные проверки!
        /// </remarks>
        public IWebSocket SubscribeAllBookTickersStream(
            Func<Common.Models.BookTickerStreamModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null)
        {
            // TODO переписать или с фабрикой или с интерфейсом для удобного тестирования
            var webSoket = MarketdataWebSocket.CreateAllBookTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   try
                   {
                       var model = _converter.Deserialize<Models.BookTickerStreamModel>(content);
                       var neededModel = _mapper.Map<Common.Models.BookTickerStreamModel>(model);
                       await onMessageReceivedFunc?.Invoke(neededModel, cancellationToken);
                   }
                   catch (Exception ex)
                   {
                       _logger.Warn(ex, $"Failed to recieve {nameof(Models.BookTickerStreamModel)}");
                   }
               },
               cancellationToken);

            return webSoket;
        }

        /// <inheritdoc />
        /// <remarks>
        ///     Проверено ручным тестированием, после изменений необходимы ручные проверки!
        /// </remarks>
        public IWebSocket SubscribeAllMarketMiniTickersStream(
            Func<IEnumerable<Common.Models.MiniTickerStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null)
        {
            // TODO переписать или с фабрикой или с интерфейсом для удобного тестирования
            var webSoket = MarketdataWebSocket.CreateAllMarketMiniTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   var models = _converter.Deserialize<IEnumerable<Models.MiniTickerStreamModel>>(content);
                   var neededModels = _mapper.Map<IEnumerable<Common.Models.MiniTickerStreamModel>>(models);

                   await onMessageReceivedFunc?.Invoke(neededModels, cancellationToken);
               },
               cancellationToken);

            return webSoket;
        }

        /// <inheritdoc />
        /// <remarks>
        ///     Проверено ручным тестированием, после изменений необходимы ручные проверки!
        /// </remarks>
        public IWebSocket SubscribePartialBookDepthStream(
            string symbol,
            Func<Common.Models.OrderBookModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null,
            int levels = 10,
            bool activateFastReceive = false)
        {
            // TODO переписать или с фабрикой или с интерфейсом для удобного тестирования
            var webSoket = MarketdataWebSocket.CreatePartialBookDepthStream(symbol, levels, activateFastReceive);
            webSoket.OnStreamClosed += onStreamClosedFunc;
            webSoket.OnClosed += OnCloseHandler;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   var model = _converter.Deserialize<Models.OrderBookModel>(content);
                   var neededModel = _mapper.Map<Common.Models.OrderBookModel>(model);
                   await onMessageReceivedFunc?.Invoke(neededModel, cancellationToken);
               },
               cancellationToken);

            return webSoket;
        }

        #endregion

        #region Spot Account/Trade

        /// <inheritdoc />
        public async Task<Common.Models.FullOrderResponseModel> CreateNewLimitOrderAsync(
            string symbol,
            OrderSideType sideType,
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
            OrderSideType sideType,
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
            OrderSideType sideType,
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
            OrderSideType sideType,
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
            OrderSideType sideType,
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
            OrderSideType sideType,
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
            OrderSideType sideType,
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

            var result = await _tradeSender.CancelAllOrdersAsync(query, cancellationToken);
            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.CancelOrderResponseModel>>(result);
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
            var result = await _tradeSender.CheckAllOpenOrdersAsync(query, cancellationToken);

            var key = isSymbolOmitted ? "null" : RequestWeightModel.GetDefaultKey();
            IncrementCallsMade(requestWeight, key);

            return _mapper.Map<IEnumerable<Common.Models.CheckOrderResponseModel>>(result);
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
            var requestWeight = _requestsWeightStorage.GetAllOrdersWeight;
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
            var result = await _tradeSender.GetAllOrdersAsync(query, cancellationToken);
            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.CheckOrderResponseModel>>(result);
        }

        /// <inheritdoc />
        public async Task<Common.Models.AccountInformation> GetAccountInformationAsync(CancellationToken cancellationToken)
        {
            var requestWeight = _requestsWeightStorage.AccountInformationWeight;
            if (CheckLimit(requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _tradeSender.GetAccountInformationAsync(cancellationToken);
            IncrementCallsMade(requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<Common.Models.AccountInformation>(result);
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
        internal string GetRedisKey(RateLimitType type) => $"tc_binance_{type.ToString().ToLower()}";

        /// <summary>
        ///     Возвращает модель ограничения скорости
        /// </summary>
        internal RateLimit GetRateLimit(ApiType type) =>
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
                if (_redisDatabase.TryIncrementOrCreateKeyValue(key, rateLimit.Interval, value))
                {
                    return;
                }

                throw new Exception($"Failed to increment or create ketValue: key='{key}' interval='{rateLimit.Interval}' value='{value}'");
            }

            throw new KeyNotFoundException($"Key '{weightParamKey}' not found.");
        }

        /// <summary>
        ///     Обработчик закрытия стрима
        /// </summary>
        private Task OnCloseHandler(BinanceWebSocket webSocket, CancellationToken cancellationToken = default)
        {
            _logger.Error($"WebSocket: {webSocket} was closed");
            webSocket.OnClosed = null;
            webSocket.OnStreamClosed = null;
            webSocket.Dispose();

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Настраивает профили оболочки десериализации объектов
        /// </summary>
        internal static JsonDeserializerWrapper GetConverter()
        {
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<Models.TickerStreamModel>());
            converter.AddConverter(new EnumerableDeserializer<Models.BookTickerStreamModel>());
            converter.AddConverter(new EnumerableDeserializer<Models.MiniTickerStreamModel>());
            converter.AddConverter(new OrderBookModelConverter());

            return converter;
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
            _redisDatabase?.Dispose();
            _httpClient.Dispose();
        }

        #endregion
    }
}
