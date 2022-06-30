using Analytic.AnalyticUnits.Profiles;
using Analytic.AnalyticUnits.Profiles.ML;
using Analytic.AnalyticUnits.Profiles.ML.Models;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDatabase.Repositories.ColdRepositories;
using Common.Helpers;
using Common.Plotter;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует класс <see cref="MlAnalyticProfile"/>
    /// </summary>
    public class MlProfileTests
    {
        #region Test

        /// <summary>
        ///     Тест прогнозов с SSA от ML.NET
        /// </summary>
        [Fact(DisplayName = "Forecast with SSA when have 1000 Test")]
        public void ForecastWithSsa1000_Test()
        {
            var serviceScopeFactoryMock = CreateMockServiceScopeFactory("Files/EntitiesForTest_1000.txt");

            var contextModel = new MlContextModel(1);
            var entities = contextModel.LoadData(serviceScopeFactoryMock, "BTCUSDT");
            var predictions = contextModel.ForecastWithSsa();
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
                entities.Select(_ => _.ClosePrice).ToArray(),
                entities.Select(_ => _.EventTime),
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
            var serviceScopeFactoryMock = CreateMockServiceScopeFactory("Files/EntitiesForTest_2000.txt");

            var contextModel = new MlContextModel(1);
            var entities = contextModel.LoadData(serviceScopeFactoryMock, "BTCUSDT");
            var predictions = contextModel.ForecastWithSsa();
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
                entities.Select(_ => _.ClosePrice).ToArray(),
                entities.Select(_ => _.EventTime),
                out var imagePath);

            Assert.True(canCreateChart);

            Assert.True(File.Exists(imagePath));
            File.Delete(imagePath);

            // deleting a Directory
            var direcroryPath = plotter.GetOrCreateFolderPath(Plotter.GraficsFolder);
            Directory.Delete(direcroryPath);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Создает мок <see cref="IServiceScopeFactory"/>
        /// </summary>
        /// <param name="pathToData"> Путь к данным для обучения модели </param>
        public static IServiceScopeFactory CreateMockServiceScopeFactory(string pathToData)
        {
            var fileContent = File.ReadAllLines(pathToData).Skip(1);
            var entities = new List<MiniTickerEntity>();
            foreach (var line in fileContent)
            {
                var properties = line.Split("  ");
                var newEntity = new MiniTickerEntity
                {
                    AggregateDataInterval = AggregateDataIntervalType.OneMinute,
                    OpenPrice = double.Parse(properties[0]),
                    ClosePrice = double.Parse(properties[1]),
                    MinPrice = double.Parse(properties[2]),
                    MaxPrice = double.Parse(properties[3]),
                    BasePurchaseVolume = double.Parse(properties[4]),
                    QuotePurchaseVolume = double.Parse(properties[5]),
                    EventTime = DateTime.Parse(properties[6]),
                    ShortName = "BTCUSDT"
                };

                newEntity.PriceDeviationPercent = CommonHelper.GetPercentDeviation(
                    newEntity.OpenPrice,
                    newEntity.ClosePrice);
                entities.Add(newEntity);
            }

            var serviceScopeFactoryMock = Substitute.For<IServiceScopeFactory>();
            var serviceScopeMock = Substitute.For<IServiceScope>();
            serviceScopeFactoryMock.CreateScope().ReturnsForAnyArgs(serviceScopeMock);

            var serviceProviderMock = Substitute.For<IServiceProvider>();
            serviceScopeMock.ServiceProvider.ReturnsForAnyArgs(serviceProviderMock);

            var databaseFactoryMock = Substitute.For<IBinanceDbContextFactory>();
            serviceProviderMock.GetService<IBinanceDbContextFactory>().ReturnsForAnyArgs(databaseFactoryMock);

            var databaseMock = Substitute.For<IUnitOfWork>();
            databaseFactoryMock.CreateScopeDatabase().ReturnsForAnyArgs(databaseMock);

            var coldUnitOfWorkMock = Substitute.For<IColdUnitOfWork>();
            databaseMock.ColdUnitOfWork.ReturnsForAnyArgs(coldUnitOfWorkMock);

            var miniTickersRepositoryMock = Substitute.For<IMiniTickerRepository>();
            coldUnitOfWorkMock.MiniTickers.ReturnsForAnyArgs(miniTickersRepositoryMock);

            miniTickersRepositoryMock
                .GetEntities(
                    Arg.Any<string>(),
                    aggregateDataInterval: Arg.Any<AggregateDataIntervalType>())
                .ReturnsForAnyArgs(entities);

            return serviceScopeFactoryMock;
        }

        #endregion
    }
}
