using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.WebSocket.Impl
{
    /// <summary>
    ///     Скромный объект для Binance над <see cref="ClientWebSocket" />
    /// </summary>
    internal class BinanceWebSocketHumble : IBinanceWebSocketHumble
    {
        #region Fields

        private readonly ClientWebSocket _webSocket;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceWebSocketHumble"/>
        public BinanceWebSocketHumble(ClientWebSocket clientWebSocket) => _webSocket = clientWebSocket;

        #endregion

        #region Properties

        /// <inheritdoc />
        public WebSocketState State => _webSocket.State;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken) =>
            await _webSocket.ConnectAsync(uri, cancellationToken);

        /// <inheritdoc />
        public async Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken) =>
            await _webSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);

        /// <inheritdoc />
        public async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken) =>
            await _webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);

        /// <inheritdoc />
        public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken) =>
            await _webSocket.ReceiveAsync(buffer, cancellationToken);

        /// <inheritdoc />
        public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) =>
            await _webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose() => _webSocket.Dispose();

        #endregion
    }
}
