using Analytic.AnalyticUnits.Profiles;
using Analytic.AnalyticUnits.Profiles.ML;
using Analytic.AnalyticUnits.Profiles.ML.DataLoaders.Impl;
using Analytic.AnalyticUnits.Profiles.ML.DataLoaders.Impl.Binance;
using Analytic.AnalyticUnits.Profiles.ML.MapperProfiles;
using Analytic.AnalyticUnits.Profiles.ML.Models.Impl;
using Analytic.Models;
using AutoMapper;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDatabase.Repositories.ColdRepositories;
using BinanceDatabase.Repositories.HotRepositories;
using Common.Helpers;
using Common.Plotter;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует класс <see cref="MlAnalyticProfile"/>
    /// </summary>
    public class MlProfileTests
    {
        private readonly ILoggerDecorator _logger = LoggerManager.CreateDefaultLogger();
        private readonly IMapper _mapper = new MapperConfiguration
            (mc => mc.AddProfile(new MlMapperProfile())).CreateMapper();

        #region Test

        /// <summary>
        ///     Тест прогнозов с SSA от ML.NET
        /// </summary>
        [Fact(DisplayName = "Forecast with SSA when have 1000 Test")]
        public void ForecastWithSsa1000_Test()
        {
            var serviceScopeFactoryMock = MockHelper.CreateMockServiceScopeFactory("Files/EntitiesForTest_1000.txt");

            var contextModel = new ForecastBySsaModel(1);
            var dataLoader = new BinanceDataLoaderForSsa(_logger, _mapper);
            var models = dataLoader.GetData(serviceScopeFactoryMock, "BTCUSDT", contextModel.NumberPricesToTake);

            Assert.NotNull(models);
            Assert.Equal(1000, models.Count());
            var predictions = contextModel.Forecast(models);
            var loggerMock = Substitute.For<ILoggerDecorator>();
            var plotter = new Plotter(loggerMock);
            var doublePredictions = Array.ConvertAll(predictions, _ => (double)_);
            var minMaxPriceModel = MinMaxPriceModel.Create("BTCUSDT", doublePredictions);

            plotter.PairName = minMaxPriceModel.TradeObjectName;
            plotter.PredictedPrices = minMaxPriceModel.PredictedPrices;
            plotter.MaxPrice = minMaxPriceModel.MaxPrice;
            plotter.MinPrice = minMaxPriceModel.MinPrice;
            plotter.MaxIndex = minMaxPriceModel.MaxIndex;
            plotter.MinIndex = minMaxPriceModel.MinIndex;
            var canCreateChart = plotter.CanCreateChart(
                models.Select(_ => _.ClosePriceDouble).ToArray(),
                models.Select(_ => _.EventTime),
                out var imagePath);

            Assert.True(canCreateChart);
            Assert.True(File.Exists(imagePath));
            File.Delete(imagePath);

            // deleting a Directory
            var direcroryPath = plotter.GetOrCreateFolderPath(Plotter.GraficsFolder);
            Directory.Delete(direcroryPath);
        }

        /// <summary>
        ///     Тест прогнозов с SSA от ML.NET
        /// </summary>
        [Fact(DisplayName = "Forecast with SSA when have 2000 Test")]
        public void ForecastWithSsa2000_Test()
        {
            var serviceScopeFactoryMock = MockHelper.CreateMockServiceScopeFactory("Files/EntitiesForTest_2000.txt");

            var contextModel = new ForecastBySsaModel(1);
            var dataLoader = new BinanceDataLoaderForSsa(_logger, _mapper);
            var models = dataLoader.GetData(serviceScopeFactoryMock, "BTCUSDT", contextModel.NumberPricesToTake);

            Assert.NotNull(models);
            Assert.Equal(2000, models.Count());
            var predictions = contextModel.Forecast(models);
            var loggerMock = Substitute.For<ILoggerDecorator>();
            var plotter = new Plotter(loggerMock);
            var doublePredictions = Array.ConvertAll(predictions, _ => (double)_);

            var minMaxPriceModel = MinMaxPriceModel.Create("BTCUSDT", doublePredictions);

            plotter.PairName = minMaxPriceModel.TradeObjectName;
            plotter.PredictedPrices = minMaxPriceModel.PredictedPrices;
            plotter.MaxPrice = minMaxPriceModel.MaxPrice;
            plotter.MinPrice = minMaxPriceModel.MinPrice;
            plotter.MaxIndex = minMaxPriceModel.MaxIndex;
            plotter.MinIndex = minMaxPriceModel.MinIndex;
            var canCreateChart = plotter.CanCreateChart(
                models.Select(_ => _.ClosePriceDouble).ToArray(),
                models.Select(_ => _.EventTime),
                out var imagePath);

            Assert.True(canCreateChart);

            Assert.True(File.Exists(imagePath));
            File.Delete(imagePath);

            // deleting a Directory
            var direcroryPath = plotter.GetOrCreateFolderPath(Plotter.GraficsFolder);
            Directory.Delete(direcroryPath);
        }

        /// <summary>
        ///     Тест выполнения функции анализа с использованием SSA
        /// </summary>
        [Fact(DisplayName = "TryAnalyzeAsync function with SSA model Test")]
        public async void TryAnalyzeAsyncWithSSA_Test()
        {
            var serviceScopeFactoryMock = MockHelper.CreateMockServiceScopeFactory("Files/EntitiesForTest_2000.txt");
            var dataLoader = new BinanceDataLoaderForSsa(_logger, _mapper);
            var analyticModel = new MlAnalyticProfile(_logger, MachineLearningModelType.SSA, dataLoader, "Test");
            var infoModel = new InfoModel("BTCUSDT");

            // Act
            var (isSuccessfulAnalyze, _) = await analyticModel.TryAnalyzeAsync(
                serviceScopeFactoryMock,
                infoModel,
                CancellationToken.None);
            Assert.True(isSuccessfulAnalyze);

            var plotter = new Plotter(_logger);
            var direcroryPath = plotter.GetOrCreateFolderPath(Plotter.GraficsFolder);

            if (Directory.Exists(direcroryPath))
            {
                Directory.Delete(direcroryPath, true);
            }
        }

        /// <summary>
        ///     Тест прогнозов с SSA от ML.NET
        /// </summary>
        [Fact(DisplayName = "Forecast when have 2000 with SSA model Test")]
        public async void MlForecastWithSSA_Test()
        {
            var serviceScopeFactoryMock = MockHelper.CreateMockServiceScopeFactory("Files/EntitiesForTest_2000.txt");
            var dataLoader = new BinanceDataLoaderForSsa(_logger, _mapper);
            var analyticModel = new MlAnalyticProfile(_logger, MachineLearningModelType.SSA, dataLoader, "Test");

            // Act
            var (isSuccessfulAnalyze, _) = await analyticModel.ForecastAsync(
                serviceScopeFactoryMock,
                "NOT BTCUSDT",
                CancellationToken.None);
            Assert.True(isSuccessfulAnalyze);

            var plotter = new Plotter(_logger);
            var direcroryPath = plotter.GetOrCreateFolderPath(Plotter.GraficsFolder);

            if (Directory.Exists(direcroryPath))
            {
                Directory.Delete(direcroryPath, true);
            }
        }

        /// <summary>
        ///     Тест выполнения функции анализа с использованием FastTree
        /// </summary>
        [Fact(DisplayName = "TryAnalyzeAsync function with FastTree model Test")]
        public async void TryAnalyzeAsyncWithFastTree_Test()
        {
            var serviceScopeFactoryMock = MockHelper.CreateMockServiceScopeFactory("Files/EntitiesForTest_2000.txt");
            var dataLoader = new BinanceDataLoader(_logger, _mapper);
            var analyticModel = new MlAnalyticProfile(_logger, MachineLearningModelType.FastTree, dataLoader, "Test");
            var infoModel = new InfoModel("BTCUSDT");

            // Act
            var (isSuccessfulAnalyze, _) = await analyticModel.TryAnalyzeAsync(
                serviceScopeFactoryMock,
                infoModel,
                CancellationToken.None);
            Assert.True(isSuccessfulAnalyze);

            var plotter = new Plotter(_logger);
            var direcroryPath = plotter.GetOrCreateFolderPath(Plotter.GraficsFolder);

            if (Directory.Exists(direcroryPath))
            {
                Directory.Delete(direcroryPath, true);
            }
        }

        /// <summary>
        ///     Тест прогнозов с FastTree от ML.NET
        /// </summary>
        [Fact(DisplayName = "Forecast when have 2000 with FastTree model Test")]
        public async void MlForecastFastTree_Test()
        {
            var serviceScopeFactoryMock = MockHelper.CreateMockServiceScopeFactory("Files/EntitiesForTest_2000.txt");
            var dataLoader = new BinanceDataLoader(_logger, _mapper);
            var analyticModel = new MlAnalyticProfile(_logger, MachineLearningModelType.FastTree, dataLoader, "Test");

            // Act
            var (isSuccessfulAnalyze, _) = await analyticModel.ForecastAsync(
                serviceScopeFactoryMock,
                "NOT BTCUSDT",
                CancellationToken.None);
            Assert.True(isSuccessfulAnalyze);

            var plotter = new Plotter(_logger);
            var direcroryPath = plotter.GetOrCreateFolderPath(Plotter.GraficsFolder);

            if (Directory.Exists(direcroryPath))
            {
                Directory.Delete(direcroryPath, true);
            }
        }

        #endregion
    }
}
