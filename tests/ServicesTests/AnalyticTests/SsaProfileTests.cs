using Analytic.AnalyticUnits.Profiles.SSA;
using Analytic.AnalyticUnits.Profiles.SSA.Models;
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
    ///     Тестирует <see cref="SsaAnalyticPofile"/>
    /// </summary>
    public class SsaProfileTests
    {
        /// <summary>
        ///     Тест SSA
        /// </summary>
        [Fact(DisplayName = "SSA Test")]
        public void Ssa_Test()
        {
            var prices = File.ReadAllLines("Files/Prices.txt").Select(_ => double.Parse(_)).ToArray();
            var predictedPrices = SsaAnalyticPofile.SSA(prices);
            var expectedPredictions = File.ReadAllLines("Files/ExpectedPrices.txt").Select(_ => double.Parse(_)).ToArray();
            Assert.Equal(expectedPredictions.Length, predictedPrices.Length);
            for (var i = 0; i < expectedPredictions.Length; i++)
            {
                Assert.Equal(expectedPredictions[i], predictedPrices[i]);
            }
        }

        /// <summary>
        ///     Тест SSA с пустым массивом цен
        /// </summary>
        [Fact(DisplayName = "SSA with empty prices array Test")]
        public void SsaEmptyPricesArray_Test()
        {
            var prices = Array.Empty<double>();
            var predictedPrices = SsaAnalyticPofile.SSA(prices);
            Assert.Empty(predictedPrices);
        }

        /// <summary>
        ///     Тест восстановления сигнала
        /// </summary>
        [Fact(DisplayName = "Signal recovery Test")]
        public void SignalRecovery_Test()
        {
            var prices = File.ReadAllLines("Files/Prices.txt").Select(_ => double.Parse(_)).ToArray();
            var ssaModel = SsaModel.Create(prices);
            ssaModel.ComputeCovariationMatrix();
            ssaModel.ComputeEigenVectorsAndValues();
            ssaModel.ComputeNeededLambdas();

            var subMatrixEigenvectors = ssaModel.GetSubListEigenvectors();
            ssaModel.ComputeMainComponentsMatrix();
            ssaModel.ComputeRestoredOriginalMatrix();
            
            // Act
            var restoredSignal = ssaModel.GetRestoredSignal();

            var expectedRestoredSignal = File.ReadAllLines("Files/ExpectedRestoredSignal.txt").Select(_ => double.Parse(_)).ToArray();
            Assert.Equal(expectedRestoredSignal.Length, restoredSignal.Length);
            for (var i = 0; i < expectedRestoredSignal.Length; i++)
            {
                Assert.Equal(expectedRestoredSignal[i], restoredSignal[i]);
            }
        }

        /// <summary>
        ///     Тест восстановления сигнала при пустой исходной восстановленной матрице
        /// </summary>
        [Fact(DisplayName = "Signal recovery with empty restored original matrix Test")]
        public void SignalRecoveryWithEmptyRestoredOriginalMatrix_Test()
        {
            var prices = File.ReadAllLines("Files/Prices.txt").Select(_ => double.Parse(_)).ToArray();
            var ssaModel = SsaModel.Create(prices);

            // Восстанавливаю сигнал
            var restoredOriginalMatrix = new double[0, 0];

            // Act
            var restoredSignal = ssaModel.GetRestoredSignal();
            Assert.Empty(restoredOriginalMatrix);
        }

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

            var minMaxModel = MinMaxPriceModel.Create(predictedPrices);
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
