using ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata.Impl;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.WebSocket;
using ExchangeLibrary.Binance.WebSocket.Marketdata;
using NSubstitute;
using System;
using System.IO;
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
        #region Public methods

        /// <summary>
        ///     Тест подписки на <see cref="MarketdataStreamType.AggregateTradeStream"/>
        /// </summary>
        [Fact(DisplayName = "Aggregate Trade Stream subscription Test")]
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
            using var webSocket = new MarketdataWebSocket<AggregateSymbolTradeStreamDto>(url, MarketdataStreamType.AggregateTradeStream, webSocketHumbleMock, bytes.Length);

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

                    return new WebSocketReceiveResult(1, WebSocketMessageType.Text, true);
                });

            return binanceWebSocketHumble;
        }

        #endregion
    }
}
