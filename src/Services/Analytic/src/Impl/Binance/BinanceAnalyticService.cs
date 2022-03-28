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

            // каждую 1 секунд вызываем метода анализа
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
        ///     Задача анализа текузей ситуации на рынке
        /// </summary>
        private async Task AnalyzeAsync(IServiceProvider serviceProvider)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                await _scheduler.UnscheduleAsync(_triggerKey);
            }

            try
            {
                var filteredModels = await DataReceivedAndFilterAsync(_cancellationToken);
                var extendedFilteredModels = await ExtendedDataReceivedAndFilterAsync(filteredModels, _cancellationToken);
                if (extendedFilteredModels.Any())
                {
                    OnModelsFiltered?.Invoke(this, extendedFilteredModels.ToArray());

                    await ModelsAnalyzeAsync(extendedFilteredModels, _cancellationToken);
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

            // отделяю дефолтные фильтры для цены
            var priceDefaultFilters = Filters
                .Where(_ => _ is PriceFilter && string.IsNullOrEmpty((_ as PriceFilter).TargetTradeObjectName));
            var nameFilters = Filters.Where(_ => _ is NameFilter);
            foreach (var model in models)
            {
                if (!nameFilters.All(_ => _.CheckConditions(new() { TradeObjectName = model.ShortName })))
                {
                    continue;
                }

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
                (infoModel.PrevPrice, infoModel.LastPrice) = (infoModel.LastPrice, model.Price); // Swap

                var lastDeviation = GetDeviation(infoModel.PrevPrice, infoModel.LastPrice);
                infoModel.PricePercentDeviations.Add(lastDeviation);
                infoModel.LastDeviation = lastDeviation;
                if (infoModel.PricePercentDeviations.Count > 40)
                {
                    infoModel.PricePercentDeviations.RemoveRange(0, 30);
                }

                // выбираю специальные фильтры для пары, если они есть
                var specialPriceFilters = Filters
                    .Where(_ => _ is PriceFilter
                        && !string.IsNullOrEmpty((_ as PriceFilter).TargetTradeObjectName)
                        && model.ShortName.Contains((_ as PriceFilter).TargetTradeObjectName));
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

                if (volumeFilters.Any(_ => _.CheckConditions(model)))
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

            if (modelsToBuy.Any())
            {
                OnModelsReadyToBuy?.Invoke(this, modelsToBuy.ToArray());
            }
        }

        #endregion
    }
}
