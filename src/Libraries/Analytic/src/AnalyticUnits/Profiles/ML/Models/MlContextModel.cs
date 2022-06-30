using Analytic.AnalyticUnits.Profiles.ML.MapperProfiles;
using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analytic.AnalyticUnits.Profiles.ML.Models
{
    /// <summary>
    ///     Содержит основную логику по созданию и обучению моделей
    /// </summary>
    internal class MlContextModel
    {
        #region Fields

        private const int _numberPricesToTake = 4000; // кол-во данных участвующих в предсказании цены
        private const int _maxNumberPricesToForecast = 150; // максимальное кол-во данных для предсказания
        private const int _denominator = 10; // делитель кол-ва данных, участвующих в предсказании цены (выше)
                                             // для прогноза только некоторого кол-ва цен

        private readonly MLContext _context;
        private readonly IMapper _mapper;

        #endregion

        #region .ctor

        /// <summary>
        ///     Содержит основную логику по созданию и обучению моделей
        /// </summary>
        /// <param name="seed"> Для постоянных предсказаний для одинаковых данных </param>
        public MlContextModel(int? seed = null)
        {
            _context = new MLContext(seed);
            var config = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MlMapperProfile());
            });
            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Представление данных для обучения модели
        /// </summary>
        public IDataView TrainingDataView { get; private set; }

        /// <summary>
        ///     Кол-во цен для предсказания
        /// </summary>
        public int NumberPricesToForecast { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Загружает данные для указанного объекта торговли
        /// </summary>
        /// <param name="tradeObjectName">
        ///     Название объекта торговли, для которого будет проводиться прогноз
        /// </param>
        /// <returns>
        ///     Массив загруженных данных
        /// </returns>
        public IEnumerable<MiniTickerEntity> LoadData(IServiceScopeFactory serviceScopeFactory, string tradeObjectName)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var databaseFactory = scope.ServiceProvider.GetService<IBinanceDbContextFactory>()
                ?? throw new InvalidOperationException($"{nameof(IBinanceDbContextFactory)} not registered!"); ;
            using var database = databaseFactory.CreateScopeDatabase();
            var miniTickerEntities = database.ColdUnitOfWork.MiniTickers
                .GetEntities(tradeObjectName, neededCount: _numberPricesToTake);

            var models = _mapper.Map<IEnumerable<TradeObjectModel>>(miniTickerEntities);
            TrainingDataView = _context.Data.LoadFromEnumerable(models);

            miniTickerEntities.TryGetNonEnumeratedCount(out var length);
            var numberPricesToForecast = length / _denominator;
            NumberPricesToForecast = numberPricesToForecast is > 5 and < _maxNumberPricesToForecast
                ? numberPricesToForecast
                : _maxNumberPricesToForecast;

            return miniTickerEntities;
        }

        /// <summary>
        ///     Прогнозирует используя алгоритм сингулярного спектрального анализа
        /// </summary>
        public float[] ForecastWithSsa()
        {
            // Сформированный ниже конвейер возьмет _numberPricesToTake обучающих выборок
            // и разделит данные на часовые интервалы (поскольку 'seriesLength' указан как 60).
            // Каждый образец анализируется через 15 минутное окно
            var pipeline = _context.Forecasting.ForecastBySsa(
            nameof(TradeObjectForecast.Forecast),
            nameof(TradeObjectModel.ClosePrice),
            15,
            60,
            _numberPricesToTake,
            NumberPricesToForecast,
            confidenceLowerBoundColumn: "Features",
            confidenceUpperBoundColumn: "Features");

            var model = pipeline.Fit(TrainingDataView);

            var forecastingEngine = model.CreateTimeSeriesEngine<TradeObjectModel, TradeObjectForecast>(_context);

            return forecastingEngine.Predict().Forecast;
        }

        #endregion
    }
}
