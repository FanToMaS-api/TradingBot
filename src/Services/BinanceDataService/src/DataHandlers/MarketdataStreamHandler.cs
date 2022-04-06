using AutoMapper;
using BinanceDatabase.Entities;
using BinanceDatabase.Repositories;
using Common.Models;
using Common.WebSocket;
using DataServiceLibrary;
using ExchangeLibrary;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Quartz;
using Scheduler;
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
        private readonly IRecurringJobScheduler _scheduler;
        private readonly IMapper _mapper;
        private TriggerKey _triggerKey;
        private readonly List<MiniTradeObjectStreamModel> _mainModelsStorage = new();
        private readonly List<MiniTradeObjectStreamModel> _assistantModelsStorage = new();
        private bool _isAssistantStorageSaving;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed;
        private IWebSocket _webSocket;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataStreamHandler"/>
        public MarketdataStreamHandler(IExchange exchange, IRecurringJobScheduler scheduler, IMapper mapper)
        {
            _exchange = exchange;
            _scheduler = scheduler;
            _mapper = mapper;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _webSocket = _exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(HandleAsync, cancellationToken, OnClosedHandler);

            Task.Run(async () => await StartWebSocket(_cancellationTokenSource.Token), cancellationToken);

            _triggerKey = await _scheduler.ScheduleAsync(Cron.EveryNthMinute(2), SaveDataAsync);
        }

        /// <inheritdoc />
        public Task StopAsync()
        {
            _scheduler?.UnscheduleAsync(_triggerKey);
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
        private Task HandleAsync(IEnumerable<MiniTradeObjectStreamModel> models, CancellationToken cancellationToken)
        {
            try
            {
                if (_isAssistantStorageSaving)
                {
                    _mainModelsStorage.AddRange(models);
                    return Task.CompletedTask;
                }

                _assistantModelsStorage.AddRange(models);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to handle received models");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Сохраняет полученные данные
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private async Task SaveDataAsync(IServiceProvider serviceProvider)
        {
            _isAssistantStorageSaving = !_isAssistantStorageSaving;

            if (_isAssistantStorageSaving)
            {
                await SaveDataAsync(serviceProvider, _assistantModelsStorage);

                return;
            }

            await SaveDataAsync(serviceProvider, _mainModelsStorage);
        }

        /// <summary>
        ///     Маппит и сохраняет данные в бд
        /// </summary>
        private async Task SaveDataAsync(IServiceProvider serviceProvider, IEnumerable<MiniTradeObjectStreamModel> streamModels)
        {
            using var database = serviceProvider.GetRequiredService<IUnitOfWork>();
            var hotData = _mapper.Map<IEnumerable<HotMiniTickerEntity>>(streamModels);

            await database.HotUnitOfWork.HotMiniTickers.AddRangeAsync(hotData, _cancellationTokenSource.Token);

            // TODO: группировка обычных данных MiniTickerEntity по интервалам

            await database.SaveChangesAsync(_cancellationTokenSource.Token);
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
