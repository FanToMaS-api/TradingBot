using DataServiceLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDataService
{
    /// <summary>
    ///     Сервис по получению и обработке данных с бинанса
    /// </summary>
    public class BinanceDataService : IDataService
    {
        #region Fields

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <summary>
        ///     Сервис по получению и обработке данных с бинанса
        /// </summary>
        /// <param name="dataHandlers"> Обработчики данных </param>
        public BinanceDataService(params IDataHandler[] dataHandlers)
        {
            DataHandlers = dataHandlers.ToImmutableArray();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public ImmutableArray<IDataHandler> DataHandlers { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task StartAsync()
        {
            _cancellationTokenSource = new();
            foreach (var handler in DataHandlers)
            {
                await handler.StartAsync(_cancellationTokenSource.Token);
            }
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            foreach (var handler in DataHandlers)
            {
                await handler.StopAsync();
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        #endregion

        #region Implementation iDisposable

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
