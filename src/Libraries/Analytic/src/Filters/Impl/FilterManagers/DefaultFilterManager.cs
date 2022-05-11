using Analytic.Models;
using BinanceDatabase;
using Common.Models;
using ExchangeLibrary;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.Filters.Impl.FilterManagers
{
    /// <inheritdoc cref="FilterManagerBase"/>
    public class DefaultFilterManager : FilterManagerBase
    {
        #region Fields

        private const int PricePercentDeviationsNumberToAnalyze = 60;
        private readonly ILoggerDecorator _logger;

        #endregion

        #region .ctor

        /// <inheritdoc cref="DefaultFilterManager"/>
        public DefaultFilterManager(ILoggerDecorator logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public override async Task<IEnumerable<InfoModel>> GetFilteredDataAsync<T>(
            IServiceScopeFactory serviceScopeFactory,
            IEnumerable<T> modelsToFilter,
            CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var exchange = scope.ServiceProvider.GetRequiredService<IExchange>();
            var databaseFactory = scope.ServiceProvider.GetRequiredService<IBinanceDbContextFactory>();
            var filteredModels = GetFilteredData(databaseFactory, modelsToFilter as IEnumerable<TradeObjectNamePriceModel>);
            return await GetExtendedFilteredModelsAsync(exchange, filteredModels.Take(2).ToList(), cancellationToken);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Первоначальная фильтрация данных
        /// </summary>
        private List<InfoModel> GetFilteredData(IBinanceDbContextFactory databaseFactory, IEnumerable<TradeObjectNamePriceModel> models)
        {
            var result = new List<InfoModel>();
            var paramountFilters = FilterGroups.Where(_ => _.Type == FilterGroupType.Primary).ToArray();
            var commonFilters = FilterGroups.Where(_ => _.Type == FilterGroupType.Common).ToArray();
            foreach (var model in models)
            {
                if (!paramountFilters.All(_ => _.CheckConditions(new(model.Name, model.Price))))
                {
                    continue;
                }

                var infoModel = new InfoModel(model.Name, model.Price)
                {
                    PricePercentDeviations = GetPriceDeviationsFromDatabase(databaseFactory, model.Name)
                };

                // если у модели есть спец группа фильтров, то по общим ее фильтровать не надо
                var specialPriceFilters = GetSpecialFiltersForModel(model);
                if (specialPriceFilters.Any())
                {
                    if (specialPriceFilters.All(_ => _.CheckConditions(infoModel)))
                    {
                        _logger.TraceAsync($"Ticker {model.Name} suitable for SPECIAL filters").Wait(5 * 1000);
                        result.Add(infoModel);
                    }

                    continue;
                }

                if (commonFilters.All(_ => _.CheckConditions(infoModel)))
                {
                    _logger.TraceAsync($"Ticker {model.Name} suitable for COMMON filters").Wait(5 * 1000);
                    result.Add(infoModel);
                }
            }

            return result;
        }

        /// <summary>
        ///     Получить отклонения цены из базы данных
        /// </summary>
        private static IQueryable<double> GetPriceDeviationsFromDatabase(IBinanceDbContextFactory databaseFactory, string pair)
        {
            using var database = databaseFactory.CreateScopeDatabase();

            return database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(_ => _.ShortName == pair && _.AggregateDataInterval == BinanceDatabase.Enums.AggregateDataIntervalType.OneMinute)
                .OrderByDescending(_ => _.EventTime)
                .Select(_ => _.PriceDeviationPercent)
                .Take(PricePercentDeviationsNumberToAnalyze);
        }

        /// <summary>
        ///     Возвращает группы фильтров для конкретной модели
        /// </summary>
        private IFilterGroup[] GetSpecialFiltersForModel(TradeObjectNamePriceModel model) =>
            FilterGroups.Where(_ => _.Type == FilterGroupType.Special && _.IsFilter(model.Name)).ToArray();

        /// <summary>
        ///     Получение дополнительных данных и их фильтрация
        /// </summary>
        private async Task<List<InfoModel>> GetExtendedFilteredModelsAsync(
            IExchange exchange,
            List<InfoModel> models,
            CancellationToken cancellationToken)
        {
            var commonLatestFilters = FilterGroups.Where(_ => _.Type == FilterGroupType.CommonLatest).ToArray();
            for (var i = 0; i < models.Count; i++)
            {
                var model = models[i];
                var specialFilters = GetSpecialLatestFiltersForModel(model);
                if (!specialFilters.Any() && !commonLatestFilters.Any())
                {
                    continue;
                }

                var extendedModel = await exchange.Marketdata.GetOrderBookAsync(model.TradeObjectName, 1000, cancellationToken);
                model.BidVolume = extendedModel.Bids.Sum(_ => _.Quantity);
                model.AskVolume = extendedModel.Asks.Sum(_ => _.Quantity);

                // если у модели есть спец группа фильтров, то по общим ее фильтровать не надо
                if (specialFilters.Any())
                {
                    if (!specialFilters.All(_ => _.CheckConditions(model)))
                    {
                        _logger
                            .TraceAsync(
                                $"Ticker {model.TradeObjectName} DOES NOT suitable for SPECIAL LATEST filters",
                                cancellationToken: cancellationToken)
                            .Wait(5 * 1000, cancellationToken);
                        models.Remove(model);
                        i--;
                    }

                    continue;
                }

                if (!commonLatestFilters.All(_ => _.CheckConditions(model)))
                {
                    _logger
                        .TraceAsync(
                            $"Ticker {model.TradeObjectName} DOES NOT suitable for COMMON LATEST filters",
                            cancellationToken: cancellationToken)
                        .Wait(5 * 1000, cancellationToken);
                    models.Remove(model);
                    i--;
                }
            }

            return models;
        }

        /// <summary>
        ///     Возвращает группы для последней фильтрации для конкретной модели
        /// </summary>
        private IFilterGroup[] GetSpecialLatestFiltersForModel(InfoModel model) =>
            FilterGroups.Where(_ => _.Type == FilterGroupType.SpecialLatest && _.IsFilter(model.TradeObjectName)).ToArray();

        #endregion
    }
}
