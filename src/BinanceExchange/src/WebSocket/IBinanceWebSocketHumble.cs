using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.WebSocket
{
    /// <summary>
    ///     Оболочка над веб-сокетами
    /// </summary>
    internal interface IBinanceWebSocketHumble : IDisposable
    {
        /// <inheritdoc cref="WebSocket.State"/>
        WebSocketState State { get; }

        /// <summary>
        ///     Подключение к веб-сокету сервера
        /// </summary>
        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

        /// <inheritdoc cref="WebSocket.CloseOutputAsync"/>
        Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);

        /// <inheritdoc cref="WebSocket.CloseAsync"/>
        Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);

        /// <inheritdoc cref="WebSocket.ReceiveAsync"/>
        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);

        /// <inheritdoc cref="WebSocket.SendAsync"/>
        Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);
    }
}
