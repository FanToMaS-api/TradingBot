using Analytic.AnalyticUnits.Profiles.ML.Models.ArgumentsCreators;
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
            var argumentsCreator = new SsaArgumentsCreator(data);
            var numberPricesToForecast = argumentsCreator.GetNumberPricesToForecast();
            var windowSize = argumentsCreator.GetWindowSize();
            var seriesLength = argumentsCreator.GetSeriesLength();

            // Сформированный ниже конвейер возьмет _numberPricesToTake обучающих выборок
            // и разделит данные на часовые интервалы (поскольку 'seriesLength' указан как 60).
            // Каждый образец анализируется через windowSize окно
            var pipeline = _context.Forecasting.ForecastBySsa(
            nameof(TradeObjectForecast.Forecast),
            nameof(TradeObjectModel.ClosePrice),
            windowSize,
            seriesLength,
            NumberPricesToTake,
            numberPricesToForecast,
            confidenceLowerBoundColumn: "Features",
            confidenceUpperBoundColumn: "Features");

            var model = pipeline.Fit(trainingDataView);

            var forecastingEngine = model.CreateTimeSeriesEngine<TradeObjectModel, TradeObjectForecast>(_context);

            return forecastingEngine.Predict().Forecast;
        }

        #endregion
    }
}
