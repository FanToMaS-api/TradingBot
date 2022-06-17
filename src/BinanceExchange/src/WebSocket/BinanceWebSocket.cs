using Common.WebSocket;
using Logger;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.WebSocket
{
    /// <summary>
    ///     Оболочка над Binance websocket
    /// </summary>
    internal class BinanceWebSocket : IWebSocket
    {
        #region Fields

        private static readonly ILoggerDecorator Log = LoggerManager.CreateDefaultLogger();
        private readonly IBinanceWebSocketHumble _webSocketHumble;
        private readonly List<Func<string, Task>> _onMessageReceivedFunctions = new();
        private readonly List<CancellationTokenRegistration> _onMessageReceivedCancellationTokenRegistrations = new();
        private readonly Uri _uri;
        private readonly int _receiveBufferSize = 8 * 1024 * 512; // 4 Kb
        private CancellationTokenSource _loopCancellationTokenSource;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceWebSocket"/>>
        public BinanceWebSocket(IBinanceWebSocketHumble socketHumble, string url)
        {
            _webSocketHumble = socketHumble;
            _uri = new Uri(url);
        }

        #endregion

        #region Events

        /// <summary>
        ///     Событие, возникающее при закрытии веб-сокета
        /// </summary>
        internal Func<BinanceWebSocket, CancellationToken, Task> WebSocketClosed { get; set; }

        /// <inheritdoc />
        public Action StreamStarted { get; set; }
        
        /// <inheritdoc />
        public Action StreamClosed { get; set; }

        #endregion

        #region Properties

        /// <inheritdoc />
        public WebSocketState SocketState => _webSocketHumble.State;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            if (_webSocketHumble.State != WebSocketState.Open)
            {
                _loopCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                await _webSocketHumble.ConnectAsync(_uri, cancellationToken);
                var task = await Task.Factory.StartNew(
                    async () => await ReceiveLoopAsync(_receiveBufferSize, _loopCancellationTokenSource.Token),
                    _loopCancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);

                StreamStarted?.Invoke();
                task.Wait(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            if (_loopCancellationTokenSource != null)
            {
                _loopCancellationTokenSource.Cancel();
            }

            if (_webSocketHumble.State == WebSocketState.Open)
            {
                await _webSocketHumble.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                await _webSocketHumble.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
            }
        }

        /// <inheritdoc />
        public void AddOnMessageReceivedFunc(Func<string, Task> onMessageReceivedFunc, CancellationToken cancellationToken)
        {
            if (cancellationToken == CancellationToken.None)
            {
                throw new Exception("Unable to register event unsubscribe. Possible memory leak");
            }

            _onMessageReceivedFunctions.Add(onMessageReceivedFunc);

            var reg = cancellationToken.Register(() =>
                _onMessageReceivedFunctions.Remove(onMessageReceivedFunc));
            _onMessageReceivedCancellationTokenRegistrations.Add(reg);
        }

        /// <inheritdoc />
        public async Task SendAsync(string message, CancellationToken cancellationToken)
        {
            var byteArray = Encoding.ASCII.GetBytes(message);

            await _webSocketHumble.SendAsync(
                new ArraySegment<byte>(byteArray),
                WebSocketMessageType.Text,
                true,
                cancellationToken);
        }

        /// <inheritdoc />
        public override string ToString() => $"Stream on this url: '{_uri}'";

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            DisconnectAsync(CancellationToken.None).Wait();
            _webSocketHumble?.Dispose();
            _onMessageReceivedCancellationTokenRegistrations.ForEach(ct => ct.Dispose());
            _loopCancellationTokenSource?.Dispose();
            _onMessageReceivedFunctions?.Clear();

            _isDisposed = true;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Цикл получения данных со стрима сервера
        /// </summary>
        private async Task ReceiveLoopAsync(int _receiveBufferSize, CancellationToken cancellationToken)
        {
            WebSocketReceiveResult receiveResult = null;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var buffer = new ArraySegment<byte>(new byte[_receiveBufferSize]);
                    try
                    {
                        receiveResult = await _webSocketHumble.ReceiveAsync(buffer, cancellationToken);

                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                        {
                            await Log.ErrorAsync("The web socket has been closed", cancellationToken: cancellationToken);
                            StreamClosed?.Invoke();
                            await WebSocketClosed?.Invoke(this, cancellationToken);

                            break;
                        }

                        var content = Encoding.UTF8.GetString(buffer.Slice(0, receiveResult.Count).ToArray());
                        _onMessageReceivedFunctions.ForEach(async func =>
                        {
                            await func(content);
                        });
                    }
                    catch (Exception ex)
                    {
                        await Log.ErrorAsync(ex, "Failed to get data from stream", cancellationToken: cancellationToken);
                        throw;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                await Log.ErrorAsync(ex, $"The recieve loop was cancelled.", cancellationToken: cancellationToken);
            }
            finally
            {
                await DisconnectAsync(CancellationToken.None);
            }
        }

        #endregion
    }
}
