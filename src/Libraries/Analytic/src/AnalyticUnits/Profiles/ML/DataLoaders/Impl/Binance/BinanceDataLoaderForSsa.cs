using Analytic.AnalyticUnits.Profiles.ML.Models;
using Analytic.AnalyticUnits.Profiles.ML.Models.Impl;
using Analytic.DataLoaders;
using AutoMapper;
using BinanceDatabase;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Analytic.AnalyticUnits.Profiles.ML.DataLoaders.Impl.Binance
{
    /// <summary>
    ///     Загрузчик данных для SSA с из базы данных
    /// </summary>
    public class BinanceDataLoaderForSsa : IDataLoader
    {
        #region Fields

        private readonly ILoggerDecorator _logger;
        private readonly IMapper _mapper;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceDataLoaderForSsa"/>
        public BinanceDataLoaderForSsa(ILoggerDecorator logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
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
            var miniTickerEntities = database.HotUnitOfWork.HotMiniTickers
                .GetEntities(tradeObjectName, neededCount: numberPricesToTake);

            var models = _mapper.Map<IEnumerable<HotTradeObjectModel>>(miniTickerEntities);

            return models;
        }

        #endregion
    }
}
