using Common.JsonConvertWrapper;
using Common.JsonConvertWrapper.Converters;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using ExchangeLibrary.Binance.Models;
using ExchangeLibrary.Binance.WebSocket;
using ExchangeLibrary.Binance.WebSocket.Marketdata;
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

namespace ExchangeLibraryTests.BinanceTests.WebSocket
{
    /// <summary>
    ///     Тестирует <see cref="MarketdataWebSocket"/>
    /// </summary>
    public class MarketdataWebSocketTest
    {
        #region Fields

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
        public async Task SubscriptionAggregateTradeStreamTest()
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
            var converter = new JsonDeserializerWrapper();

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<AggregateSymbolTradeStreamModel>(content);
                    var properties = typeof(AggregateSymbolTradeStreamModel).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(expected), properties[i].GetValue(actual));
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        #region Book ticker stream tests

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.IndividualSymbolBookTickerStream"/>
        /// </summary>
        [Fact(DisplayName = "Individual symbol book ticker stream subscription Test")]
        public async Task SubscriptionBookTickerStreamTest()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/IndividualSymbolBookTickerStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = new MarketdataWebSocket(
                url,
                MarketdataStreamType.IndividualSymbolBookTickerStream,
                webSocketHumbleMock);
            var converter = new JsonDeserializerWrapper();

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<BookTickerStreamModel>(content);
                    var properties = typeof(BookTickerStreamModel).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(_expectedBookTickerStreamModel), properties[i].GetValue(actual));
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.AllBookTickersStream"/>
        /// </summary>
        [Fact(DisplayName = "All book tickers stream subscription Test")]
        public async Task SubscriptionAllBookTickerStreamTest()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/AllBookTickersStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket.CreateAllBookTickersStream(webSocketHumbleMock);
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<BookTickerStreamModel>());

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<IEnumerable<BookTickerStreamModel>>(content);
                    var properties = typeof(BookTickerStreamModel).GetProperties();
                    var actualList = actual.ToList();
                    for (var j = 0; j < actualList.Count; j++)
                    {
                        for (var i = 0; i < properties.Length; i++)
                        {
                            Assert.Equal(properties[i].GetValue(_expectedBookTickerStreamModel), properties[i].GetValue(actualList[j]));
                        }
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        #endregion

        #region Candlestick stream test

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.CandlestickStream"/>
        /// </summary>
        [Fact(DisplayName = "Candlestick stream subscription Test")]
        public async Task SubscriptionCandlestickStreamTest()
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
            var converter = new JsonDeserializerWrapper();

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<CandlestickStreamModel>(content);
                    Assert.Equal(expected.EventTimeUnix, actual.EventTimeUnix);
                    Assert.Equal(expected.Symbol, actual.Symbol);

                    var properties = typeof(KlineModel).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(expected.Kline), properties[i].GetValue(actual.Kline));
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        #endregion

        #region Mini ticker stream tests

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.IndividualSymbolMiniTickerStream"/>
        /// </summary>
        [Fact(DisplayName = "Individual symbol mini ticker stream subscription Test")]
        public async Task SubscriptionMiniTickerStreamTest()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/IndividualSymbolMiniTickerStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = new MarketdataWebSocket(
                url,
                MarketdataStreamType.IndividualSymbolMiniTickerStream,
                webSocketHumbleMock);
            var converter = new JsonDeserializerWrapper();

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<MiniTickerStreamModel>(content);
                    var properties = typeof(MiniTickerStreamModel).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(_expectedMiniTickerStreamModel), properties[i].GetValue(actual));
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.AllMarketMiniTickersStream"/>
        /// </summary>
        [Fact(DisplayName = "All market mini tickers stream subscription Test")]
        public async Task SubscriptionAllMiniTickerStreamTest()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/AllMarketMiniTickersStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket.CreateAllMarketMiniTickersStream(webSocketHumbleMock);
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<MiniTickerStreamModel>());

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<IEnumerable<MiniTickerStreamModel>>(content);
                    var properties = typeof(MiniTickerStreamModel).GetProperties();
                    var actualList = actual.ToList();
                    for (var j = 0; j < actualList.Count; j++)
                    {
                        for (var i = 0; i < properties.Length; i++)
                        {
                            Assert.Equal(properties[i].GetValue(_expectedMiniTickerStreamModel), properties[i].GetValue(actualList[j]));
                        }
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        #endregion

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.PartialBookDepthStream"/>
        /// </summary>
        [Fact(DisplayName = "Partial book depth stream subscription Test")]
        public async Task SubscriptionPartialBookDepthStreamTest()
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

            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new OrderBookModelConverter());

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<OrderBookModel>(content);
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

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.TradeStream"/>
        /// </summary>
        [Fact(DisplayName = "Trade stream subscription Test")]
        public async Task SubscriptionTradeStreamTest()
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
            var converter = new JsonDeserializerWrapper();

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<SymbolTradeStreamModel>(content);
                    var properties = typeof(SymbolTradeStreamModel).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(expected), properties[i].GetValue(actual));
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        #region Market ticker stream tests

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.IndividualSymbolTickerStream"/>
        /// </summary>
        [Fact(DisplayName = "Individual symbol ticker stream subscription Test")]
        public async Task SubscriptionIndividualSymbolTickerStreamTest()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/IndividualSymbolTickerStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = new MarketdataWebSocket(
                url,
                MarketdataStreamType.IndividualSymbolTickerStream,
                webSocketHumbleMock);
            var converter = new JsonDeserializerWrapper();

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<TickerStreamModel>(content);
                    var properties = typeof(TickerStreamModel).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(_expectedTickerStreamModel), properties[i].GetValue(actual));
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.AllMarketTickersStream"/>
        /// </summary>
        [Fact(DisplayName = "All market tickers stream subscription Test")]
        public async Task SubscriptionAllMarketTickersStreamTest()
        {
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/AllMarketTickersStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket.CreateAllTickersStream(
                webSocketHumbleMock);
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<TickerStreamModel>());

            webSocket.AddOnMessageReceivedFunc(
                async (content) =>
                {
                    var actual = converter.Deserialize<IEnumerable<TickerStreamModel>>(content);
                    var actualList = actual.ToList();
                    var properties = typeof(TickerStreamModel).GetProperties();
                    for (var j = 0; j < actualList.Count; j++)
                    {
                        for (var i = 0; i < properties.Length; i++)
                        {
                            Assert.Equal(properties[i].GetValue(_expectedTickerStreamModel), properties[i].GetValue(actualList[j]));
                        }
                    }

                    await webSocket.DisconnectAsync(CancellationToken.None);
                },
                CancellationToken.None);

            // Act
            await webSocket.ConnectAsync(CancellationToken.None);
        }

        #endregion

        /// <summary>
        ///     Проверка верной конвертации значений периодов свечей
        /// </summary>
        [Fact(DisplayName = "Candlesticks Interval Convertation Test")]
        public void CandlestickIntervalConversionTest()
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

        #region Private methods

        /// <summary>
        ///     Возвращает содержимое файла в массиве байтов
        /// </summary>
        /// <param name="filePath">  путь к файлу с ответом со стрима </param>
        private byte[] GetBytes(string filePath)
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
            binanceWebSocketHumble.ConnectAsync(new Uri(url), CancellationToken.None).Returns(Task.CompletedTask);
            binanceWebSocketHumble.ReceiveAsync(Arg.Any<ArraySegment<byte>>(), Arg.Any<CancellationToken>())
                .Returns(_ =>
                {
                    for (var i = 0; i < bytes.Length; i++)
                    {
                        ((ArraySegment<byte>)_[0])[i] = bytes[i];
                    }

                    return new WebSocketReceiveResult(bytes.Length, WebSocketMessageType.Text, true);
                });

            return binanceWebSocketHumble;
        }

        #endregion
    }
}
