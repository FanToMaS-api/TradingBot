using Analytic.AnalyticUnits.Profiles.ML.Models;
using Analytic.AnalyticUnits.Profiles.ML.Models.Impl;
using Analytic.DataLoaders;
using Analytic.Filters.Enums;
using AutoMapper;
using BinanceDatabase;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Analytic.AnalyticUnits.Profiles.ML.DataLoaders.Impl.Binance
{
    /// <summary>
    ///     Загрузчик данных для обучения моделей из базы данных
    /// </summary>
    public class BinanceDataLoader : IDataLoader
    {
        #region Fields

        private readonly ILoggerDecorator _logger;
        private readonly IMapper _mapper;
        private readonly AggregateDataIntervalType _aggregateDataInterval;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceDataLoader"/>
        public BinanceDataLoader(
            ILoggerDecorator logger,
            IMapper mapper,
            AggregateDataIntervalType aggregateDataInterval = AggregateDataIntervalType.Default)
        {
            _logger = logger;
            _mapper = mapper;
            _aggregateDataInterval = aggregateDataInterval;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IEnumerable<IObjectForMachineLearning> GetData(
            IServiceScopeFactory serviceScopeFactory,
            string tradeObjectName,
            int numberPricesToTake)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var databaseFactory = scope.ServiceProvider.GetService<IBinanceDbContextFactory>()
                ?? throw new InvalidOperationException($"{nameof(IBinanceDbContextFactory)} not registered!"); ;
            using var database = databaseFactory.CreateScopeDatabase();
            var entities = database.ColdUnitOfWork.MiniTickers
                .GetEntities(tradeObjectName, _aggregateDataInterval.CastToBinanceDataAggregateType(), numberPricesToTake);

            var models = _mapper.Map<IEnumerable<TradeObjectModel>>(entities);

            return models;
        }

        #endregion
    }
}
