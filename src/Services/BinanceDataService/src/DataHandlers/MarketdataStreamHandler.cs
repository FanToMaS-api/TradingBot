using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
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

        private readonly ILoggerDecorator _logger;
        private readonly IExchange _exchange;
        private readonly IRecurringJobScheduler _scheduler;
        private readonly IMapper _mapper;
        private TriggerKey _triggerKey;
        private TriggerKey _dataDelitionTriggerKey;
        private TriggerKey _dataAggregationTriggerKey;
        private readonly List<MiniTradeObjectStreamModel> _mainModelsStorage = new();
        private readonly List<MiniTradeObjectStreamModel> _assistantModelsStorage = new();
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

        #region Properties

        /// <summary>
        ///     Флаг определящий текущее место сохранения данных
        /// </summary>
        internal bool IsAssistantStorageSaving { get; private set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _webSocket = _exchange.MarketdataStreams.SubscribeAllMarketMiniTickersStream(HandleDataAsync, cancellationToken, WebSocketCloseHandler);

            Task.Run(async () => await StartWebSocket(_cancellationTokenSource.Token), cancellationToken);

            _triggerKey = await _scheduler.ScheduleAsync(Cron.Minutely(), SaveDataAsync);
            _dataDelitionTriggerKey = await _scheduler.ScheduleAsync(Cron.DailyOnHour(23), DeleteDataAsync);
            _dataAggregationTriggerKey = await _scheduler.ScheduleAsync(Cron.DailyOnHour(21), AggregateAndSaveDataAsync);

            await _logger.InfoAsync("Marketdata handler launched successfully!", cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            _scheduler?.UnscheduleAsync(_triggerKey);
            _scheduler?.UnscheduleAsync(_dataDelitionTriggerKey);
            _scheduler?.UnscheduleAsync(_dataAggregationTriggerKey);
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
        internal async Task HandleDataAsync(IEnumerable<MiniTradeObjectStreamModel> models, CancellationToken cancellationToken)
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
            IsAssistantStorageSaving = !IsAssistantStorageSaving;
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
            var databaseFactory = serviceProvider.GetService<IBinanceDbContextFactory>();
            using var database = databaseFactory.CreateScopeDatabase();
            var hotData = _mapper.Map<IEnumerable<HotMiniTickerEntity>>(streamModels);
            var coldData = _mapper.Map<IEnumerable<MiniTickerEntity>>(streamModels);

            await database.HotUnitOfWork.HotMiniTickers.AddRangeAsync(hotData, _cancellationTokenSource.Token);
            await database.ColdUnitOfWork.MiniTickers.AddRangeAsync(coldData, _cancellationTokenSource.Token);
            await database.SaveChangesAsync(_cancellationTokenSource.Token);

            await _logger.TraceAsync(
                $"{streamModels.Count()} entities successfully saved",
                cancellationToken: _cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Агрегирует полученные за день данные
        /// </summary>
        private async Task AggregateAndSaveDataAsync(IServiceProvider serviceProvider)
        {
            await _logger.InfoAsync(
               "Data aggregating started!",
               cancellationToken: _cancellationTokenSource.Token);

            try
            {
                var watch = new Stopwatch();
                watch.Start();
                var databaseFactory = serviceProvider.GetService<IBinanceDbContextFactory>();
                using var database = databaseFactory.CreateScopeDatabase();
                var aggregateCount = 0;
                var now = DateTime.Now;
                await foreach (var groupedData in GetAveragingMiniTickersAsync(database, AggregateDataIntervalType.FiveMinutes, now))
                {
                    var count = groupedData.Count();
                    aggregateCount += count;

                    var name = groupedData.Select(_ => _.ShortName).FirstOrDefault();
                    await _logger.TraceAsync($"{count} data successfully aggregated for '{name}'", cancellationToken: _cancellationTokenSource.Token);

                    await database.ColdUnitOfWork.MiniTickers.AddRangeAsync(groupedData, _cancellationTokenSource.Token);
                    await database.SaveChangesAsync(_cancellationTokenSource.Token);
                }

                watch.Stop();
                await _logger.InfoAsync(
                    $"Data aggregating ended!\n" +
                    $"{aggregateCount} entities successfully aggregate and saved\n" +
                    $"Time elapsed: {watch.Elapsed.TotalSeconds} s",
                    cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex, "Failed to aggregate and save miniTickers", cancellationToken: _cancellationTokenSource.Token);
            }
        }

        /// <summary>
        ///     Возвращает агрегированные (через усреднение) данные о мини-тикерах
        /// </summary>
        private async IAsyncEnumerable<IEnumerable<MiniTickerEntity>> GetAveragingMiniTickersAsync(
            IUnitOfWork database,
            AggregateDataIntervalType intervalType,
            DateTime now)
        {
            var interval = intervalType.ConvertToTimeSpan();
            var pageSize = 300;
            var shortNames = GetPairNames(database, now);
            foreach (var name in shortNames)
            {
                var averagedGroup = new List<MiniTickerEntity>();
                var aggregatedMiniTickers = new List<MiniTickerEntity>();
                var allCount = await database.ColdUnitOfWork.MiniTickers
                    .CreateQuery()
                    .CountAsync(_ => _.EventTime > now.AddDays(-1) && _.ShortName == name && _.AggregateDataInterval == AggregateDataIntervalType.Default);
                var pagesCount = (int)Math.Ceiling(allCount / (double)pageSize);
                for (var page = 0; page < pagesCount; page++)
                {
                    var entities = GetNonAggregatingMiniTickersEntities(database, name, page, pageSize, now);
                    averagedGroup.AddRange(GetAveragingTicker(entities, intervalType, interval));
                    database.ColdUnitOfWork.MiniTickers.RemoveRange(entities);
                }

                aggregatedMiniTickers.AddRange(averagedGroup);
                await database.SaveChangesAsync(_cancellationTokenSource.Token);

                yield return aggregatedMiniTickers;
            }
        }

        /// <summary>
        ///     Возвращает уникальные названия пар, хранящиеся в бд
        /// </summary>
        private static List<string> GetPairNames(IUnitOfWork database, DateTime now)
        {
            var pairs = database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(_ => _.EventTime > now.AddDays(-1) && _.AggregateDataInterval == AggregateDataIntervalType.Default)
                .Select(_ => _.ShortName)
                .Distinct()
                .ToList();

            return pairs;
        }

        /// <summary>
        ///     Возвращает еще не агрегированные модели за этот день
        /// </summary>
        private static IEnumerable<MiniTickerEntity> GetNonAggregatingMiniTickersEntities(IUnitOfWork database, string name, int page, int pageSize, DateTime now)
        {
            var models = database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(_ => _.EventTime > now.AddDays(-1) && _.ShortName == name && _.AggregateDataInterval == AggregateDataIntervalType.Default)
                .OrderBy(_ => _.EventTime)
                .Skip(page * pageSize)
                .Take(pageSize);

            return models;
        }

        /// <summary>
        ///     Возвращает усредненные объекты
        /// </summary>
        internal static List<MiniTickerEntity> GetAveragingTicker(
            IEnumerable<MiniTickerEntity> entities,
            AggregateDataIntervalType intervalType,
            TimeSpan interval)
        {
            var aggregatedMiniTickers = new List<MiniTickerEntity>();
            var first = entities.FirstOrDefault();
            var start = first.EventTime;
            var counter = 0;
            var aggregateObject = new MiniTickerEntity()
            {
                ShortName = first.ShortName,
                AggregateDataInterval = intervalType,
                EventTime = start,
            };

            foreach (var item in entities)
            {
                if (item.EventTime - start > interval)
                {
                    AveragingFields(aggregateObject, counter);
                    aggregatedMiniTickers.Add(aggregateObject);
                    counter = 1;
                    aggregateObject = (MiniTickerEntity)item.Clone();
                    aggregateObject.AggregateDataInterval = intervalType;
                    start = item.EventTime;
                    continue;
                }

                AggregateFields(item, aggregateObject);
                counter++;
            }

            AveragingFields(aggregateObject, counter);
            aggregatedMiniTickers.Add(aggregateObject);

            return aggregatedMiniTickers;
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
                var count = await database.HotUnitOfWork.HotMiniTickers.RemoveUntilAsync(now.AddDays(-1), _cancellationTokenSource.Token);

                await _logger.InfoAsync(
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

            StopAsync().Wait();
            _isDisposed = true;
        }

        #endregion
    }
}
