using Analytic.AnalyticUnits.Profiles.ML.Models;
using Analytic.AnalyticUnits.Profiles.ML.Models.Impl;
using Analytic.DataLoaders;
using Analytic.Models;
using AutoMapper;
using Common.Plotter;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits.Profiles.ML
{
    /// <summary>
    ///     Профиль аналитики, использующий машинное обучение
    /// </summary>
    public class MlAnalyticProfile : IAnalyticProfile
    {
        #region Fields

        private readonly ILoggerDecorator _logger;
        private readonly IMachineLearningModel _mlContextModel;
        private readonly IDataLoader _dataLoader;
        private readonly Plotter _plotter;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MlAnalyticProfile" />
        public MlAnalyticProfile(
            ILoggerDecorator logger,
            MachineLearningModelType learningModelType,
            IDataLoader dataLoader,
            string name)
        {
            Name = name;
            _logger = logger;
            _plotter = new(_logger);
            _dataLoader = dataLoader;
            _mlContextModel = learningModelType switch
            {
                MachineLearningModelType.SSA => new ForecastBySsaModel(),
                _ => throw new Exception($"Unknown type of {nameof(MachineLearningModelType)} = '{learningModelType}'")
            };
        }

        #endregion

        #region Implementation of IAnalyticProfile

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public async Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IServiceScopeFactory serviceScopeFactory,
            InfoModel model,
            CancellationToken cancellationToken)
        {
            var pairName = model.TradeObjectName;

            return await ForecastAsync(serviceScopeFactory, pairName, cancellationToken);
        }

        /// <summary>
        ///     Строит прогноз
        /// </summary>
        /// <param name="pairName"> Название пары </param>
        internal async Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> ForecastAsync(
            IServiceScopeFactory serviceScopeFactory,
            string pairName,
            CancellationToken cancellationToken)
        {
            var models = _dataLoader.GetData(serviceScopeFactory, pairName, _mlContextModel.NumberPricesToTake);
            var predictions = _mlContextModel.Forecast(models);
            if (!predictions.Any())
            {
                return (false, null);
            }

            await _logger.TraceAsync(
                $"Successful predicted {predictions.Length} prices for {pairName}",
                cancellationToken: cancellationToken);

            var doublePredictions = Array.ConvertAll(predictions, _ => (double)_);
            var minMaxPriceModel = MinMaxPriceModel.Create(pairName, doublePredictions);
            if (minMaxPriceModel.MinPrice <= 0)
            {
                return (false, null);
            }

            MapMinMaxModelToPlotter(minMaxPriceModel);
            var doublePrices = Array.ConvertAll(predictions, _ => (double)_);
            var canCreateChart = _plotter.CanCreateChart(
                models.Select(_ => _.ClosePriceDouble).ToArray(),
                models.Select(_ => _.EventTime),
                out var imagePath);

            var result = new AnalyticResultModel()
            {
                TradeObjectName = pairName,
                HasPredictionImage = canCreateChart,
                RecommendedPurchasePrice = minMaxPriceModel.MinPrice,
                RecommendedSellingPrice = minMaxPriceModel.MaxPrice,
                ImagePath = imagePath
            };

            return (true, result);
        }

        /// <summary>
        ///     Маппит значения модели максимальной и минимальной цены в модель для построения графика
        /// </summary>
        /// <param name="minMaxPriceModel"> Модели максимальной и минимальной цены </param>
        private void MapMinMaxModelToPlotter(MinMaxPriceModel minMaxPriceModel)
        {
            _plotter.PairName = minMaxPriceModel.TradeObjectName;
            _plotter.PredictedPrices = minMaxPriceModel.PredictedPrices;
            _plotter.MaxPrice = minMaxPriceModel.MaxPrice;
            _plotter.MinPrice = minMaxPriceModel.MinPrice;
            _plotter.MaxIndex = minMaxPriceModel.MaxIndex;
            _plotter.MinIndex = minMaxPriceModel.MinIndex;
        }

        #endregion
    }
}
