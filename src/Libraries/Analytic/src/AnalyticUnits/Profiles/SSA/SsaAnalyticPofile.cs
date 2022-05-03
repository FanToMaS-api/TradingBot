﻿using Analytic.AnalyticUnits.Profiles.SSA.Models;
using Analytic.Models;
using BinanceDatabase;
using BinanceDatabase.Entities;
using Common.Plotter;
using Logger;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits.Profiles.SSA
{
    /// <summary>
    ///     Использует алгоритм сингулярного спектрального анализа для предсказания цены
    /// </summary>
    /// <remarks>
    ///     https://ru.wikipedia.org/wiki/SSA_(%D0%BC%D0%B5%D1%82%D0%BE%D0%B4)
    /// </remarks>
    public class SsaAnalyticPofile : IAnalyticProfile
    {
        #region Fields

        private readonly ILoggerDecorator _logger;
        private readonly Plotter _plotter;
        private const int _numberPricesToTake = 900; // кол-во данных участвующих в предсказании цены
        private const int _denominator = 5; // делитель кол-ва данных, участвующих в предсказании цены (выше)
                                            // для прогноза только некоторого кол-ва цен

        private const int _neededLambdaNumber = 7; // Кол-во значимых собственных значений, которое будет отбираться
        private const double _minMagnitudeForSsaSmoothing = 0.0002; // Минимальная магнитуда
                                                                    // для собственного значения при сглаживании

        #endregion

        #region .ctor

        /// <inheritdoc cref="SsaAnalyticPofile"/>
        public SsaAnalyticPofile(ILoggerDecorator logger, string name)
        {
            Name = name;
            _logger = logger;
            _plotter = new(_logger);
            _logger.TraceAsync($"Directory with saved grafics='{AppDomain.CurrentDomain.BaseDirectory}'").Wait(5 * 1000);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IServiceScopeFactory serviceScopeFactory,
            InfoModel model,
            CancellationToken cancellationToken)
        {
            var pairName = model.TradeObjectName;
            var enities = GetHotMiniTickers(serviceScopeFactory, pairName);
            if (!enities.Any())
            {
                return (false, null);
            }

            var prices = enities.Select(_ => _.Price).ToArray();
            var predictions = SSA(prices);
            if (!predictions.Any())
            {
                return (false, null);
            }

            await _logger.TraceAsync(
                $"Successful predicted {predictions.Length} prices for {pairName}",
                cancellationToken: cancellationToken);
            await SavePredictionAsync(serviceScopeFactory, pairName, enities.Last().ReceivedTime, predictions, cancellationToken);

            var minMaxPriceModel = MinMaxPriceModel.Create(predictions);
            if (minMaxPriceModel.MinPrice <= 0)
            {
                return (false, null);
            }

            _plotter.PairName = pairName;
            _plotter.PredictedPrices = predictions;
            _plotter.MaxPrice = minMaxPriceModel.MaxPrice;
            _plotter.MinPrice = minMaxPriceModel.MinPrice;
            _plotter.MaxIndex = minMaxPriceModel.MaxIndex;
            _plotter.MinIndex = minMaxPriceModel.MinIndex;
            var canCreateChart = _plotter.CanCreateChart(
                prices,
                enities.Select(_ => _.ReceivedTime),
                out var imagePath);

            var result = new AnalyticResultModel()
            {
                TradeObjectName = pairName,
                HasPredictionImage = canCreateChart,
                RecommendedPurchasePrice = minMaxPriceModel.MinPrice,
                RecommendedSellingPrice = minMaxPriceModel.MaxPrice,
                ImagePath = imagePath
            };

            return (true, result);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Возвращает самые актуальные сущности из базы для конкретной пары
        /// </summary>
        private static HotMiniTickerEntity[] GetHotMiniTickers(IServiceScopeFactory serviceScopeFactory, string pairName)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var databaseFactory = scope.ServiceProvider.GetRequiredService<IBinanceDbContextFactory>();
            using var database = databaseFactory.CreateScopeDatabase();

            return database.HotUnitOfWork.HotMiniTickers.GetArray(pairName, _numberPricesToTake);
        }

        /// <summary>
        ///     Сингулярный спектральный анализ
        /// </summary>
        /// <returns>
        ///     Возвращает массив предсказанных данных
        /// </returns>
        /// <remarks>
        ///     используется для определения основных составляющих временного ряда 
        ///     и подавления шума
        /// </remarks>
        internal static double[] SSA(double[] prices)
        {
            if (prices.Length == 0)
            {
                return Array.Empty<double>();
            }

            var ssaModel = SsaModel.Create(prices);
            var subMatrixEigenvectors = MakeSmoothing(ssaModel);
            if (!subMatrixEigenvectors.Any())
            {
                return Array.Empty<double>();
            }

            // Восстанавливаю сигнал
            // var (matrixEigenvectors, _) = ssaModel.GetEigenVectorsAndValues();
            // var mainComponents = ssaModel.MainComponentsMatrix;
            // var restoredOriginalMatrix = (matrixEigenvectors * mainComponents).ToArray();
            // var restoredSignal = SignalRecovery(restoredOriginalMatrix, prices.Length, ssaModel.TauDelayNumber);

            var (Vstar, tauMatrixEigenvector) = GetPreparedMatrixForForecasting(
                subMatrixEigenvectors,
                ssaModel.TauDelayNumber);

            // предсказываем только четвертую часть от исходных данных
            var predictions = ForecastingSignal(
                Vstar,
                tauMatrixEigenvector,
                prices,
                ssaModel.TauDelayNumber,
                prices.Length / _denominator);

            return predictions;
        }

        /// <summary>
        ///     Производит SSA-сглаживание
        /// </summary>
        internal static List<Vector<double>> MakeSmoothing(SsaModel ssaModel)
        {
            var (matrixEigenvectors, matrixEigenvalues) = ssaModel.GetEigenVectorsAndValues();
            var eigenvaluesArray = matrixEigenvalues.ToList();
            eigenvaluesArray.Sort((x, y) =>
            {
                return x.Magnitude < y.Magnitude ? 1 : x.Magnitude == y.Magnitude ? 0 : -1;
            });

            var neededLambdas = eigenvaluesArray.Where(_ => _.Magnitude > _minMagnitudeForSsaSmoothing);
            neededLambdas = neededLambdas.Count() < _neededLambdaNumber ? eigenvaluesArray.Take(_neededLambdaNumber) : neededLambdas;
            var subMatrixEigenvectors = GetSubListEigenvectors(ssaModel, neededLambdas);

            return subMatrixEigenvectors;
        }

        /// <summary>
        ///     Возвращает список значимых векторов, для предсказаний <br/>
        ///     Зануляем столбцы собственных векторов, которые незначимы
        /// </summary>
        internal static List<Vector<double>> GetSubListEigenvectors(
            SsaModel model,
            IEnumerable<System.Numerics.Complex> neededLambdas)
        {
            var subEigenvectors = new List<Vector<double>>(); // понадобится для предсказаний
            var (matrixEigenvectors, matrixEigenvalues) = model.GetEigenVectorsAndValues();
            for (var i = 0; i < matrixEigenvalues.Count; i++)
            {
                if (neededLambdas.Contains(matrixEigenvalues[i]))
                {
                    subEigenvectors.Add(matrixEigenvectors.Column(i));
                    continue;
                }

                // зануляем столбцы собственных векторов, которые незначимы (сглаживаем)
                matrixEigenvectors.ClearColumn(i);
            }

            return subEigenvectors;
        }

        /// <summary>
        ///     Подготавливает матрицы для предсказания по алгоритму
        /// </summary>
        /// <param name="subEigenvectors"> Отобранные собственные вектора </param>
        /// <param name="tau"> Кол-во задержек </param>
        /// <returns>
        ///     <see langword="Vstar"/> Матрица значимых собственных векторов без последней строки
        ///     <see langword="tauMatrixEigenvector"/> Тау-вектор (последняя строка <paramref name="subEigenvectors"/>) <br/>
        /// </returns>
        private static (Matrix<double> Vstar, Matrix<double> tauMatrixEigenvector) GetPreparedMatrixForForecasting(
            List<Vector<double>> subEigenvectors,
            int tau)
        {
            var tauEigenvector = new List<double[]>
            {
                new double[subEigenvectors.Count]
            };

            var Vstar = Matrix<double>.Build.Dense(tau - 1, subEigenvectors.Count);
            for (var j = 0; j < subEigenvectors.Count; j++)
            {
                tauEigenvector[0][j] = subEigenvectors[j].Last();
                for (var i = 0; i < tau - 1; i++)
                {
                    Vstar[i, j] = subEigenvectors[j][i];
                }
            }

            var tauMatrixEigenvector = Matrix<double>.Build.DenseOfRowArrays(tauEigenvector);

            return (Vstar, tauMatrixEigenvector);
        }

        /// <summary>
        ///     Предсказывает сигнал
        /// </summary>
        /// <param name="Vstar"> Матрица значимых собственных векторов без последней строки </param>
        /// <param name="tauMatrixEigenvector"> Тау-вектор (изъятая строка из <paramref name="Vstar"/>) </param>
        /// <param name="array"> Исходыный массив </param>
        /// <param name="tau_delayNumber"> Кол-во задержек </param>
        /// <param name="neededForecastingCount"> Кол-во предсказаний </param>
        private static double[] ForecastingSignal(
            Matrix<double> Vstar,
            Matrix<double> tauMatrixEigenvector,
            double[] array,
            int tau_delayNumber,
            int neededForecastingCount)
        {
            var list = array.ToList();
            var predictions = new List<double>();

            for (var k = 0; k < neededForecastingCount; k++)
            {
                var Q = Matrix<double>.Build.Dense(tau_delayNumber - 1, 1);
                var index = 0;
                for (var i = list.Count - tau_delayNumber + 1; i < list.Count && index < tau_delayNumber - 1; i++, index++)
                {
                    Q[index, 0] = list[i];
                }

                var numerator = (tauMatrixEigenvector * Vstar.Transpose() * Q)[0, 0];
                var denominator = (1 - tauMatrixEigenvector * tauMatrixEigenvector.Transpose())[0, 0];
                var newX = numerator / denominator;

                list.Add(newX);
                predictions.Add(newX);
            }

            return predictions.ToArray();
        }

        /// <summary>
        ///     Сохраняет предсказанные значения в бд
        /// </summary>
        private async Task SavePredictionAsync(
            IServiceScopeFactory serviceScopeFactory,
            string tradeObjectName,
            DateTime predictionTime,
            double[] predictions,
            CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var databaseFactory = scope.ServiceProvider.GetRequiredService<IBinanceDbContextFactory>();
            using var database = databaseFactory.CreateScopeDatabase();
            var predictionEntity = new PredictionEntity
            {
                ShortName = tradeObjectName,
                PredictionTime = predictionTime,
                PriceValues = predictions,
            };

            try
            {
                await database.ColdUnitOfWork.Predictions.AddAsync(predictionEntity, cancellationToken);

                await database.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex, "Failed to save predicted data", cancellationToken: cancellationToken);
            }
        }

        #region Signal recovery

        /// <summary>
        ///     Восстанавливает сигнал
        /// </summary>
        /// <param name="newX"> Восстановленная исходная матрица </param>
        /// <param name="pricesLength"> Кол-во значений в первоначальном векторе-сигнале </param>
        /// <param name="tau_delayNumber"> Число задержек </param>
        internal static double[] SignalRecovery(double[,] newX, int pricesLength, int tau_delayNumber)
        {
            if (newX is null || newX.Length == 0)
            {
                return Array.Empty<double>();
            }

            var result = new List<double>();
            var n = pricesLength - tau_delayNumber + 1;
            for (var s = 1; s <= pricesLength; s++)
            {
                if (s < tau_delayNumber)
                {
                    var item = 0d;
                    for (var i = 0; i < s; i++)
                    {
                        item += newX[i, s - i - 1];
                    }

                    result.Add(item / s);
                    continue;
                }

                if (s < n)
                {
                    var item = 0d;
                    for (var i = 0; i < tau_delayNumber; i++)
                    {
                        item += newX[i, s - i - 1];
                    }

                    result.Add(item / tau_delayNumber);
                    continue;
                }

                var lastItem = 0d;
                for (var i = 0; i < (pricesLength - s + 1); i++)
                {
                    lastItem += newX[i + s - n, n - i - 1];
                }

                result.Add(lastItem / (pricesLength - s + 1));
            }

            return result.ToArray();
        }

        #endregion

        #endregion
    }
}
