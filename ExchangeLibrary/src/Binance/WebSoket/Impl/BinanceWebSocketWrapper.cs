using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.WebSoket.Impl
{
    /// <summary>
    ///     Оболочка над Binance websocket
    /// </summary>
    internal class BinanceWebSocketWrapper
    {
        #region Fields

        private readonly IBinanceWebSocketHumble _webSocketHumble;
        private List<Func<string, Task>> _onMessageReceivedFunctions = new();
        private List<CancellationTokenRegistration> _onMessageReceivedCancellationTokenRegistrations = new();
        private CancellationTokenSource _loopCancellationTokenSource;
        private Uri _url;
        private int _receiveBufferSize;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceWebSocketWrapper"/>>
        public BinanceWebSocketWrapper(IBinanceWebSocketHumble socketHumble, string url, int receiveBufferSize = 8192)
        {
            _webSocketHumble = socketHumble;
            _url = new Uri(url);
            _receiveBufferSize = receiveBufferSize;
        }

        #endregion

        #region Public methods



        #endregion
    }
}
