using Common.WebSocket;
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

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IBinanceWebSocketHumble _webSocketHumble;
        private readonly List<Func<string, Task>> _onMessageReceivedFunctions = new();
        private readonly List<CancellationTokenRegistration> _onMessageReceivedCancellationTokenRegistrations = new();
        private CancellationTokenSource _loopCancellationTokenSource;
        private readonly Uri _uri;
        private readonly int _receiveBufferSize = 8 * 1024 * 512; // 4 Kb
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

        #region Properties

        /// <summary>
        ///     Событие, возникающее при закрытии веб-сокета
        /// </summary>
        internal Func<BinanceWebSocket, CancellationToken, Task> OnClosed { get; set; }

        /// <inheritdoc />
        public Action OnStreamClosed { get; set; }

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
                    async () => await ReceiveLoopAsync(_loopCancellationTokenSource.Token, _receiveBufferSize),
                    _loopCancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);

                task.Wait();
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
            _onMessageReceivedFunctions.Add(onMessageReceivedFunc);

            if (cancellationToken != CancellationToken.None)
            {
                var reg = cancellationToken.Register(() =>
                    _onMessageReceivedFunctions.Remove(onMessageReceivedFunc));

                _onMessageReceivedCancellationTokenRegistrations.Add(reg);
            }
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

        #region Private methods

        /// <summary>
        ///     Цикл получения данных со стрима сервера
        /// </summary>
        private async Task ReceiveLoopAsync(CancellationToken cancellationToken, int _receiveBufferSize)
        {
            WebSocketReceiveResult receiveResult = null;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var buffer = new ArraySegment<byte>(new byte[_receiveBufferSize]);
                    try
                    {
                        receiveResult = await _webSocketHumble.ReceiveAsync(buffer, cancellationToken);

                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                        {
                            Log.Error($"The web socket has been closed");
                            OnStreamClosed?.Invoke();
                            await OnClosed?.Invoke(this, cancellationToken);

                            break;
                        }

                        var content = Encoding.UTF8.GetString(buffer.Slice(0, receiveResult.Count).ToArray());
                        _onMessageReceivedFunctions.ForEach(func =>
                        {
                            var task = func(content);
                            CheckTaskException(task);
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to get data from stream");
                        throw;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Log.Error(ex, $"The recieve loop was cancelled.");
                await DisconnectAsync(CancellationToken.None);
            }
        }

        /// <summary>
        ///     Проверяет на успешное завершение таски, бросает исключения если таска провалилась
        /// </summary>
        /// <param name="task"></param>
        private void CheckTaskException(Task task)
        {
            if (task.Status == TaskStatus.Faulted)
            {
                foreach (var e in task.Exception.InnerExceptions)
                {
                    throw e;
                }
            }
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

            DisconnectAsync(CancellationToken.None).Wait();
            _webSocketHumble.Dispose();
            _onMessageReceivedCancellationTokenRegistrations.ForEach(ct => ct.Dispose());
            _loopCancellationTokenSource.Dispose();

            _isDisposed = true;
        }

        #endregion
    }
}
