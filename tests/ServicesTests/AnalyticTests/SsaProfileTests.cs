using Analytic.AnalyticUnits.Profiles.SSA;
using Analytic.AnalyticUnits.Profiles.SSA.Models;
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
        ///     Тест восстановления сигнала
        /// </summary>
        [Fact(DisplayName = "Signal recovery Test")]
        public void SignalRecovery_Test()
        {
            var prices = File.ReadAllLines("Files/Prices.txt").Select(_ => double.Parse(_)).ToArray();
            var ssaModel = SsaModel.Create(prices);
            var subMatrixEigenvectors = SsaAnalyticPofile.MakeSmoothing(ssaModel);
            var (matrixEigenvectors, matrixEigenvalues) = ssaModel.GetEigenVectorsAndValues();

            // Восстанавливаю сигнал
            var mainComponents = ssaModel.MainComponentsMatrix;
            var restoredOriginalMatrix = (matrixEigenvectors * mainComponents).ToArray();
            var restoredSignal = SsaAnalyticPofile.SignalRecovery(restoredOriginalMatrix, prices.Length, ssaModel.TauDelayNumber);

            var expectedRestoredSignal = File.ReadAllLines("Files/ExpectedRestoredSignal.txt").Select(_ => double.Parse(_)).ToArray();
            Assert.Equal(expectedRestoredSignal.Length, restoredSignal.Length);
            for (var i = 0; i < expectedRestoredSignal.Length; i++)
            {
                Assert.Equal(expectedRestoredSignal[i], restoredSignal[i]);
            }
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
            var ssaProfile = new SsaAnalyticPofile(logger, "Test Analytic Profile");
            var dates = new List<DateTime>();
            var start = new DateTime(2022, 12, 12, 12, 12, 12);
            foreach (var price in prices)
            {
                dates.Add(start);
                start = start.AddSeconds(1);
            }

            var minMaxModel = MinMaxPriceModel.Create(predictedPrices);
            Assert.True(ssaProfile.CanCreateChart(prices, predictedPrices, dates, "TEST_PAIR", minMaxModel, out var path));

            // проверка выдачи ошибки того, что график был недавно создан
            Assert.False(ssaProfile.CanCreateChart(prices, predictedPrices, dates, "TEST_PAIR", minMaxModel, out var _));
            Assert.True(File.Exists(path));
            File.Delete(path);

            // deleting a Directory
            var direcroryPath = ssaProfile.GetOrCreateFolderPath(SsaAnalyticPofile.GraficsFolder);
            Directory.Delete(direcroryPath);
        }
    }
}
