using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDataService.Configuration.AggregatorConfigs;
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

namespace BinanceDataService.DataAggregators
{
    /// <summary>
    ///     Базовый класс агрегирования данных
    /// </summary>
    internal class DataAggregator : IDisposable
    {
        #region Fields

        private readonly AggregatorConfigBase _configuration;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ILoggerDecorator _logger;
        private readonly IRecurringJobScheduler _scheduler;
        private TriggerKey _triggerKey;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <summary>
        ///     Создает агрегатор данных
        /// </summary>
        /// <param name="logger"> Логгер </param>
        /// <param name="scheduler">  Создает расписания агрегирования данных </param>
        /// <param name="aggregatorConfig"> Настройки агрегирования </param>
        public DataAggregator(ILoggerDecorator logger, IRecurringJobScheduler scheduler, AggregatorConfigBase aggregatorConfig)
        {
            _logger = logger;
            _scheduler = scheduler;
            _configuration = aggregatorConfig;
            _cancellationTokenSource = new();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Активный ли агррегатор данных
        /// </summary>
        public bool IsActive => _configuration?.IsNeedToAggregateColdData ?? false;

        #endregion

        #region Internal methods

        /// <summary>
        ///     Агрегирует и сохраняет данные
        /// </summary>
        internal virtual async Task AggregateAndSaveDataAsync(IServiceProvider serviceProvider)
        {
            await _logger.InfoAsync(
               $"{_configuration.AggregateDataInterval} data aggregating started!",
               cancellationToken: _cancellationTokenSource.Token);

            try
            {
                var watch = new Stopwatch();
                watch.Start();
                var databaseFactory = serviceProvider.GetService<IBinanceDbContextFactory>()
                    ?? throw new InvalidOperationException($"{nameof(IBinanceDbContextFactory)} not registered!");
                using var database = databaseFactory.CreateScopeDatabase();
                var aggregateCount = 0;
                var now = DateTime.Now;
                await foreach (var groupedData in GetAveragingMiniTickersAsync(database))
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
                    $"{_configuration.AggregateDataInterval} data aggregating ended!\n" +
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
        ///     Запускает агрегирование данных
        /// </summary>
        internal virtual async Task StartAsync()
        {
            if (_configuration?.IsNeedToAggregateColdData ?? false)
            {
                _triggerKey = await _scheduler.ScheduleAsync(_configuration.AggregateDataCron, AggregateAndSaveDataAsync);
            }
        }

        /// <summary>
        ///     Останавливает агрегирование данных
        /// </summary>
        internal virtual async Task StopAsync()
        {
            if (_configuration?.IsNeedToAggregateColdData ?? false)
            {
                await _scheduler?.UnscheduleAsync(_triggerKey);
            }
        }

        /// <summary>
        ///     Возвращает агрегированные (через усреднение) данные о мини-тикерах
        /// </summary>
        internal async IAsyncEnumerable<IEnumerable<MiniTickerEntity>> GetAveragingMiniTickersAsync(IUnitOfWork database)
        {
            var pageSize = 300;
            var shortNames = GetPairNames(database);
            foreach (var name in shortNames)
            {
                var averagedGroup = new List<MiniTickerEntity>();
                var aggregatedMiniTickers = new List<MiniTickerEntity>();
                var allCount = await database.ColdUnitOfWork.MiniTickers
                    .CreateQuery()
                    .CountAsync(_ => _.ShortName == name && _configuration.AggregateDataInterval > _.AggregateDataInterval);
                var pagesCount = (int)Math.Ceiling(allCount / (double)pageSize);
                for (var page = 0; page < pagesCount; page++)
                {
                    var entities = GetNonAggregatingMiniTickersEntities(database, name, page, pageSize);
                    averagedGroup.AddRange(GetAveragingTicker(entities, _configuration.AggregateDataInterval));
                    database.ColdUnitOfWork.MiniTickers.RemoveRange(entities);
                }

                aggregatedMiniTickers.AddRange(averagedGroup);
                await database.SaveChangesAsync(_cancellationTokenSource.Token);

                yield return aggregatedMiniTickers;
            }
        }

        /// <summary>
        ///     Возвращает еще не агрегированные модели
        /// </summary>
        internal IEnumerable<MiniTickerEntity> GetNonAggregatingMiniTickersEntities(
            IUnitOfWork database,
            string name,
            int page,
            int pageSize)
        {
            var models = database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(_ => _.ShortName == name && _configuration.AggregateDataInterval > _.AggregateDataInterval)
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
            AggregateDataIntervalType intervalType)
        {
            var interval = intervalType.ConvertToTimeSpan();
            var aggregatedMiniTickers = new List<MiniTickerEntity>();
            var first = entities.FirstOrDefault();
            var start = first.EventTime;
            var counter = 0;
            var aggregateObject = new MiniTickerEntity()
            {
                ShortName = first.ShortName,
                AggregateDataInterval = intervalType,
                EventTime = start,
                MinPrice = double.MaxValue,
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
        ///     Агрегирует поля объектов в один объект
        /// </summary>
        internal static void AggregateFields(MiniTickerEntity addedObject, MiniTickerEntity aggregateObject)
        {
            aggregateObject.OpenPrice += addedObject.OpenPrice;
            aggregateObject.ClosePrice += addedObject.ClosePrice;
            aggregateObject.MaxPrice = aggregateObject.MaxPrice > addedObject.MaxPrice ? aggregateObject.MaxPrice : addedObject.MaxPrice;
            aggregateObject.MinPrice = aggregateObject.MinPrice < addedObject.MinPrice ? aggregateObject.MinPrice : addedObject.MinPrice;
            aggregateObject.QuotePurchaseVolume += addedObject.QuotePurchaseVolume;
            aggregateObject.BasePurchaseVolume += addedObject.BasePurchaseVolume;
        }

        /// <summary>
        ///     Усредняет значения полей
        /// </summary>
        internal static void AveragingFields(MiniTickerEntity aggregateObject, int counter)
        {
            aggregateObject.OpenPrice /= counter;
            aggregateObject.ClosePrice /= counter;
            aggregateObject.QuotePurchaseVolume /= counter;
            aggregateObject.BasePurchaseVolume /= counter;
        }

        #endregion

        /// <summary>
        ///     Возвращает уникальные названия пар, хранящиеся в бд
        /// </summary>
        private List<string> GetPairNames(IUnitOfWork database)
        {
            var pairs = database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(_ => _configuration.AggregateDataInterval > _.AggregateDataInterval)
                .Select(_ => _.ShortName)
                .Distinct()
                .ToList();

            return pairs;
        }

        #region Implementation IDisposable

        /// <inheritdoc />
        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            StopAsync().Wait();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _isDisposed = true;
        }

        #endregion
    }
}
