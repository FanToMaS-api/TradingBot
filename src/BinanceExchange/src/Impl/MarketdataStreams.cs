using AutoMapper;
using BinanceExchange.Enums;
using BinanceExchange.Enums.Helper;
using BinanceExchange.JsonConverters;
using BinanceExchange.Models;
using BinanceExchange.WebSocket;
using BinanceExchange.WebSocket.Marketdata;
using Common.JsonConvertWrapper;
using Common.JsonConvertWrapper.Converters;
using Common.WebSocket;
using ExchangeLibrary;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.Impl
{
    /// <inheritdoc cref="IMarketdataStreams"/>
    internal class MarketdataStreams : IMarketdataStreams
    {
        #region Fields

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMapper _mapper;
        private readonly JsonDeserializerWrapper _converter = GetConverter();

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataStreams"/>
        public MarketdataStreams(IMapper mapper) => _mapper = mapper;

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
            var streamType = stream.ConvertToMarketdataStreamType();
            var webSoket = new MarketdataWebSocket(symbol, streamType);
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   IMarketdataStreamModel model = streamType switch
                   {
                       MarketdataStreamType.AggregateTradeStream => _converter.Deserialize<AggregateSymbolTradeStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolBookTickerStream => _converter.Deserialize<BookTickerStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolMiniTickerStream => _converter.Deserialize<MiniTickerStreamModel>(content),
                       MarketdataStreamType.TradeStream => _converter.Deserialize<SymbolTradeStreamModel>(content),
                       MarketdataStreamType.IndividualSymbolTickerStream => _converter.Deserialize<TickerStreamModel>(content),
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
            var interval = candleStickInterval.ConvertToCandleStickIntervalType();
            var webSoket = MarketdataWebSocket.CreateCandlestickStream(symbol, interval);
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   try
                   {
                       var model = _converter.Deserialize<CandlestickStreamModel>(content);
                       var neededModel = _mapper.Map<Common.Models.CandlestickStreamModel>(model);
                       await onMessageReceivedFunc?.Invoke(neededModel, cancellationToken);
                   }
                   catch (Exception ex)
                   {
                       Logger.Warn(ex, $"Failed to recieve {nameof(Common.Models.TradeObjectStreamModel)}");
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
            Func<IEnumerable<Common.Models.TradeObjectStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null)
        {
            var webSoket = MarketdataWebSocket.CreateAllTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   try
                   {
                       var models = _converter.Deserialize<IEnumerable<TickerStreamModel>>(content);
                       var neededModels = _mapper.Map<IEnumerable<Common.Models.TradeObjectStreamModel>>(models);
                       await onMessageReceivedFunc?.Invoke(neededModels, cancellationToken);
                   }
                   catch (Exception ex)
                   {
                       Logger.Warn(ex, $"Failed to recieve {nameof(Common.Models.TradeObjectStreamModel)}");
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
            var webSoket = MarketdataWebSocket.CreateAllBookTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   try
                   {
                       var model = _converter.Deserialize<BookTickerStreamModel>(content);
                       var neededModel = _mapper.Map<Common.Models.BookTickerStreamModel>(model);
                       await onMessageReceivedFunc?.Invoke(neededModel, cancellationToken);
                   }
                   catch (Exception ex)
                   {
                       Logger.Warn(ex, $"Failed to recieve {nameof(Models.BookTickerStreamModel)}");
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
            Func<IEnumerable<Common.Models.MiniTradeObjectStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null)
        {
            var webSoket = MarketdataWebSocket.CreateAllMarketMiniTickersStream();
            webSoket.OnClosed += OnCloseHandler;
            webSoket.OnStreamClosed += onStreamClosedFunc;

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   var models = _converter.Deserialize<IEnumerable<MiniTickerStreamModel>>(content);
                   var neededModels = _mapper.Map<IEnumerable<Common.Models.MiniTradeObjectStreamModel>>(models);

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
            var webSoket = MarketdataWebSocket.CreatePartialBookDepthStream(symbol, levels, activateFastReceive);
            webSoket.OnStreamClosed += onStreamClosedFunc;
            webSoket.OnClosed += OnCloseHandler; // ERROR: чет тут масло на масле!
                                                 // убрать точно одно событие
                                                 // переименовать все событяи по проекту
                                                 // Проследить отписывание от событий!

            webSoket.AddOnMessageReceivedFunc(
               async content =>
               {
                   var model = _converter.Deserialize<OrderBookModel>(content);
                   var neededModel = _mapper.Map<Common.Models.OrderBookModel>(model);
                   await onMessageReceivedFunc?.Invoke(neededModel, cancellationToken);
               },
               cancellationToken);

            return webSoket;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик закрытия стрима
        /// </summary>
        private Task OnCloseHandler(BinanceWebSocket webSocket, CancellationToken cancellationToken = default)
        {
            Logger.Error($"WebSocket: {webSocket} was closed");
            webSocket.OnClosed -= OnCloseHandler;
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
            converter.AddConverter(new EnumerableDeserializer<TickerStreamModel>());
            converter.AddConverter(new EnumerableDeserializer<BookTickerStreamModel>());
            converter.AddConverter(new EnumerableDeserializer<MiniTickerStreamModel>());
            converter.AddConverter(new OrderBookModelConverter());

            return converter;
        }

        #endregion
    }
}
