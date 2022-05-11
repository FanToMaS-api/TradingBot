using Analytic.AnalyticUnits;
using Analytic.Filters;
using Analytic.Models;
using BinanceDatabase;
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

namespace Analytic.Binance
{
    /// <inheritdoc cref="IAnalyticService"/>
    internal class BinanceAnalyticService : IAnalyticService
    {
        #region Fields

        private readonly ILoggerDecorator _logger;
        private readonly IExchange _exchange;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRecurringJobScheduler _scheduler;
        private TriggerKey _triggerKey;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceAnalyticService"/>
        public BinanceAnalyticService(
            IExchange exchange,
            IServiceScopeFactory serviceScopeFactory,
            IRecurringJobScheduler recurringJobScheduler,
            ILoggerDecorator logger)
        {
            _exchange = exchange;
            _serviceScopeFactory = serviceScopeFactory;
            _scheduler = recurringJobScheduler;
            _logger = logger;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public List<IProfileGroup> ProfileGroups { get; } = new();

        /// <inheritdoc />
        public List<FilterManagerBase> FilterManagers { get; internal set; }

        /// <inheritdoc />
        public EventHandler<InfoModel[]> OnModelsFiltered { get; set; }

        /// <inheritdoc />
        public EventHandler<AnalyticResultModel[]> OnSuccessfulAnalize { get; set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken.Register(() => StopAsync(CancellationToken.None).GetAwaiter().GetResult());

            // каждую минуту на 7ю секунду вызываем метода анализа
            _triggerKey = await _scheduler.ScheduleAsync(Cron.MinutelyOnSecond(7), AnalyzeAsync);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken) =>
            await _scheduler.UnscheduleAsync(_triggerKey);

        #region Add/Remove methods

        /// <inheritdoc />
        public void AddFilterManager(FilterManagerBase filterManager) =>
            FilterManagers.Add(filterManager);

        /// <inheritdoc />
        public void ClearFilterManagers() => FilterManagers.Clear();

        /// <inheritdoc />
        public void AddProfileGroup(IProfileGroup profileGroup) => ProfileGroups.Add(profileGroup);

        /// <inheritdoc />
        public bool RemoveProfileGroup(IProfileGroup profileGroup) => ProfileGroups.Remove(profileGroup);

        /// <inheritdoc />
        public bool RemoveProfile(string profileName)
        {
            foreach (var group in ProfileGroups)
            {
                if (group.Remove(profileName))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region Private methods

        /// <summary>
        ///     Задача анализа текущей ситуации на рынке
        /// </summary>
        private async Task AnalyzeAsync(IServiceProvider serviceProvider)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                await _scheduler.UnscheduleAsync(_triggerKey);
            }

            try
            {
                var models = await _exchange.Marketdata.GetSymbolPriceTickerAsync(null, _cancellationTokenSource.Token);
                var extendedFilteredModels = new List<InfoModel>();
                foreach (var filterManager in FilterManagers)
                {
                    extendedFilteredModels.AddRange(await filterManager.GetFilteredDataAsync(
                        _serviceScopeFactory,
                        models,
                        _cancellationTokenSource.Token));
                }
                
                if (!extendedFilteredModels.Any())
                {
                    return;
                }

                OnModelsFiltered?.Invoke(this, extendedFilteredModels.ToArray());

                var analyzedModels = await GetAnalyzedModelsAsync(
                    extendedFilteredModels.ToList(),
                    _cancellationTokenSource.Token);
                if (analyzedModels.Any())
                {
                    OnSuccessfulAnalize?.Invoke(this, analyzedModels.ToArray());
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex, "Failed to analyze", cancellationToken: _cancellationTokenSource.Token);
            }
        }

        /// <summary>
        ///     Запускает анализ каждой модели для решения о дальнейшей покупке
        /// </summary>
        private async Task<List<AnalyticResultModel>> GetAnalyzedModelsAsync(List<InfoModel> models, CancellationToken cancellationToken)
        {
            var modelsToBuy = new List<AnalyticResultModel>();
            foreach (var model in models)
            {
                foreach (var profileGroup in ProfileGroups.Where(_ => _.IsActive))
                {
                    var (isSuccessful, analyticModel) = await profileGroup.TryAnalyzeAsync(
                        _serviceScopeFactory,
                        model,
                        cancellationToken);
                    if (!isSuccessful)
                    {
                        continue;
                    }

                    modelsToBuy.Add(analyticModel);
                    await _logger.TraceAsync($"Successful analysis model {analyticModel.TradeObjectName}\n" +
                        $"Has image: {analyticModel.HasPredictionImage}\n" +
                        $"Path to image: {analyticModel.ImagePath}",
                        cancellationToken: cancellationToken);
                }
            }

            return modelsToBuy;
        }

        #endregion

        #region Implemetation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }

        #endregion
    }
}
