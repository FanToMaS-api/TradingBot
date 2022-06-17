using DataServiceLibrary;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDataService
{
    /// <summary>
    ///     Сервис по получению и обработке данных с бинанса
    /// </summary>
    internal class BinanceDataService : IDataService
    {
        #region Fields

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <summary>
        ///     Сервис по получению и обработке данных с бинанса
        /// </summary>
        /// <param name="dataHandler"> Обработчик данных </param>
        public BinanceDataService(IDataHandler dataHandler)
        {
            DataHandler = dataHandler;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IDataHandler DataHandler { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task StartAsync()
        {
            _cancellationTokenSource = new();
            await DataHandler.StartAsync(_cancellationTokenSource.Token);
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            await DataHandler.StopAsync();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            StopAsync().Wait();
            _isDisposed = true;
        }

        #endregion
    }
}
