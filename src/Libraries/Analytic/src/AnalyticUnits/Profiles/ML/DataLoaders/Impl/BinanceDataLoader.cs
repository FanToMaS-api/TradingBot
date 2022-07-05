using Analytic.AnalyticUnits.Profiles.ML.Models;
using Analytic.AnalyticUnits.Profiles.ML.Models.Impl;
using Analytic.DataLoaders;
using AutoMapper;
using BinanceDatabase;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Analytic.AnalyticUnits.Profiles.ML.DataLoaders.Impl
{
    /// <summary>
    ///     Загрузчик данных для ML с из базы данных
    /// </summary>
    internal class BinanceDataLoader : IDataLoader
    {
        #region Fields

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILoggerDecorator _logger;
        private readonly IMapper _mapper;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceDataLoader"/>
        public BinanceDataLoader(IServiceScopeFactory serviceScopeFactory, ILoggerDecorator logger, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _mapper = mapper;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IEnumerable<IObjectForMachineLearning> GetDataForSsa(
            string tradeObjectName,
            int numberPricesToTake)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var databaseFactory = scope.ServiceProvider.GetService<IBinanceDbContextFactory>()
                ?? throw new InvalidOperationException($"{nameof(IBinanceDbContextFactory)} not registered!"); ;
            using var database = databaseFactory.CreateScopeDatabase();
            var miniTickerEntities = database.ColdUnitOfWork.MiniTickers
                .GetEntities(tradeObjectName, neededCount: numberPricesToTake);

            var models = _mapper.Map<IEnumerable<TradeObjectModel>>(miniTickerEntities);

            return models;
        }

        #endregion
    }
}
