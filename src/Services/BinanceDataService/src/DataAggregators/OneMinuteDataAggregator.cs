using BinanceDatabase;
using BinanceDatabase.Enums;
using BinanceDataService.Configuration.AggregatorConfigs;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Scheduler;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDataService.DataAggregators
{
    /// <summary>
    ///     Агрегирует данные по одной минуте
    /// </summary>
    internal sealed class OneMinuteDataAggregator : DataAggregatorBase
    {
        #region Fields

        private readonly ILoggerDecorator _logger;
        private readonly IRecurringJobScheduler _scheduler;
        private TriggerKey _triggerKey;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <summary>
        ///     Создает агрегатор данных по одной минуте
        /// </summary>
        /// <param name="logger"> Логгер </param>
        /// <param name="scheduler"> Шедаляр для задачи расписания агрегирования данных </param>
        /// <param name="oneHourAggregatorConfig"> Настройки агрегирования </param>
        private OneMinuteDataAggregator(ILoggerDecorator logger, IRecurringJobScheduler scheduler, OneHourAggregatorConfig oneHourAggregatorConfig)
        {
            _logger = logger;
            _scheduler = scheduler;
            Configuration = oneHourAggregatorConfig;
            CancellationTokenSource = new();
        }

        #endregion

        #region Override Properties

        /// <inheritdoc />
        public override AggregatorConfigBase Configuration { get; protected set; }

        /// <inheritdoc />
        public override CancellationTokenSource CancellationTokenSource { get; protected set; }

        /// <inheritdoc />
        public override AggregateDataIntervalType IntervalType { get; protected set; }

        #endregion

        #region Override methods

        /// <inheritdoc />
        public override async Task StartAsync()
        {
            if (Configuration.IsNeedToAggregateColdData)
            {
                _triggerKey = await _scheduler.ScheduleAsync(Configuration.AggregateDataCron, AggregateAndSaveDataAsync);
            }
        }

        /// <inheritdoc />
        public override async Task StopAsync()
        {
            if (Configuration.IsNeedToAggregateColdData)
            {
                await _scheduler?.UnscheduleAsync(_triggerKey);
            }
        }

        /// <inheritdoc />
        internal override async Task AggregateAndSaveDataAsync(IServiceProvider serviceProvider)
        {
            await _logger.InfoAsync(
               "One minute data aggregating started!",
               cancellationToken: CancellationTokenSource.Token);

            try
            {
                var watch = new Stopwatch();
                watch.Start();
                var databaseFactory = serviceProvider.GetService<IBinanceDbContextFactory>();
                using var database = databaseFactory.CreateScopeDatabase();
                var aggregateCount = 0;
                var now = DateTime.Now;
                await foreach (var groupedData in GetAveragingMiniTickersAsync(database))
                {
                    var count = groupedData.Count();
                    aggregateCount += count;

                    var name = groupedData.Select(_ => _.ShortName).FirstOrDefault();
                    await _logger.TraceAsync($"{count} data successfully aggregated for '{name}'", cancellationToken: CancellationTokenSource.Token);

                    await database.ColdUnitOfWork.MiniTickers.AddRangeAsync(groupedData, CancellationTokenSource.Token);
                    await database.SaveChangesAsync(CancellationTokenSource.Token);
                }

                watch.Stop();
                await _logger.InfoAsync(
                    $"One minute data aggregating ended!\n" +
                    $"{aggregateCount} entities successfully aggregate and saved\n" +
                    $"Time elapsed: {watch.Elapsed.TotalSeconds} s",
                    cancellationToken: CancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex, "Failed to aggregate and save miniTickers", cancellationToken: CancellationTokenSource.Token);
            }
        }

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            StopAsync().Wait();
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            _isDisposed = true;
        }

        #endregion
    }
}
