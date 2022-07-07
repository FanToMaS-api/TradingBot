using Analytic.AnalyticUnits.Profiles;
using Common.Plotter;
using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует <see cref="Plotter"/>
    /// </summary>
    public class PlotterTests
    {
        /// <summary>
        ///     Тест создания изображения
        /// </summary>
        [Fact(DisplayName = "Creating image Test")]
        public void CreatingImage_Test()
        {
            var prices = File.ReadAllLines("Files/Prices.txt").Select(_ => double.Parse(_)).ToArray();
            var predictedPrices = File.ReadAllLines("Files/ExpectedPrices.txt").Select(_ => double.Parse(_)).ToArray();
            var logger = LoggerManager.CreateDefaultLogger();
            var plotter = new Plotter(logger);
            var dates = new List<DateTime>();
            var start = new DateTime(2022, 12, 12, 12, 12, 12);
            foreach (var price in prices)
            {
                dates.Add(start);
                start = start.AddSeconds(1);
            }

            var minMaxModel = MinMaxPriceModel.Create("TEST_GRAPH", predictedPrices);
            plotter.PredictedPrices = predictedPrices;
            plotter.MaxPrice = minMaxModel.MaxPrice;
            plotter.MinPrice = minMaxModel.MinPrice;
            plotter.MaxIndex = minMaxModel.MaxIndex;
            plotter.MinIndex = minMaxModel.MinIndex;
            plotter.PairName = "TEST_GRAPH";

            // Act 
            Assert.True(plotter.CanCreateChart(prices, dates, out var path));

            // проверка отмены создания графика из-за того, что он недавно был создан
            // Assert.False(ssaProfile.CanCreateChart(prices, dates, minMaxModel, out var _));
            Assert.True(File.Exists(path));
            File.Delete(path);

            // deleting a Directory
            var direcroryPath = plotter.GetOrCreateFolderPath(Plotter.GraficsFolder);
            Directory.Delete(direcroryPath);
        }
    }
}
