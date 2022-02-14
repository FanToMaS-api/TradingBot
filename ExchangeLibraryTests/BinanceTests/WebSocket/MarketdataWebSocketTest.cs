using ExchangeLibrary.Binance.DTOs.Marketdata;
using ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata.Impl;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
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

        private readonly BookTickerStreamDto _expectedBookTickerStreamDto = new()
        {
            OrderBookUpdatedId = 400900217,
            Symbol = "BNBUSDT",
            BestBidPrice = 25.35190000,
            BestBidQuantity = 31.21000000,
            BestAskPrice = 25.36520000,
            BestAskQuantity = 40.66000000,
        };

        private readonly TickerStreamDto _expectedTickerStreamDto = new()
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

        private readonly MiniTickerStreamDto _expectedMiniTickerStreamDto = new()
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
            var expected = new AggregateSymbolTradeStreamDto
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
            using var webSocket = new MarketdataWebSocket<AggregateSymbolTradeStreamDto>(
                url,
                MarketdataStreamType.AggregateTradeStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    var properties = typeof(AggregateSymbolTradeStreamDto).GetProperties();
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
            using var webSocket = new MarketdataWebSocket<BookTickerStreamDto>(
                url,
                MarketdataStreamType.IndividualSymbolBookTickerStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    var properties = typeof(BookTickerStreamDto).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(_expectedBookTickerStreamDto), properties[i].GetValue(actual));
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
            using var webSocket = MarketdataWebSocket<IEnumerable<BookTickerStreamDto>>.CreateAllBookTickersStream(webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    var properties = typeof(BookTickerStreamDto).GetProperties();
                    var actualList = actual.ToList();
                    for (var j = 0; j < actualList.Count; j++)
                    {
                        for (var i = 0; i < properties.Length; i++)
                        {
                            Assert.Equal(properties[i].GetValue(_expectedBookTickerStreamDto), properties[i].GetValue(actualList[j]));
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
            var expected = new CandlestickStreamDto
            {
                EventTimeUnix = 123456789,
                Symbol = "BNBBTC",
                Kline = new KlineModelDto
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
            using var webSocket = MarketdataWebSocket<CandlestickStreamDto>.CreateCandlestickStream(
                url,
                CandleStickIntervalType.OneMinute,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    Assert.Equal(expected.EventTimeUnix, actual.EventTimeUnix);
                    Assert.Equal(expected.Symbol, actual.Symbol);

                    var properties = typeof(KlineModelDto).GetProperties();
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
            using var webSocket = new MarketdataWebSocket<MiniTickerStreamDto>(
                url,
                MarketdataStreamType.IndividualSymbolMiniTickerStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    var properties = typeof(MiniTickerStreamDto).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(_expectedMiniTickerStreamDto), properties[i].GetValue(actual));
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
            using var webSocket = MarketdataWebSocket<IEnumerable<MiniTickerStreamDto>>.CreateAllMarketMiniTickersStream(webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    var properties = typeof(MiniTickerStreamDto).GetProperties();
                    var actualList = actual.ToList();
                    for(var j = 0; j < actualList.Count; j++)
                    {
                        for (var i = 0; i < properties.Length; i++)
                        {
                            Assert.Equal(properties[i].GetValue(_expectedMiniTickerStreamDto), properties[i].GetValue(actualList[j]));
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
            var expected = new OrderBookDto
            {
                LastUpdateId = 160,
                Bids = new List<PriceQtyPair>
                {
                    new PriceQtyPair
                    {
                        Price = 0.0024,
                        Qty = 10
                    },
                    new PriceQtyPair
                    {
                        Price = 0.0025,
                        Qty = 11
                    },
                },
                Asks = new List<PriceQtyPair>
                {
                    new PriceQtyPair
                    {
                        Price = 0.0026,
                        Qty = 100
                    },
                    new PriceQtyPair
                    {
                        Price = 0.0027,
                        Qty = 111
                    },
                }
            };
            var url = "wss://stream.binance.com:9443";
            var bytes = GetBytes("../../../BinanceTests/Jsons/WebSocket/PartialBookDepthStream.json");
            var webSocketHumbleMock = GetMockingBinanceWebHumble(url, bytes);
            using var webSocket = MarketdataWebSocket<OrderBookDto>.CreatePartialBookDepthStream(
                url,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    Assert.Equal(expected.Bids.Count, actual.Bids.Count);
                    Assert.Equal(expected.Asks.Count, actual.Asks.Count);
                    Assert.Equal(expected.LastUpdateId, actual.LastUpdateId);
                    Assert.Equal(expected.Bids[0].Price, actual.Bids[0].Price);
                    Assert.Equal(expected.Bids[0].Qty, actual.Bids[0].Qty);
                    Assert.Equal(expected.Bids[1].Price, actual.Bids[1].Price);
                    Assert.Equal(expected.Bids[1].Qty, actual.Bids[1].Qty);
                    Assert.Equal(expected.Asks[0].Price, actual.Asks[0].Price);
                    Assert.Equal(expected.Asks[0].Qty, actual.Asks[0].Qty);
                    Assert.Equal(expected.Asks[1].Price, actual.Asks[1].Price);
                    Assert.Equal(expected.Asks[1].Qty, actual.Asks[1].Qty);

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
            var expected = new SymbolTradeStreamDto
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
            using var webSocket = new MarketdataWebSocket<SymbolTradeStreamDto>(
                url,
                MarketdataStreamType.TradeStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    var properties = typeof(SymbolTradeStreamDto).GetProperties();
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
            using var webSocket = new MarketdataWebSocket<TickerStreamDto>(
                url,
                MarketdataStreamType.IndividualSymbolTickerStream,
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    var properties = typeof(TickerStreamDto).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        Assert.Equal(properties[i].GetValue(_expectedTickerStreamDto), properties[i].GetValue(actual));
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
            using var webSocket = MarketdataWebSocket<IEnumerable<TickerStreamDto>>.CreateAllTickersStream(
                webSocketHumbleMock);

            webSocket.AddOnMessageReceivedFunc(
                async (actual) =>
                {
                    var actualList = actual.ToList();
                    var properties = typeof(TickerStreamDto).GetProperties();
                    for (var j = 0; j < actualList.Count; j++)
                    {
                        for (var i = 0; i < properties.Length; i++)
                        {
                            Assert.Equal(properties[i].GetValue(_expectedTickerStreamDto), properties[i].GetValue(actualList[j]));
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
            var intervals = new List<CandleStickIntervalType>
            {

                CandleStickIntervalType.OneMinute,
                CandleStickIntervalType.ThreeMinutes,
                CandleStickIntervalType.FiveMinutes,
                CandleStickIntervalType.FifteenMinutes,
                CandleStickIntervalType.ThirtyMinutes,
                CandleStickIntervalType.OneHour,
                CandleStickIntervalType.TwoHour,
                CandleStickIntervalType.FourHours,
                CandleStickIntervalType.SixHours,
                CandleStickIntervalType.EightHours,
                CandleStickIntervalType.TwelveHours,
                CandleStickIntervalType.OneDay,
                CandleStickIntervalType.ThreeDays,
                CandleStickIntervalType.OneWeek,
                CandleStickIntervalType.OneMonth
            };

            for (var i = 0; i < intervalstringViews.Count; i++)
            {
                Assert.Equal(intervalstringViews[i], intervals[i].GetInterval());
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
        public IBinanceWebSocketHumble GetMockingBinanceWebHumble(string url, byte[] bytes)
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
