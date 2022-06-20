using Analytic.Filters.FilterGroup.Impl;
using Analytic.Models;
using Common.Helpers;
using Common.Models;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.Filters.Impl.FilterManagers
{
    /// <inheritdoc cref="FilterManagerBase"/>
    public class DefaultFilterManager : FilterManagerBase
    {
        #region Fields

        private readonly ILoggerDecorator _logger;

        #endregion

        #region .ctor

        /// <inheritdoc cref="DefaultFilterManager"/>
        internal DefaultFilterManager(ILoggerDecorator logger, IList<IFilterGroup> filterGroups)
        {
            _logger = logger;
            FilterGroups = new ReadOnlyCollection<IFilterGroup>(filterGroups); 
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public override async Task<IEnumerable<InfoModel>> GetFilteredDataAsync<T>(
            IServiceScopeFactory serviceScopeFactory,
            IEnumerable<T> modelsToFilter,
            CancellationToken cancellationToken)
        {
            var filteredModels = await GetFilteredDataAsync(serviceScopeFactory, modelsToFilter as IEnumerable<TradeObjectNamePriceModel>, cancellationToken);
            return await GetExtendedFilteredModelsAsync(serviceScopeFactory, filteredModels, cancellationToken);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Первоначальная фильтрация данных
        /// </summary>
        private async Task<List<InfoModel>> GetFilteredDataAsync(
            IServiceScopeFactory serviceScopeFactory,
            IEnumerable<TradeObjectNamePriceModel> models,
            CancellationToken cancellationToken)
        {
            var result = new List<InfoModel>();
            var paramountFilters = FilterGroups.Where(_ => _.Type == FilterGroupType.Primary).ToArray();
            var commonFilters = FilterGroups.Where(_ => _.Type == FilterGroupType.Common).ToArray();
            foreach (var model in models)
            {
                if (!await paramountFilters.TrueForAllAsync(async _ => await _.CheckConditionsAsync(serviceScopeFactory, new(model.Name, model.Price), cancellationToken)))
                {
                    continue;
                }

                var infoModel = new InfoModel(model.Name, model.Price);

                // если у модели есть спец группа фильтров, то по общим ее фильтровать не надо
                var specialPriceFilters = GetSpecialFiltersForModel(model);
                if (specialPriceFilters.Any())
                {
                    if (await specialPriceFilters.TrueForAllAsync(async _ => await _.CheckConditionsAsync(serviceScopeFactory, infoModel, cancellationToken)))
                    {
                        await _logger.TraceAsync($"Ticker {model.Name} suitable for SPECIAL filters", cancellationToken: cancellationToken);
                        result.Add(infoModel);
                    }

                    continue;
                }

                if (await commonFilters.TrueForAllAsync(async _ => await _.CheckConditionsAsync(serviceScopeFactory, infoModel, cancellationToken)))
                {
                    await _logger.TraceAsync($"Ticker {model.Name} suitable for COMMON filters", cancellationToken: cancellationToken);
                    result.Add(infoModel);
                }
            }

            return result;
        }

        /// <summary>
        ///     Возвращает группы фильтров для конкретной модели
        /// </summary>
        private List<IFilterGroup> GetSpecialFiltersForModel(TradeObjectNamePriceModel model) =>
            FilterGroups.Where(_ => _.Type == FilterGroupType.Special && _.IsFilter(model.Name)).ToList();

        /// <summary>
        ///     Получение дополнительных данных и их фильтрация
        /// </summary>
        private async Task<List<InfoModel>> GetExtendedFilteredModelsAsync(
            IServiceScopeFactory serviceScopeFactory,
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

                // если у модели есть спец группа фильтров, то по общим ее фильтровать не надо
                if (specialFilters.Any())
                {
                    if (!await specialFilters.TrueForAllAsync(async _ => await _.CheckConditionsAsync(serviceScopeFactory, model, cancellationToken)))
                    {
                        await _logger
                            .TraceAsync(
                                $"Ticker {model.TradeObjectName} DOES NOT suitable for SPECIAL LATEST filters",
                                cancellationToken: cancellationToken);
                        models.Remove(model);
                        i--;
                    }

                    continue;
                }

                if (!await commonLatestFilters.TrueForAllAsync(async _ => await _.CheckConditionsAsync(serviceScopeFactory, model, cancellationToken)))
                {
                    await _logger
                        .TraceAsync(
                            $"Ticker {model.TradeObjectName} DOES NOT suitable for COMMON LATEST filters",
                            cancellationToken: cancellationToken);
                    models.Remove(model);
                    i--;
                }
            }

            return models;
        }

        /// <summary>
        ///     Возвращает группы для последней фильтрации для конкретной модели
        /// </summary>
        private List<IFilterGroup> GetSpecialLatestFiltersForModel(InfoModel model) =>
            FilterGroups
            .Where(_ => _.Type == FilterGroupType.SpecialLatest && _.IsFilter(model.TradeObjectName))
            .ToList();

        #endregion
    }
}
