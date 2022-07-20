using Analytic.AnalyticUnits.Profiles.ML.Models.ArgumentsCreators;
using Common.Helpers;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analytic.AnalyticUnits.Profiles.ML.Models.Impl
{
    /// <summary>
    ///     Содержит основную логику для прогноза с помощью SSA
    /// </summary>
    internal class ForecastBySsaModel : IMachineLearningModel
    {
        #region Fields

        private readonly MLContext _context;

        #endregion

        #region .ctor

        /// <summary>
        ///     Содержит основную логику для прогноза с помощью SSA
        /// </summary>
        /// <param name="seed"> Для постоянных прогнозов </param>
        public ForecastBySsaModel(int? seed = null)
        {
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

            var  trainingDataView = _context.Data.LoadFromEnumerable(data);
            var argumentsCreator = new ArgumentsCreator(data);
            var numberPricesToForecast = argumentsCreator.GetNumberPricesToForecast();
            Assert.True(numberPricesToForecast > 0);

            var windowSize = argumentsCreator.GetWindowSize();
            Assert.True(windowSize > 0);

            var seriesLength = argumentsCreator.GetSeriesLength();
            Assert.True(seriesLength > 0);

            // Сформированный ниже конвейер возьмет _numberPricesToTake обучающих выборок
            // и разделит данные на часовые интервалы ('seriesLength').
            // Каждый образец анализируется через windowSize окно
            var pipeline = _context.Forecasting.ForecastBySsa(
            nameof(SsaForecast.Forecast),
            nameof(IObjectForMachineLearning.ClosePrice),
            windowSize,
            seriesLength,
            NumberPricesToTake,
            numberPricesToForecast,
            confidenceLowerBoundColumn: "Features",
            confidenceUpperBoundColumn: "Features");

            var model = pipeline.Fit(trainingDataView);

            var forecastingEngine = model.CreateTimeSeriesEngine<HotTradeObjectModel, SsaForecast>(_context);

            return forecastingEngine.Predict().Forecast;
        }

        #endregion
    }
}
