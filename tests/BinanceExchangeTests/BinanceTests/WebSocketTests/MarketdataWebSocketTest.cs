﻿using BinanceExchange.Enums;
using BinanceExchange.Enums.Helper;
using BinanceExchange.Impl;
using BinanceExchange.Models;
using BinanceExchange.WebSocket;
using BinanceExchange.WebSocket.Marketdata;
using Common.JsonConvertWrapper;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SharedForTest;

namespace BinanceExchangeTests.BinanceTests.WebSocketTests
{
    /// <summary>
    ///     Тестирует <see cref="MarketdataWebSocket"/>
    /// </summary>
    public class MarketdataWebSocketTest : IDisposable
    {
        #region Fields

        private readonly JsonDeserializerWrapper _deserializer = MarketdataStreams.GetConverter();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly BookTickerStreamModel _expectedBookTickerStreamModel = new()
        {
            OrderBookUpdatedId = 400900217,
            Symbol = "BNBUSDT",
            BestBidPrice = 25.35190000,
            BestBidQuantity = 31.21000000,
            BestAskPrice = 25.36520000,
            BestAskQuantity = 40.66000000,
        };

        private readonly TickerStreamModel _expectedTickerStreamModel = new()
        {
            Symbol = "BNBBTC",
            EventTimeUnix = 123456789,
            Price = 0.0015,
            PricePercentChange = 250.00,
            WeightedAveragePrice = 0.0018,
            FirstPrice = 0.0009,
            LastPrice = 0.0025,
            LastQuantity = 10,
            BestBidPrice = 0.0024,
            BestBidQuantity = 10,
            BestAskPrice = 0.0026,
            BestAskQuantity = 100,
            OpenPrice = 0.0010,
            MaxPrice = 0.0025,
            MinPrice = 0.0010,
            AllBaseVolume = 10000,
            AllQuoteVolume = 18,
            StatisticOpenTimeUnix = 1,
            StatisticCloseTimeUnix = 86400000,
            FirstTradeId = 2,
            LastTradeId = 18150,
            TradeNumber = 18151
        };

        private readonly MiniTickerStreamModel _expectedMiniTickerStreamModel = new()
        {
            EventTimeUnix = 123456789,
            Symbol = "BNBBTC",
            ClosePrice = 0.0025,
            OpenPrice = 0.0010,
            MinPrice = 0.0008,
            MaxPrice = 0.0025,
            BasePurchaseVolume = 10000,
            QuotePurchaseVolume = 18
        };

        #endregion

        #region Public methods

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.AggregateTradeStream"/>
        /// </summary>
        [Fact(DisplayName = "Aggregate trade stream subscription Test")]
        public async Task SubscriptionAggregateTradeStream_Test()
        {
            var expected = new AggregateSymbolTradeStreamModel
            {
                Symbol = "BNBBTC",
                EventTimeUnix = 123456789,
                AggregateTradeId = 12345,
                Price = 0.001,
                Quantity = 100,
                FirstTradeId = 100,
                LastTradeId = 105,
                TradeTimeUnix = 123456785,
                IsMarketMaker = true,
            };
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/AggregateTradeStreams.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = new MarketdataWebSocket(
                url,
                MarketdataStreamType.AggregateTradeStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<AggregateSymbolTradeStreamModel>(content);
                    TestExtensions.CheckingAssertions(expected, actual);

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        #region Book ticker stream tests

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.IndividualSymbolBookTickerStream"/>
        /// </summary>
        [Fact(DisplayName = "Individual symbol book ticker stream subscription Test")]
        public async Task SubscriptionBookTickerStream_Test()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/IndividualSymbolBookTickerStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = new MarketdataWebSocket(
                url,
                MarketdataStreamType.IndividualSymbolBookTickerStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<BookTickerStreamModel>(content);
                    TestExtensions.CheckingAssertions(_expectedBookTickerStreamModel, actual);

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.AllBookTickersStream"/>
        /// </summary>
        [Fact(DisplayName = "All book tickers stream subscription Test")]
        public async Task SubscriptionAllBookTickerStream_Test()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/AllBookTickersStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket.CreateAllBookTickersStream(webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<IEnumerable<BookTickerStreamModel>>(content);
                    var actualList = actual.ToList();
                    for (var j = 0; j < actualList.Count; j++)
                    {
                        TestExtensions.CheckingAssertions(_expectedBookTickerStreamModel, actualList[j]);
                    }

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        #endregion

        #region Candlestick stream test

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.CandlestickStream"/>
        /// </summary>
        [Fact(DisplayName = "Candlestick stream subscription Test")]
        public async Task SubscriptionCandlestickStream_Test()
        {
            var expected = new CandlestickStreamModel
            {
                EventTimeUnix = 123456789,
                Symbol = "BNBBTC",
                Kline = new KlineModel
                {
                    KineStartTimeUnix = 123400000,
                    KineStopTimeUnix = 123460000,
                    Symbol = "BNBBTC",
                    interval = "1m",
                    FirstTradeId = 100,
                    LastTradeId = 200,
                    OpenPrice = 0.0010,
                    MinPrice = 0.0015,
                    MaxPrice = 0.0025,
                    ClosePrice = 0.0020,
                    Volume = 1000,
                    IsKlineClosed = true,
                    QuoteAssetVolume = 1.0000,
                    TradesNumber = 100,
                    BasePurchaseVolume = 500,
                    QuotePurchaseVolume = 0.500
                }
            };
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/CandlestickStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket.CreateCandlestickStream(
                url,
                CandlestickIntervalType.OneMinute,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<CandlestickStreamModel>(content);
                    Assert.Equal(expected.EventTimeUnix, actual.EventTimeUnix);
                    Assert.Equal(expected.Symbol, actual.Symbol);

                    TestExtensions.CheckingAssertions(expected.Kline, actual.Kline);

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        #endregion

        #region Mini ticker stream tests

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.IndividualSymbolMiniTickerStream"/>
        /// </summary>
        [Fact(DisplayName = "Individual symbol mini ticker stream subscription Test")]
        public async Task SubscriptionMiniTickerStream_Test()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/IndividualSymbolMiniTickerStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = new MarketdataWebSocket(
                url,
                MarketdataStreamType.IndividualSymbolMiniTickerStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<MiniTickerStreamModel>(content);
                    TestExtensions.CheckingAssertions(_expectedMiniTickerStreamModel, actual);

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.AllMarketMiniTickersStream"/>
        /// </summary>
        [Fact(DisplayName = "All market mini tickers stream subscription Test")]
        public async Task SubscriptionAllMiniTickerStream_Test()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/AllMarketMiniTickersStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket.CreateAllMarketMiniTickersStream(webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<IEnumerable<MiniTickerStreamModel>>(content);
                    var properties = typeof(MiniTickerStreamModel).GetProperties();
                    var actualList = actual.ToList();
                    for (var j = 0; j < actualList.Count; j++)
                    {
                        TestExtensions.CheckingAssertions(_expectedMiniTickerStreamModel, actualList[j]);
                    }

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        #endregion

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.PartialBookDepthStream"/>
        /// </summary>
        [Fact(DisplayName = "Partial book depth stream subscription Test")]
        public async Task SubscriptionPartialBookDepthStream_Test()
        {
            var expected = new OrderBookModel
            {
                LastUpdateId = 160,
                Bids = new List<PriceQtyPair>
                {
                    new PriceQtyPair
                    {
                        Price = 0.0024,
                        Quantity = 10
                    },
                    new PriceQtyPair
                    {
                        Price = 0.0025,
                        Quantity = 11
                    },
                },
                Asks = new List<PriceQtyPair>
                {
                    new PriceQtyPair
                    {
                        Price = 0.0026,
                        Quantity = 100
                    },
                    new PriceQtyPair
                    {
                        Price = 0.0027,
                        Quantity = 111
                    },
                }
            };
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/PartialBookDepthStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket.CreatePartialBookDepthStream(
                url,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<OrderBookModel>(content);
                    Assert.Equal(expected.Bids.Count, actual.Bids.Count);
                    Assert.Equal(expected.Asks.Count, actual.Asks.Count);
                    Assert.Equal(expected.LastUpdateId, actual.LastUpdateId);
                    Assert.Equal(expected.Bids[0].Price, actual.Bids[0].Price);
                    Assert.Equal(expected.Bids[0].Quantity, actual.Bids[0].Quantity);
                    Assert.Equal(expected.Bids[1].Price, actual.Bids[1].Price);
                    Assert.Equal(expected.Bids[1].Quantity, actual.Bids[1].Quantity);
                    Assert.Equal(expected.Asks[0].Price, actual.Asks[0].Price);
                    Assert.Equal(expected.Asks[0].Quantity, actual.Asks[0].Quantity);
                    Assert.Equal(expected.Asks[1].Price, actual.Asks[1].Price);
                    Assert.Equal(expected.Asks[1].Quantity, actual.Asks[1].Quantity);

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.TradeStream"/>
        /// </summary>
        [Fact(DisplayName = "Trade stream subscription Test")]
        public async Task SubscriptionTradeStream_Test()
        {
            var expected = new SymbolTradeStreamModel
            {
                Symbol = "BNBBTC",
                EventTimeUnix = 123456789,
                SellerOrderId = 50,
                BuyerOrderId = 88,
                Price = 0.001,
                Quantity = 100,
                TradeTimeUnix = 123456785,
                IsMarketMaker = true,
                TradeId = 12345
            };
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/TradeStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = new MarketdataWebSocket(
                url,
                MarketdataStreamType.TradeStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<SymbolTradeStreamModel>(content);
                    TestExtensions.CheckingAssertions(expected, actual);

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        #region Market ticker stream tests

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.IndividualSymbolTickerStream"/>
        /// </summary>
        [Fact(DisplayName = "Individual symbol ticker stream subscription Test")]
        public async Task SubscriptionIndividualSymbolTickerStream_Test()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/IndividualSymbolTickerStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = new MarketdataWebSocket(
                url,
                MarketdataStreamType.IndividualSymbolTickerStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<TickerStreamModel>(content);
                    TestExtensions.CheckingAssertions(_expectedTickerStreamModel, actual);

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.AllMarketTickersStream"/>
        /// </summary>
        [Fact(DisplayName = "All market tickers stream subscription Test")]
        public async Task SubscriptionAllMarketTickersStream_Test()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/AllMarketTickersStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket.CreateAllTickersStream(
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = _deserializer.Deserialize<IEnumerable<TickerStreamModel>>(content);
                    var actualList = actual.ToList();
                    for (var j = 0; j < actualList.Count; j++)
                    {
                        TestExtensions.CheckingAssertions(_expectedTickerStreamModel, actualList[j]);
                    }

                    await webSocket.DisconnectAsync(_cancellationTokenSource.Token);
                },
                _cancellationTokenSource.Token);

            // Act
            await webSocket.ConnectAsync(_cancellationTokenSource.Token);
        }

        #endregion

        /// <summary>
        ///     Проверка верной конвертации значений периодов свечей
        /// </summary>
        [Fact(DisplayName = "Candlesticks Interval Convertation Test")]
        public void CandlestickIntervalConversion_Test()
        {
            var intervalstringViews = new List<string>
            { "1m", "3m", "5m", "15m", "30m", "1h", "2h", "4h", "6h", "8h", "12h", "1d", "3d", "1w", "1M" };
            var intervals = new List<CandlestickIntervalType>
            {

                CandlestickIntervalType.OneMinute,
                CandlestickIntervalType.ThreeMinutes,
                CandlestickIntervalType.FiveMinutes,
                CandlestickIntervalType.FifteenMinutes,
                CandlestickIntervalType.ThirtyMinutes,
                CandlestickIntervalType.OneHour,
                CandlestickIntervalType.TwoHour,
                CandlestickIntervalType.FourHours,
                CandlestickIntervalType.SixHours,
                CandlestickIntervalType.EightHours,
                CandlestickIntervalType.TwelveHours,
                CandlestickIntervalType.OneDay,
                CandlestickIntervalType.ThreeDays,
                CandlestickIntervalType.OneWeek,
                CandlestickIntervalType.OneMonth
            };

            for (var i = 0; i < intervalstringViews.Count; i++)
            {
                Assert.Equal(intervalstringViews[i], intervals[i].ToUrl());
                Assert.Equal(intervalstringViews[i].ConvertToCandleStickIntervalType(), intervals[i]);
            }
        }

        #endregion

        #region Implmentation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Возвращает содержимое файла в массиве байтов
        /// </summary>
        /// <param name="filePath">  путь к файлу с ответом со стрима </param>
        private static byte[] GetBytes(string filePath)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, filePath);
            var content = File.ReadAllText(path);

            return Encoding.UTF8.GetBytes(content);
        }

        /// <summary>
        ///     Возвращает заглушку для <see cref="IBinanceWebSocketHumble"/>
        /// </summary>
        /// <param name="url"> url подписки на стрим </param>
        /// <param name="bytes"> Данные </param>
        internal IBinanceWebSocketHumble GetMockingBinanceWebHumble(string url, byte[] bytes)
        {
            var binanceWebSocketHumble = Substitute.For<IBinanceWebSocketHumble>();
            binanceWebSocketHumble.ConnectAsync(new Uri(url), _cancellationTokenSource.Token).Returns(Task.CompletedTask);
            binanceWebSocketHumble.ReceiveAsync(Arg.Any<ArraySegment<byte>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    for (var i = 0; i < bytes.Length; i++)
                    {
                        callInfo.ArgAt<ArraySegment<byte>>(0)[i] = bytes[i];
                    }

                    return new WebSocketReceiveResult(bytes.Length, WebSocketMessageType.Text, true);
                });

            return binanceWebSocketHumble;
        }

        #endregion
    }
}
