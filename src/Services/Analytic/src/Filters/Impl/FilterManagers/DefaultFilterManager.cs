using Analytic.Models;
using Common.Models;
using ExchangeLibrary;
using NLog;
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

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, InfoModel> _infoModels = new();

        #endregion

        #region Public methods

        /// <inheritdoc />
        public override async Task<IEnumerable<InfoModel>> GetFilteredDataAsync<T>(
            IExchange exchange,
            IEnumerable<T> modelsToFilter,
            CancellationToken cancellationToken)
        {
            var filteredModels = GetFilteredData(modelsToFilter as IEnumerable<TradeObjectNamePriceModel>);
            return await GetExtendedFilteredModelsAsync(exchange, filteredModels.Take(2).ToList(), cancellationToken);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Первоначальная фильтрация данных
        /// </summary>
        private List<InfoModel> GetFilteredData(IEnumerable<TradeObjectNamePriceModel> models)
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

                if (!_infoModels.ContainsKey(model.Name))
                {
                    _infoModels[model.Name] = new(model.Name, model.Price);

                    continue;
                }

                var infoModel = _infoModels[model.Name];
                CalculateData(infoModel, model);

                // если у модели есть спец группа фильтров, то по общим ее фильтровать не надо
                var specialPriceFilters = GetSpecialFiltersForModel(model);
                if (specialPriceFilters.Any())
                {
                    if (specialPriceFilters.All(_ => _.CheckConditions(infoModel)))
                    {
                        Log.Trace($"Ticker {model.Name} suitable for SPECIAL filters");
                        result.Add(infoModel);
                    }

                    continue;
                }

                if (commonFilters.All(_ => _.CheckConditions(infoModel)))
                {
                    Log.Trace($"Ticker {model.Name} suitable for COMMON filters");
                    result.Add(infoModel);
                }
            }

            return result;
        }

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
                        Log.Trace($"Ticker {model.TradeObjectName} does not suitable for SPECIAL LATEST filters");
                        models.Remove(model);
                        i--;
                    }

                    continue;
                }

                if (!commonLatestFilters.All(_ => _.CheckConditions(model)))
                {
                    Log.Trace($"Ticker {model.TradeObjectName} does not suitable for COMMON LATEST filters");
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
