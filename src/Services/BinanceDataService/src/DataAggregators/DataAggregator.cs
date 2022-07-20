using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDataService.Configuration.AggregatorConfigs;
using Common.Helpers;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly AggregateDataIntervalType _reducedDataIntervalType;
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
        public DataAggregator(
            ILoggerDecorator logger,
            IRecurringJobScheduler scheduler,
            AggregatorConfigBase aggregatorConfig)
        {
            _logger = logger;
            _scheduler = scheduler;
            _configuration = aggregatorConfig;
            _cancellationTokenSource = new();
            _reducedDataIntervalType = GetReducedDataAggregationInterval();
        }

        #endregion

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

        #region Properties

        /// <summary>
        ///     Активный ли агрегатор данных
        /// </summary>
        public bool IsActive => _configuration?.IsNeedToAggregateColdData ?? false;

        #endregion

        #region Internal methods

        /// <summary>
        ///     Агрегирует и сохраняет данные
        /// </summary>
        internal virtual async Task AggregateAndSaveDataAsync(IServiceProvider serviceProvider)
        {
            try
            {
                await _logger.InfoAsync(
                   $"{_configuration.AggregateDataInterval} data aggregating started!",
                   cancellationToken: _cancellationTokenSource.Token);

                var watch = new Stopwatch();
                watch.Start();

                var databaseFactory = serviceProvider.GetService<IBinanceDbContextFactory>()
                    ?? throw new InvalidOperationException($"{nameof(IBinanceDbContextFactory)} not registered!");
                using var database = databaseFactory.CreateScopeDatabase();

                var totalAggregatedCount = 0;
                await foreach (var groupedData in GetAveragingMiniTickersAsync(database))
                {
                    var count = groupedData.Count();
                    totalAggregatedCount += count;

                    var name = groupedData.FirstOrDefault().ShortName;
                    await LogAboutSuccessfullyAggregationAsync(name, count);

                    await database.ColdUnitOfWork.MiniTickers.AddRangeAsync(groupedData, _cancellationTokenSource.Token);
                }

                await database.SaveChangesAsync(_cancellationTokenSource.Token);

                watch.Stop();
                await LogAboutEndDataAggregationAsync(totalAggregatedCount, watch.Elapsed.TotalSeconds);
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
                _triggerKey = await _scheduler.GeneralScheduleAsync(_configuration.AggregateDataCron, AggregateAndSaveDataAsync);
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
                var allCount = await CountEntitiesWithName(database, name);
                if (allCount == 0)
                {
                    continue;
                }

                var pagesCount = (int)Math.Ceiling(allCount / (double)pageSize);
                var query = CreateQuery(database, _ => _.ShortName == name && _reducedDataIntervalType == _.AggregateDataInterval);
                for (var page = 0; page < pagesCount; page++)
                {
                    var entities = GetNextPage(query, page, pageSize);
                    if (entities.Any())
                    {
                        averagedGroup.AddRange(GetAveragingTickers(entities, _configuration.AggregateDataInterval));
                    }
                }

                aggregatedMiniTickers.AddRange(averagedGroup);
                await database.SaveChangesAsync(_cancellationTokenSource.Token);

                yield return aggregatedMiniTickers;
            }
        }

        private async Task<int> CountEntitiesWithName(IUnitOfWork database, string name)
        {
            return await database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .CountAsync(
                    _ => _.ShortName == name && _reducedDataIntervalType == _.AggregateDataInterval,
                    _cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Возвращает еще не агрегированные модели
        /// </summary>
        internal IEnumerable<MiniTickerEntity> GetNextPage(
            IOrderedQueryable<MiniTickerEntity> query,
            int page,
            int pageSize)
        {
            var models = query
                .Skip(page * pageSize)
                .Take(pageSize);

            return models;
        }

        /// <summary>
        ///     Возвращает усредненные объекты
        /// </summary>
        internal static List<MiniTickerEntity> GetAveragingTickers(
            IEnumerable<MiniTickerEntity> entities,
            AggregateDataIntervalType intervalType)
        {
            var firstEntity = entities.FirstOrDefault();
            var startTime = firstEntity.EventTime;
            var aggregateObject = new MiniTickerEntity()
            {
                ShortName = firstEntity.ShortName,
                AggregateDataInterval = intervalType,
                EventTime = startTime,
                MinPrice = double.MaxValue,
            };

            var aggregatedMiniTickers = new List<MiniTickerEntity>();
            var averagingObjectsCounter = 0;
            var interval = intervalType.ConvertToTimeSpan();
            foreach (var item in entities)
            {
                if (item.EventTime - startTime > interval)
                {
                    AveragingFields(aggregateObject, averagingObjectsCounter);

                    aggregatedMiniTickers.Add(aggregateObject);
                    averagingObjectsCounter = 1;

                    aggregateObject = (MiniTickerEntity)item.Clone();
                    aggregateObject.AggregateDataInterval = intervalType; // TODO: нахуя здесь это?
                    startTime = item.EventTime;
                    continue;
                }

                AggregateFields(item, aggregateObject);
                averagingObjectsCounter++;
            }

            AveragingFields(aggregateObject, averagingObjectsCounter);
            aggregatedMiniTickers.Add(aggregateObject);

            return aggregatedMiniTickers;
        }

        /// <summary>
        ///     Агрегирует поля объектов в один объект
        /// </summary>
        internal static void AggregateFields(MiniTickerEntity addedObject, MiniTickerEntity aggregateObject)
        {
            Assert.True(addedObject.ShortName == aggregateObject.ShortName, "Names are different!");
            aggregateObject.OpenPrice += addedObject.OpenPrice;
            aggregateObject.ClosePrice += addedObject.ClosePrice;
            aggregateObject.PriceDeviationPercent += addedObject.PriceDeviationPercent;
            aggregateObject.MaxPrice = aggregateObject.MaxPrice > addedObject.MaxPrice ? aggregateObject.MaxPrice : addedObject.MaxPrice;
            aggregateObject.MinPrice = aggregateObject.MinPrice < addedObject.MinPrice ? aggregateObject.MinPrice : addedObject.MinPrice;
            aggregateObject.QuotePurchaseVolume += addedObject.QuotePurchaseVolume;
            aggregateObject.BasePurchaseVolume += addedObject.BasePurchaseVolume;
        }

        /// <summary>
        ///     Усредняет значения полей
        /// </summary>
        internal static void AveragingFields(MiniTickerEntity aggregateObject, int averagingObjectsCounter)
        {
            Assert.True(averagingObjectsCounter > 0, $"{nameof(averagingObjectsCounter)} should be greater than 0");
            aggregateObject.OpenPrice /= averagingObjectsCounter;
            aggregateObject.ClosePrice /= averagingObjectsCounter;
            aggregateObject.QuotePurchaseVolume /= averagingObjectsCounter;
            aggregateObject.BasePurchaseVolume /= averagingObjectsCounter;
            aggregateObject.PriceDeviationPercent /= averagingObjectsCounter;
        }

        /// <summary>
        ///     Возвращает пониженный на 1 интервал для агрегирования данных
        /// </summary>
        /// <remarks>
        ///     Чтобы при агрегированиии например по 5ти минутному интервалу,
        ///     не попадались одновременно с 1минутными данными Default данные,
        ///     т.е. не выполнялась двойная агрегация
        /// </remarks>
        internal AggregateDataIntervalType GetReducedDataAggregationInterval()
        {
            var reducedDataAggregationInterval = _configuration.AggregateDataInterval - 1 < 0
                ? AggregateDataIntervalType.Default
                : _configuration.AggregateDataInterval - 1;

            return reducedDataAggregationInterval;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Лог об успешной агрегации данных для определенного тикера
        /// </summary>
        /// <param name="name"> Название тикера </param>
        /// <param name="count"> Кол-во агрегированных данных </param>
        private async Task LogAboutSuccessfullyAggregationAsync(string name, int count)
        {
            await _logger.TraceAsync(
                $"{count} data successfully aggregated for '{name}'",
                cancellationToken: _cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Лог об успешном окончании агрегирования данных
        /// </summary>
        /// <param name="aggregateCount"> Суммарное кол-во агрегированных данных </param>
        /// <param name="totalsSeconds"> Кол-во затраченных секунд на агрегацию </param>
        private async Task LogAboutEndDataAggregationAsync(int aggregateCount, double totalsSeconds)
        {
            await _logger.InfoAsync(
                $"{_configuration.AggregateDataInterval} data aggregating ended!\n" +
                $"{aggregateCount} entities successfully aggregate and saved\n" +
                $"Time elapsed: {totalsSeconds} s",
                cancellationToken: _cancellationTokenSource.Token);
        }

        /// <summary>
        ///     Создает запрос мини тикеров к базе данных
        /// </summary>
        /// <param name="func"> Функция отбора </param>
        private IOrderedQueryable<MiniTickerEntity> CreateQuery(IUnitOfWork database, Expression<Func<MiniTickerEntity, bool>> func)
        {
            return database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(func)
                .OrderBy(_ => _.EventTime);
        }

        /// <summary>
        ///     Возвращает уникальные названия пар, хранящиеся в бд
        /// </summary>
        private List<string> GetPairNames(IUnitOfWork database)
        {
            var pairs = database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(_ => _reducedDataIntervalType == _.AggregateDataInterval)
                .Select(_ => _.ShortName)
                .Distinct()
                .ToList();

            return pairs;
        }

        #endregion
    }
}
