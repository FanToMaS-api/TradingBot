using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDataService.Configuration;
using BinanceDataService.DataAggregators;
using Common.Helpers;
using Common.Models;
using Common.WebSocket;
using DataServiceLibrary;
using ExchangeLibrary;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private const int WebSocketReconnectionLimit = 3;
        private readonly ILoggerDecorator _logger;
        private readonly IExchange _exchange;
        private readonly IRecurringJobScheduler _scheduler;
        private readonly IMapper _mapper;
        private readonly MarketdataStreamHandlerConfig _configuration;
        private readonly List<MiniTradeObjectStreamModel> _mainModelsStorage = new();
        private readonly List<MiniTradeObjectStreamModel> _assistantModelsStorage = new();
        private readonly List<DataAggregator> _dataAggregators = new();
        private readonly object _lockObject = new();
        private TriggerKey _triggerKey;
        private TriggerKey _dataDeletionTriggerKey;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed;
        private IWebSocket _webSocket;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataStreamHandler"/>
        public MarketdataStreamHandler(
            MarketdataStreamHandlerConfig config,
            IExchange exchange,
            IRecurringJobScheduler scheduler,
            IMapper mapper,
            ILoggerDecorator logger)
        {
            _configuration = config;
            _exchange = exchange;
            _scheduler = scheduler;
            _mapper = mapper;
            _logger = logger;

            _dataAggregators.Add(new DataAggregator(_logger, _scheduler, config.OneMinuteAggregator));
            _dataAggregators.Add(new DataAggregator(_logger, _scheduler, config.FiveMinutesAggregator));
            _dataAggregators.Add(new DataAggregator(_logger, _scheduler, config.FifteenMinutesAggregator));
            _dataAggregators.Add(new DataAggregator(_logger, _scheduler, config.OneHourAggregator));
        }

        #endregion

        #region Implementation of IDataHandler

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            InitializeWebSocket(cancellationToken);
            StartWebSocketTask(cancellationToken);

            _triggerKey = await _scheduler.GeneralScheduleAsync(_configuration.SaveDataCron, SaveDataAsync);
            _dataDeletionTriggerKey = await _scheduler.GeneralScheduleAsync(_configuration.DeleteDataCron, DeleteDataAsync);

            StartDataAggregators();
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            _scheduler?.UnscheduleAsync(_triggerKey);
            _scheduler?.UnscheduleAsync(_dataDeletionTriggerKey);
            _dataAggregators.ForEach(async _ => await _.StopAsync());

            _webSocket?.DisconnectAsync(_cancellationTokenSource.Token);

            await _logger.InfoAsync("Marketdata handler stopped", cancellationToken: _cancellationTokenSource.Token);
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

            StopAsync().Wait();
            _webSocket?.Dispose();
            _isDisposed = true;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Флаг, определяющий текущее место сохранения данных
        /// </summary>
        internal bool IsAssistantStorageSaving { get; private set; }

        #endregion

        #region Private methods

        private void InitializeWebSocket(CancellationToken cancellationToken)
        {
            _webSocket = _exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(
                OnDataReceived,
                cancellationToken);

            _webSocket.StreamClosed += OnWebSocketClosed;
            _webSocket.StreamStarted += OnStreamStarted;
        }

        /// <summary>
        ///     Запускает задачу стрима с вебсокета
        /// </summary>
        private void StartWebSocketTask(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(
                async (_) => await StartWebSocket(_cancellationTokenSource.Token),
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                cancellationToken);
        }

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
                await _logger.ErrorAsync(ex, "Failed to start websocket stream", cancellationToken: cancellationToken);
            }
        }

        private void StartDataAggregators()
        {
            _dataAggregators.ForEach(async _ => await _.StartAsync());
        }

        /// <summary>
        ///     Обрабатывает полученные данные
        /// </summary>
        internal async Task OnDataReceived(IEnumerable<MiniTradeObjectStreamModel> models, CancellationToken cancellationToken)
        {
            try
            {
                if (IsAssistantStorageSaving)
                {
                    _mainModelsStorage.AddRange(models);
                    return;
                }

                _assistantModelsStorage.AddRange(models);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex, "Failed to handle received models", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        ///     Сохраняет полученные данные
        /// </summary>
        internal async Task SaveDataAsync(IServiceProvider serviceProvider)
        {
            var isAssistantStorageSaving = false;
            lock (_lockObject)
            {
                isAssistantStorageSaving = !IsAssistantStorageSaving;
                IsAssistantStorageSaving = isAssistantStorageSaving;
            }

            try
            {
                var entitiesCount = 0;
                if (isAssistantStorageSaving)
                {
                    entitiesCount = _assistantModelsStorage.Count;
                    await SaveDataAsync(serviceProvider, _assistantModelsStorage);

                    ClearListWithAssertion(_assistantModelsStorage, entitiesCount);

                    return;
                }

                entitiesCount = _mainModelsStorage.Count;
                await SaveDataAsync(serviceProvider, _mainModelsStorage);

                ClearListWithAssertion(_mainModelsStorage, entitiesCount);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(
                    ex,
                    $"Failed to save data {nameof(IsAssistantStorageSaving)}='{IsAssistantStorageSaving}'",
                    cancellationToken: _cancellationTokenSource.Token);
            }
        }

        private void ClearListWithAssertion(List<MiniTradeObjectStreamModel> list, int entitiesCount)
        {
            Assert.True(list.Count == entitiesCount);

            list.Clear();

            Assert.True(list.Count == 0);
        }

        /// <summary>
        ///     Маппит и сохраняет данные в бд
        /// </summary>
        private async Task SaveDataAsync(IServiceProvider serviceProvider, IEnumerable<MiniTradeObjectStreamModel> streamModels)
        {
            var databaseFactory = serviceProvider.GetService<IBinanceDbContextFactory>()
                ?? throw new InvalidOperationException($"{nameof(IBinanceDbContextFactory)} not registered!");

            using var database = databaseFactory.CreateScopeDatabase();
            await AddHotDataAsync(database, streamModels);
            await AddColdDataAsync(database, streamModels);

            await database.SaveChangesAsync(_cancellationTokenSource.Token);

            await _logger.TraceAsync(
                $"{streamModels.Count()} entities successfully saved",
                cancellationToken: _cancellationTokenSource.Token);
        }

        private async Task AddHotDataAsync(IUnitOfWork database, IEnumerable<MiniTradeObjectStreamModel> streamModels)
        {
            var hotData = _mapper.Map<IEnumerable<HotMiniTickerEntity>>(streamModels);
            await database.HotUnitOfWork.HotMiniTickers.AddRangeAsync(hotData, _cancellationTokenSource.Token);
        }

        private async Task AddColdDataAsync(IUnitOfWork database, IEnumerable<MiniTradeObjectStreamModel> streamModels)
        {
            if (_dataAggregators.Any(_ => _.IsActive))
            {
                var coldData = _mapper.Map<IEnumerable<MiniTickerEntity>>(streamModels);
                await database.ColdUnitOfWork.MiniTickers.AddRangeAsync(coldData, _cancellationTokenSource.Token);
            }
        }

        /// <summary>
        ///     Обработчик начала стрима
        /// </summary>
        public void OnStreamStarted()
        {
            _logger
                .InfoAsync("Marketdata handler launched successfully!", cancellationToken: _cancellationTokenSource.Token)
                .Wait(5 * 1000);
            _webSocket.StreamStarted -= OnStreamStarted;
        }

        /// <summary>
        ///     Обработчик закрытия стрима
        /// </summary>
        private void OnWebSocketClosed()
        {
            _webSocket.StreamClosed -= OnWebSocketClosed;
            _logger.InfoAsync("The websocket has been closed").Wait(5 * 1000);

            if (_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            // QUESTION: интересное решение, надо проверить НО скорее всего неверное!
            _webSocket?.DisconnectAsync(_cancellationTokenSource.Token);
            _webSocket.Dispose();
            _webSocket = _exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(
                OnDataReceived,
                _cancellationTokenSource.Token);
            _webSocket.StreamClosed += OnWebSocketClosed;

            for (var i = 0; i < WebSocketReconnectionLimit; i++)

            {
                _logger.InfoAsync($"Restarting stream, attempt number = {i + 1}").Wait(5 * 1000);
                StartWebSocketTask(_cancellationTokenSource.Token);

                if (_webSocket.SocketState is System.Net.WebSockets.WebSocketState.Open)
                {
                    _logger
                        .InfoAsync("Marketdata handler launched successfully!", cancellationToken: _cancellationTokenSource.Token)
                        .Wait(5 * 1000);
                    return;
                }

                Task.Delay(2 * 1000).Wait(); // Принудительная задержка переподключения
            }
        }

        /// <summary>
        ///     Удаляет накопившиеся данные
        /// </summary>
        internal async Task DeleteDataAsync(IServiceProvider serviceProvider)
        {
            var now = DateTime.Now;
            var databaseFactory = serviceProvider.GetRequiredService<IBinanceDbContextFactory>();
            using var database = databaseFactory.CreateScopeDatabase();
            try
            {
                var deletedHotEntitiesCount = await database.HotUnitOfWork.HotMiniTickers
                    .RemoveUntilAsync(now.AddDays(-1), _cancellationTokenSource.Token);

                var deletedColdEntitiesCount = await database.ColdUnitOfWork.MiniTickers
                    .RemoveUntilAsync(now.AddDays(-2), _cancellationTokenSource.Token);

                await _logger.InfoAsync(
                    $"Old data deleted successfully! HOT entities removed: {deletedHotEntitiesCount}. " +
                    $"COLD entities removed: {deletedColdEntitiesCount}",
                    cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(
                    ex,
                    "Failed to remove hot data",
                    cancellationToken: _cancellationTokenSource.Token);
            }
        }

        #endregion
    }
}
