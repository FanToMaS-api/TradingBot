using Analytic.AnalyticUnits;
using Analytic.Filters;
using Analytic.Models;
using ExchangeLibrary;
using NLog;
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
    public class BinanceAnalyticService : IAnalyticService
    {
        #region Fields

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IExchange _exchange;
        private readonly IRecurringJobScheduler _scheduler;
        private readonly Dictionary<string, InfoModel> _infoModels = new();
        private TriggerKey _triggerKey;
        private readonly CancellationTokenSource _cts;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceAnalyticService"/>
        public BinanceAnalyticService(
            IExchange exchange,
            IRecurringJobScheduler recurringJobScheduler,
            CancellationTokenSource cancellationTokenSource)
        {
            _exchange = exchange;
            _scheduler = recurringJobScheduler;
            _cts = cancellationTokenSource;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public List<IProfileGroup> ProfileGroups { get; } = new();

        /// <inheritdoc />
        public List<IFilter> Filters { get; } = new();

        /// <inheritdoc />
        public EventHandler<InfoModel[]> OnModelsFiltered { get; set; }

        /// <inheritdoc />
        public EventHandler<AnalyticResultModel[]> OnReadyToBuy { get; set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            // каждые 45 секунд вызываем метода анализа
            _triggerKey = await _scheduler.ScheduleAsync(Cron.Secondly(45), AnalyzeAsync);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.UnscheduleAsync(_triggerKey);
        }

        #region Add/Remove methods

        /// <inheritdoc />
        public void AddProfileGroup(IProfileGroup profileGroup) => ProfileGroups.Add(profileGroup);

        /// <inheritdoc />
        public void AddFilter(IFilter filter) => Filters.Add(filter);

        /// <inheritdoc />
        public bool RemoveFilter(IFilter filter) => Filters.Remove(filter);

        /// <inheritdoc />
        public bool RemoveFilter(string filterName)
        {
            foreach (var filter in Filters)
            {
                if (filter.FilterName == filterName)
                {
                    return Filters.Remove(filter);
                }
            }

            return false;
        }

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
        ///     Задача анализа текузей ситуации на рынке
        /// </summary>
        private async Task AnalyzeAsync(IServiceProvider serviceProvider)
        {
            var cancellationToken = _cts.Token;
            try
            {
                var filteredModels = await DataReceivedAndFilterAsync(cancellationToken);
                var extendedFilteredModels = await ExtendedDataReceivedAndFilterAsync(filteredModels, cancellationToken);
                if (extendedFilteredModels.Any())
                {
                    OnModelsFiltered?.Invoke(this, extendedFilteredModels.ToArray());

                    await ModelsAnalyzeAsync(extendedFilteredModels, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to analyze");
            }
        }

        /// <summary>
        ///     Получение общих данных с биржи и их первоначальная фильтрация
        /// </summary>
        private async Task<List<InfoModel>> DataReceivedAndFilterAsync(CancellationToken cancellationToken)
        {
            var models = await _exchange.GetSymbolPriceTickerAsync(null, cancellationToken);
            var result = new List<InfoModel>();

            // все фильтры кроме фильтра объемов, так как оно тестируется после получения доп. данных
            var priceNameFilters = Filters.Where(_ => _ is not VolumeFilter);
            foreach (var model in models)
            {
                if (!_infoModels.ContainsKey(model.ShortName))
                {
                    _infoModels[model.ShortName] = new()
                    {
                        TradeObjectName = model.ShortName,
                        LastPrice = model.Price,
                    };

                    continue;
                }

                var infoModel = _infoModels[model.ShortName];
                (infoModel.PrevPrice, infoModel.LastPrice) = (infoModel.LastPrice, model.Price);
                var lastDeviation = GetDeviation(infoModel.PrevPrice, infoModel.LastPrice);
                infoModel.PricePercentDeviations.Add(lastDeviation);

                if (priceNameFilters.All(_ => _.CheckConditions(infoModel)))
                {
                    result.Add(infoModel);
                }
            }

            return result;
        }

        /// <summary>
        ///     Возвращает процентное отклонение новой цены от старой
        /// </summary>
        private static double GetDeviation(double oldPrice, double newPrice) => (newPrice / (double)oldPrice - 1) * 100;

        /// <summary>
        ///     Получение дополнительных данных и их фильтрация
        /// </summary>
        private async Task<List<InfoModel>> ExtendedDataReceivedAndFilterAsync(List<InfoModel> models, CancellationToken cancellationToken)
        {
            var volumeFilters = Filters.Where(_ => _ is VolumeFilter);
            var result = new List<InfoModel>();
            foreach (var model in models)
            {
                var extendedModel = await _exchange.GetOrderBookAsync(model.TradeObjectName, 5000, cancellationToken);
                model.BidVolume = extendedModel.Bids.Sum(_ => _.Quantity);
                model.AskVolume = extendedModel.Asks.Sum(_ => _.Quantity);

                if (volumeFilters.All(_ => _.CheckConditions(model)))
                {
                    result.Add(model);
                }
            }

            return result;
        }

        /// <summary>
        ///     Запускает анализ каждой модели для решения о дальнейшей покупке
        /// </summary>
        private async Task ModelsAnalyzeAsync(List<InfoModel> models, CancellationToken cancellationToken)
        {
            var modelsToBuy = new List<AnalyticResultModel>();
            foreach (var model in models)
            {
                foreach (var profileGroup in ProfileGroups)
                {
                    var (isSuccessful, analyticModel) = await profileGroup.TryAnalyzeAsync(model, cancellationToken);
                    if (!isSuccessful)
                    {
                        continue;
                    }

                    var conflictedModel = modelsToBuy.FirstOrDefault(_ => _.TradeObjectName == analyticModel.TradeObjectName);
                    if (conflictedModel is null)
                    {
                        modelsToBuy.Add(analyticModel);
                        continue;
                    }

                    // усредняем
                    conflictedModel.RecommendedPurchasePrice += analyticModel.RecommendedPurchasePrice;
                    conflictedModel.RecommendedPurchasePrice /= 2;
                }
            }

            if (modelsToBuy.Any())
            {
                OnReadyToBuy?.Invoke(this, modelsToBuy.ToArray());
            }
        }

        #endregion
    }
}
