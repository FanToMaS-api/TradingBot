using Analytic.AnalyticUnits.Profiles.ML.Models.ArgumentsCreators;
using Logger;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analytic.AnalyticUnits.Profiles.ML.Models.Impl
{
    /// <summary>
    ///     Содержит основную логику для прогноза с помощью FastTree
    /// </summary>
    internal class ForecastByFastTreeModel : IMachineLearningModel
    {
        #region Fields

        private readonly ILoggerDecorator _logger;
        private readonly MLContext _context;

        #endregion

        #region .ctor

        /// <summary>
        ///     Содержит основную логику для прогноза с помощью FastTree
        /// </summary>
        /// <param name="seed"> Для постоянных прогнозов </param>
        public ForecastByFastTreeModel(ILoggerDecorator logger, int? seed = null)
        {
            _logger = logger;
            _context = new MLContext(seed);
            NumberPricesToTake = 4000;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Кол-во данных участвующих в предсказании цены
        /// </summary>
        public int NumberPricesToTake { get; init; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public float[] Forecast(IEnumerable<IObjectForMachineLearning> data)
        {
            if (!data.Any())
            {
                throw new ArgumentException($"{nameof(data)} are empty!");
            }

            var trainingDataView = _context.Data.LoadFromEnumerable(data.Cast<TradeObjectModel>());
            var trainTestData = _context.Data.TrainTestSplit(trainingDataView, 0.2);
            var trainData = trainTestData.TrainSet;
            var pipeline = CreatePipeline(trainingDataView);

            // Обучение
            var model = pipeline.Fit(trainData);

            var testData = trainTestData.TestSet;

            // Тестирование
            var predictions = model.Transform(testData);
            var metrics = _context.Regression.Evaluate(predictions, nameof(TradeObjectModel.ClosePrice));
            if (metrics.RSquared < 0.8)
            {
                _logger.InfoAsync("FastTree regression gave poor performance");
                return Array.Empty<float>();
            }

            var dataForPredict = _context.Data
                .LoadFromEnumerable(data.TakeLast(150)
                .Cast<TradeObjectModel>());
            var f = model.Transform(dataForPredict);

            return f.GetColumn<float>("Score").ToArray();
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Создает пайплайн
        /// </summary>
        private EstimatorChain<RegressionPredictionTransformer<FastTreeRegressionModelParameters>> CreatePipeline(
            IDataView trainingDataView)
        {
            var featureNames = GetFeatureNames(trainingDataView);
            var trainer = _context.Regression.Trainers.FastTree(nameof(TradeObjectModel.ClosePrice));
            var pipeline = _context.Transforms
                .Concatenate("Features", featureNames)
                .Append(trainer);

            return pipeline;
        }

        /// <summary>
        ///     Возвращает массив фич
        /// </summary>
        internal static string[] GetFeatureNames(IDataView trainingDataView)
        {
            var excludedFeatures = new HashSet<string>(
                new[]
                {
                    nameof(TradeObjectModel.ClosePriceDouble),
                    nameof(TradeObjectModel.EventTime),
                });

            return trainingDataView.Schema
                .Select(x => x.Name)
                .Where(x => !excludedFeatures.Contains(x))
                .ToArray();
        }

        #endregion
    }
}
