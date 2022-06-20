using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDataService.Configuration;
using BinanceDataService.DataAggregators;
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
        private TriggerKey _dataDelitionTriggerKey;
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
            _webSocket = _exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(
                OnDataReceived,
                cancellationToken);

            _webSocket.StreamClosed += OnWebSocketClosed;
            _webSocket.StreamStarted += OnStreamStarted;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Factory.StartNew(
                async (_) => await StartWebSocket(_cancellationTokenSource.Token),
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            _triggerKey = await _scheduler.ScheduleAsync(_configuration.SaveDataCron, SaveDataAsync);
            _dataDelitionTriggerKey = await _scheduler.ScheduleAsync(_configuration.DeleteDataCron, DeleteDataAsync);
            _dataAggregators.ForEach(async _ => await _.StartAsync());

        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            _scheduler?.UnscheduleAsync(_triggerKey);
            _scheduler?.UnscheduleAsync(_dataDelitionTriggerKey);
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
            lock (_lockObject)
            {
                IsAssistantStorageSaving = !IsAssistantStorageSaving;
            }

            try
            {
                if (IsAssistantStorageSaving)
                {
                    await SaveDataAsync(serviceProvider, _assistantModelsStorage);
                    _assistantModelsStorage.Clear();
                    return;
                }

                await SaveDataAsync(serviceProvider, _mainModelsStorage);
                _mainModelsStorage.Clear();
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(
                    ex,
                    $"Failed to save data _isAssistantStorageSaving='{IsAssistantStorageSaving}'",
                    cancellationToken: _cancellationTokenSource.Token);
            }
        }

        /// <summary>
        ///     Маппит и сохраняет данные в бд
        /// </summary>
        private async Task SaveDataAsync(IServiceProvider serviceProvider, IEnumerable<MiniTradeObjectStreamModel> streamModels)
        {
            var databaseFactory = serviceProvider.GetService<IBinanceDbContextFactory>()
                ?? throw new InvalidOperationException($"{nameof(IBinanceDbContextFactory)} not registered!");

            using var database = databaseFactory.CreateScopeDatabase();
            var hotData = _mapper.Map<IEnumerable<HotMiniTickerEntity>>(streamModels);
            await database.HotUnitOfWork.HotMiniTickers.AddRangeAsync(hotData, _cancellationTokenSource.Token);

            if (_dataAggregators.Any(_ => _.IsActive))
            {
                var coldData = _mapper.Map<IEnumerable<MiniTickerEntity>>(streamModels);
                await database.ColdUnitOfWork.MiniTickers.AddRangeAsync(coldData, _cancellationTokenSource.Token);
            }

            await database.SaveChangesAsync(_cancellationTokenSource.Token);

            await _logger.TraceAsync(
                $"{streamModels.Count()} entities successfully saved",
                cancellationToken: _cancellationTokenSource.Token);
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
            _logger.InfoAsync("The websocket has been closed.").Wait(5 * 1000);

            // QUESTION: интересное решение, надо проверить НО скорее всего неверное!
            _webSocket.Dispose();
            _webSocket?.DisconnectAsync(_cancellationTokenSource.Token);
            _webSocket = _exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(
                OnDataReceived,
                _cancellationTokenSource.Token);
            _webSocket.StreamClosed += OnWebSocketClosed;

            for (var i = 0; i < WebSocketReconnectionLimit; i++)

            {
                _logger.InfoAsync($"Restarting stream, attempt number = {i + 1}").Wait(5 * 1000);

                Task.Factory.StartNew(
                    async (_) => await StartWebSocket(_cancellationTokenSource.Token),
                    TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                    _cancellationTokenSource.Token);

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
                var deletedEntitiesCount = await database.HotUnitOfWork.HotMiniTickers
                    .RemoveUntilAsync(now.AddDays(-1), _cancellationTokenSource.Token);

                await _logger.InfoAsync(
                    $"Old data deleted successfully! Entities removed: {deletedEntitiesCount}",
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
