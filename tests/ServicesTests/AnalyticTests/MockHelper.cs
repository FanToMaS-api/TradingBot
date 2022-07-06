using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDatabase.Repositories.ColdRepositories;
using BinanceDatabase.Repositories.HotRepositories;
using Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnalyticTests
{
    /// <summary>
    ///     Помогает с моками объектов
    /// </summary>
    internal static class MockHelper
    {
        /// <summary>
        ///     Создает мок <see cref="IServiceScopeFactory"/>
        /// </summary>
        /// <param name="pathToData"> Путь к данным для обучения модели </param>
        public static IServiceScopeFactory CreateMockServiceScopeFactory(string pathToData)
        {

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

            var hotUnitOfWorkMock = Substitute.For<IHotUnitOfWork>();
            databaseMock.HotUnitOfWork.ReturnsForAnyArgs(hotUnitOfWorkMock);

            var miniTickersRepositoryMock = Substitute.For<IMiniTickerRepository>();
            coldUnitOfWorkMock.MiniTickers.ReturnsForAnyArgs(miniTickersRepositoryMock);

            var hotMiniTickersRepositoryMock = Substitute.For<IHotMiniTickerRepository>();
            hotUnitOfWorkMock.HotMiniTickers.ReturnsForAnyArgs(hotMiniTickersRepositoryMock);
            var entities = GetMiniTickerEntities(pathToData);

            miniTickersRepositoryMock
                .GetEntities(
                    "BTCUSDT",
                    aggregateDataInterval: Arg.Any<AggregateDataIntervalType>())
                .ReturnsForAnyArgs(entities);

            var hotEntities = entities.Select(_ => new HotMiniTickerEntity
            {
                Pair = _.ShortName,
                Price = _.ClosePrice,
                ReceivedTime = _.EventTime
            });

            hotMiniTickersRepositoryMock
                .GetEntities("BTCUSDT", Arg.Any<int>())
                .ReturnsForAnyArgs(hotEntities);

            return serviceScopeFactoryMock;
        }

        /// <summary>
        ///     Получить сущности <see cref="MiniTickerEntity"/>
        /// </summary>
        /// <param name="pathToData">
        ///     Путь к файлу с данными, из которых создаются сущности
        /// </param>
        public static IEnumerable<MiniTickerEntity> GetMiniTickerEntities(string pathToData)
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

            return entities;
        }
    }
}
