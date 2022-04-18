using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using Common.Models;
using Common.WebSocket;
using DataServiceLibrary;
using ExchangeLibrary;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Scheduler;
using System;
using System.Collections.Generic;
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

        private readonly ILoggerDecorator _logger;
        private readonly IExchange _exchange;
        private readonly IRecurringJobScheduler _scheduler;
        private readonly IMapper _mapper;
        private TriggerKey _triggerKey;
        private TriggerKey _dataDelitionTriggerKey;
        private readonly List<MiniTradeObjectStreamModel> _mainModelsStorage = new();
        private readonly List<MiniTradeObjectStreamModel> _assistantModelsStorage = new();
        private bool _isAssistantStorageSaving;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed;
        private IWebSocket _webSocket;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MarketdataStreamHandler"/>
        public MarketdataStreamHandler(
            IExchange exchange,
            IRecurringJobScheduler scheduler,
            IMapper mapper,
            ILoggerDecorator logger)
        {
            _exchange = exchange;
            _scheduler = scheduler;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _webSocket = _exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(HandleAsync, cancellationToken, WebSocketCloseHandler);

            Task.Run(async () => await StartWebSocket(_cancellationTokenSource.Token), cancellationToken);

            _triggerKey = await _scheduler.ScheduleAsync(Cron.Minutely(), SaveDataAsync);
            _dataDelitionTriggerKey = await _scheduler.ScheduleAsync(Cron.Daily(), DeleteDataAsync);

            await _logger.InfoAsync("Marketdata handler launched successfully!", cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            _scheduler?.UnscheduleAsync(_triggerKey);
            _scheduler?.UnscheduleAsync(_dataDelitionTriggerKey);
            _webSocket?.Dispose();

            await _logger.InfoAsync("Marketdata handler sropped", cancellationToken: _cancellationTokenSource.Token);
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
                await _logger.ErrorAsync(ex, "Failed to start web socket stream", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        ///     Обрабатывает полученные данные
        /// </summary>
        internal async Task HandleAsync(IEnumerable<MiniTradeObjectStreamModel> models, CancellationToken cancellationToken)
        {
            try
            {
                if (_isAssistantStorageSaving)
                {
                    _mainModelsStorage.AddRange(models);
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
            _isAssistantStorageSaving = !_isAssistantStorageSaving;

            try
            {
                if (_isAssistantStorageSaving)
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
                    $"Failed to save data _isAssistantStorageSaving='{_isAssistantStorageSaving}'",
                    cancellationToken: _cancellationTokenSource.Token);
            }
        }

        /// <summary>
        ///     Маппит и сохраняет данные в бд
        /// </summary>
        internal async Task SaveDataAsync(IServiceProvider serviceProvider, IEnumerable<MiniTradeObjectStreamModel> streamModels)
        {
            var databaseFactory = serviceProvider.GetRequiredService<IBinanceDbContextFactory>();
            using var database = databaseFactory.CreateScopeDatabase();
            var hotData = _mapper.Map<IEnumerable<HotMiniTickerEntity>>(streamModels);

            await database.HotUnitOfWork.HotMiniTickers.AddRangeAsync(hotData, _cancellationTokenSource.Token);

            var groupedData = GetGroupedMiniTickers(AggregateDataIntervalType.OneMinute, streamModels);
            await database.ColdUnitOfWork.MiniTickers.AddRangeAsync(groupedData, _cancellationTokenSource.Token);

            await database.SaveChangesAsync(_cancellationTokenSource.Token);
            await _logger.TraceAsync(
                $"{streamModels.Count()} entities successfully saved",
                cancellationToken: _cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Возвращает аггрегированные (через усреднение) данные о мини-тикерах
        /// </summary>
        internal MiniTickerEntity[] GetGroupedMiniTickers(AggregateDataIntervalType intervalType, IEnumerable<MiniTradeObjectStreamModel> streamModels)
        {
            var groupsByName = _mapper.Map<IEnumerable<MiniTickerEntity>>(streamModels).GroupBy(_ => _.ShortName);
            var interval = intervalType.ConvertToTimeSpan();
            var aggregatedMiniTickers = new List<MiniTickerEntity>();
            foreach (var group in groupsByName)
            {
                var first = group.FirstOrDefault();
                var start = first.EventTime;
                var counter = 0;
                var isNotDistributedItemsLeft = false;
                var aggregateObject = new MiniTickerEntity()
                {
                    ShortName = group.Key,
                    AggregateDataInterval = intervalType,
                    EventTime = start,
                };

                foreach (var item in group)
                {
                    isNotDistributedItemsLeft = true;
                    if (item.EventTime - start > interval)
                    {
                        AveragingFields(aggregateObject, counter);
                        AddToResult(aggregatedMiniTickers, aggregateObject, ref counter, ref isNotDistributedItemsLeft);
                        aggregateObject = item;
                        aggregateObject.AggregateDataInterval = intervalType;
                        start = item.EventTime;
                        isNotDistributedItemsLeft = true;
                        continue;
                    }

                    AggregateFields(item, aggregateObject);
                    counter++;
                }

                if (isNotDistributedItemsLeft)
                {
                    AveragingFields(aggregateObject, counter);
                    AddToResult(aggregatedMiniTickers, aggregateObject, ref counter, ref isNotDistributedItemsLeft);
                }
            }

            return aggregatedMiniTickers.ToArray();
        }

        /// <summary>
        ///     Функция добавления усредненной модели в результирующий список и сброса параметров
        /// </summary>
        /// <param name="counter"> Счетчик усредненных моделей </param>
        /// <param name="isDataLeft"> Флаг, что усредненных данных больше нет </param>
        internal static void AddToResult(
            List<MiniTickerEntity> aggregatedMiniTickers,
            MiniTickerEntity aggregateObject,
            ref int counter,
            ref bool isDataLeft)
        {
            aggregatedMiniTickers.Add(aggregateObject);
            counter = 1;
            isDataLeft = false;
        }

        /// <summary>
        ///     Аггрегирует поля объектов в один объект
        /// </summary>
        internal static void AggregateFields(MiniTickerEntity addedObject, MiniTickerEntity aggregateObject)
        {
            aggregateObject.OpenPrice += addedObject.OpenPrice;
            aggregateObject.MaxPrice += addedObject.MaxPrice;
            aggregateObject.ClosePrice += addedObject.ClosePrice;
            aggregateObject.MinPrice += addedObject.MinPrice;
            aggregateObject.QuotePurchaseVolume += addedObject.QuotePurchaseVolume;
            aggregateObject.BasePurchaseVolume += addedObject.BasePurchaseVolume;
        }

        /// <summary>
        ///     Усредняет значения полей
        /// </summary>
        internal static void AveragingFields(MiniTickerEntity aggregateObject, int conter)
        {
            aggregateObject.OpenPrice /= conter;
            aggregateObject.MaxPrice /= conter;
            aggregateObject.ClosePrice /= conter;
            aggregateObject.MinPrice /= conter;
            aggregateObject.QuotePurchaseVolume /= conter;
            aggregateObject.BasePurchaseVolume /= conter;
        }

        /// <summary>
        ///     Обработчик закрытия стрима
        /// </summary>
        private void WebSocketCloseHandler()
        {
            _logger.InfoAsync("The websocket has been closed. Restarting stream...").Wait(5 * 1000);

            Task.Run(async () => await _webSocket.ConnectAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
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
                var count = database.HotUnitOfWork.HotMiniTickers.RemoveUntil(now.AddDays(-1));

                await database.SaveChangesAsync(_cancellationTokenSource.Token);

                await _logger.TraceAsync(
                    $"Old data deleted successfully! Entities removed: {count}",
                    cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex, "Failed to remove hot data", cancellationToken: _cancellationTokenSource.Token);
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

            StopAsync().GetAwaiter().GetResult();
            _isDisposed = true;
        }

        #endregion
    }
}
