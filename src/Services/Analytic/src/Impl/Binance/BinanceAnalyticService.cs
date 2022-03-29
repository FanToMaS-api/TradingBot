using Analytic.AnalyticUnits;
using Analytic.Filters;
using Analytic.Models;
using Common.Models;
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
        private readonly CancellationToken _cancellationToken;

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
            _cancellationToken = cancellationTokenSource.Token;
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
        public EventHandler<AnalyticResultModel[]> OnModelsReadyToBuy { get; set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => StopAsync(CancellationToken.None).GetAwaiter().GetResult());

            // каждую 1 минуту вызываем метода анализа
            _triggerKey = await _scheduler.ScheduleAsync("0 * * ? * *", AnalyzeAsync);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken) =>
            await _scheduler.UnscheduleAsync(_triggerKey);

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
        ///     Задача анализа текущей ситуации на рынке
        /// </summary>
        private async Task AnalyzeAsync(IServiceProvider serviceProvider)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                await _scheduler.UnscheduleAsync(_triggerKey);
            }

            try
            {
                var models = await _exchange.GetSymbolPriceTickerAsync(null, _cancellationToken);
                var filteredModels = GetFilteredData(models);
                var extendedFilteredModels = await GetExtendedFilteredModelsAsync(filteredModels, _cancellationToken);
                if (extendedFilteredModels.Any())
                {
                    OnModelsFiltered?.Invoke(this, extendedFilteredModels.ToArray());

                    var analyzedModels = await GetAnalyzedModelsAsync(extendedFilteredModels, _cancellationToken);
                    if (analyzedModels.Any())
                    {
                        OnModelsReadyToBuy?.Invoke(this, analyzedModels.ToArray());
                    }
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
        private List<InfoModel> GetFilteredData(IEnumerable<TradeObjectNamePriceModel> models)
        {
            var result = new List<InfoModel>();

            var priceDefaultFilters = GetDefaultFilters();
            var nameFilters = Filters.Where(_ => _.Type == FilterType.NameFilter);
            foreach (var model in models)
            {
                if (!nameFilters.All(_ => _.CheckConditions(new(model.Name, -1)))) // просто отсеивание неподходящих моделей
                {
                    continue;
                }

                if (!_infoModels.ContainsKey(model.Name))
                {
                    _infoModels[model.Name] = new(model.Name, model.Price);

                    continue;
                }

                var infoModel = _infoModels[model.Name];
                CalculateData(infoModel, model);

                var specialPriceFilters = GetSpecialFiltersForModel(model);
                if (specialPriceFilters.Any())
                {
                    if (specialPriceFilters.All(_ => _.CheckConditions(infoModel)))
                    {
                        result.Add(infoModel);
                    }

                    continue;
                }

                if (priceDefaultFilters.All(_ => _.CheckConditions(infoModel)))
                {
                    result.Add(infoModel);
                }
            }

            return result;
        }

        /// <summary>
        ///     Возвращает фильтры для цены, работающие для всех объектов торговли
        /// </summary>
        private IEnumerable<IFilter> GetDefaultFilters() =>
            Filters.Where(_ =>
                (_.Type == FilterType.PriceFilter || _.Type == FilterType.PriceDeviationFilter)
                && string.IsNullOrEmpty(_.TargetTradeObjectName));

        /// <summary>
        ///     Производит новые расчеты для модели <paramref name="infoModel"/>
        /// </summary>
        private static void CalculateData(InfoModel infoModel, TradeObjectNamePriceModel model)
        {
            (infoModel.PrevPrice, infoModel.LastPrice) = (infoModel.LastPrice, model.Price); // Swap

            var lastDeviation = GetDeviation(infoModel.PrevPrice, infoModel.LastPrice);
            infoModel.PricePercentDeviations.Add(lastDeviation);
            infoModel.LastDeviation = lastDeviation;
            if (infoModel.PricePercentDeviations.Count > 40)
            {
                // пока нет сохранения в бд, удаляю с целью экономии памяти
                infoModel.PricePercentDeviations.RemoveRange(0, 30);
            }
        }

        /// <summary>
        ///     Возвращает процентное отклонение новой цены от старой
        /// </summary>
        private static double GetDeviation(double oldPrice, double newPrice) => (newPrice / (double)oldPrice - 1) * 100;

        /// <summary>
        ///     Возвращает фильтры для конкретной модели
        /// </summary>
        private IEnumerable<IFilter> GetSpecialFiltersForModel(TradeObjectNamePriceModel model) =>
            Filters.Where(_ =>
                (_.Type == FilterType.PriceFilter || _.Type == FilterType.PriceDeviationFilter)
                && !string.IsNullOrEmpty(_.TargetTradeObjectName)
                && model.Name.Contains(_.TargetTradeObjectName, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        ///     Получение дополнительных данных и их фильтрация
        /// </summary>
        private async Task<List<InfoModel>> GetExtendedFilteredModelsAsync(List<InfoModel> models, CancellationToken cancellationToken)
        {
            var volumeFilters = Filters.Where(_ => _.Type == FilterType.VolumeFilter && string.IsNullOrEmpty(_.TargetTradeObjectName));
            for (var i = 0; i < models.Count; i++)
            {
                var model = models[i];
                var extendedModel = await _exchange.GetOrderBookAsync(model.TradeObjectName, 5000, cancellationToken);
                model.BidVolume = extendedModel.Bids.Sum(_ => _.Quantity);
                model.AskVolume = extendedModel.Asks.Sum(_ => _.Quantity);

                var specialFilters = GetSpecialVolumeFiltersForModel(model);
                if (specialFilters.Any())
                {
                    if (!specialFilters.All(_ => _.CheckConditions(model)))
                    {
                        models.Remove(model);
                        i--;
                    }

                    continue;
                }

                if (!volumeFilters.All(_ => _.CheckConditions(model)))
                {
                    models.Remove(model);
                    i--;
                }
            }

            return models;
        }

        /// <summary>
        ///     Возвращает фильтры для конкретной модели
        /// </summary>
        private IEnumerable<IFilter> GetSpecialVolumeFiltersForModel(InfoModel model) =>
            Filters.Where(_ =>
                _.Type == FilterType.VolumeFilter
                && string.IsNullOrEmpty(_.TargetTradeObjectName)
                && model.TradeObjectName.Contains(_.TargetTradeObjectName, StringComparison.InvariantCultureIgnoreCase));

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
                    var (isSuccessful, analyticModel) = await profileGroup.TryAnalyzeAsync(_exchange, model, cancellationToken);
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

            return modelsToBuy;
        }

        #endregion
    }
}
