using Common.Models;
using Common.WebSocket;
using DataServiceLibrary;
using ExchangeLibrary;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDataService.DataHandlers
{
    /// <summary>
    ///     Обработчик данных со стримов маркетдаты для тикеров
    /// </summary>
    internal class MarketdataStreamHandler : IDataHandler
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IExchange _exchange;
        private readonly List<MiniTickerStreamModel> _receivedModelsStorage = new();
        private readonly List<MiniTickerStreamModel> _assistantModelsStorage = new();
        private bool _isSavingDataNow;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed;
        private IWebSocket _webSocket;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataStreamHandler"/>
        public MarketdataStreamHandler(IExchange exchange)
        {
            _exchange = exchange;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _webSocket = _exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(HandleAsync, cancellationToken, OnClosedHandler);

            Task.Run(async () => await StartWebSocket(_cancellationTokenSource.Token), cancellationToken);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            return Task.CompletedTask;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Запускает стрим
        /// </summary>
        private async Task StartWebSocket(CancellationToken cancellationToken)
        {
            try
            {
                await _webSocket.ConnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to start web socket stream");
            }
        }

        /// <summary>
        ///     Обрабатывает полученные данные
        /// </summary>
        private async Task HandleAsync(IEnumerable<MiniTickerStreamModel> models, CancellationToken cancellationToken)
        {
            try
            {
                if (_isSavingDataNow)
                {
                    _assistantModelsStorage.AddRange(models);
                    return;
                }

                _receivedModelsStorage.AddRange(models);

                if (_receivedModelsStorage.Count > 10_000)
                {
                    _isSavingDataNow = true;

                    _isSavingDataNow = false;
                }

                _receivedModelsStorage.AddRange(_assistantModelsStorage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to handle received models");
            }
        }

        /// <summary>
        ///     Обработчик закрытия стрима
        /// </summary>
        private void OnClosedHandler()
        {
            Task.Run(async () => await _webSocket.ConnectAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
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

            StopAsync().GetAwaiter().GetResult();
            _isDisposed = true;
        }

        #endregion
    }
}
