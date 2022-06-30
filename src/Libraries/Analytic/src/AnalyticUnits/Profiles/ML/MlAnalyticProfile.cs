using Analytic.AnalyticUnits.Profiles.ML.Models;
using Analytic.Models;
using BinanceDatabase.Entities;
using Common.Plotter;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections.Generic;
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
        private readonly Plotter _plotter;
        private readonly MlContextModel _mlContextModel;
        private readonly bool _useSsaForecasting;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MlAnalyticProfile" />
        public MlAnalyticProfile(ILoggerDecorator logger, string name, bool useSsaForecasting = true)
        {
            Name = name;
            _logger = logger;
            _plotter = new(_logger);
            _useSsaForecasting = useSsaForecasting;
            _mlContextModel = new MlContextModel();
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
            var entities = _mlContextModel.LoadData(serviceScopeFactory, pairName);

            return !entities.Any()
                ? (false, null)
                : _useSsaForecasting
                    ? await ForecastBySsaAsync(entities, pairName, cancellationToken)
                    : (false, null);
        }

        /// <summary>
        ///     Строит прогноз с использованием алгоритма Сингулярного спектрального анализа
        /// </summary>
        /// <param name="entities"> Сущности, которые используются для обучения модели </param>
        /// <param name="pairName"> Название пары </param>
        private async Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> ForecastBySsaAsync(
            IEnumerable<MiniTickerEntity> entities,
            string pairName,
            CancellationToken cancellationToken)
        {
            var predictions = _mlContextModel.ForecastWithSsa();
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
            var canCreateChart = _plotter.CanCreateChart(
                entities.Select(_ => _.ClosePrice).ToArray(),
                entities.Select(_ => _.EventTime),
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
