using Analytic.Models;
using BinanceDatabase.Entities;
using BinanceDatabase.Repositories;
using ExchangeLibrary;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        #endregion

        #region .ctor

        /// <inheritdoc cref="SsaAnalyticPofile"/>
        public SsaAnalyticPofile(string name)
        {
            Name = name;
            _logger.Trace($"Directory with saved grafics='{AppDomain.CurrentDomain.BaseDirectory}'");
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
            using var scope = serviceScopeFactory.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var enities = await database.HotUnitOfWork.HotMiniTickers.GetArrayAsync(model.TradeObjectName, cancellationToken: cancellationToken);
            if (!enities.Any())
            {
                return (false, null);
            }

            var data = enities.Select(_ => _.Price).ToArray();
            var predictions = SSA(data);
            if (!predictions.Any())
            {
                return (false, null);
            }

            _logger.Trace($"Successful predicted {predictions.Length} prices for {model.TradeObjectName}");
            await SavePredictionAsync(database, model.TradeObjectName, enities.Last().ReceivedTime, predictions, cancellationToken);
            var (minPrice, maxPrice) = GetMinMaxPredictedPrice(predictions);
            if (maxPrice < minPrice * 1.15)
            {
                return (false, null);
            }

            var canCreateChart = CanCreateChart(data, predictions, enities[0].ReceivedTime, model.TradeObjectName, out var imagePath);

            var result = new AnalyticResultModel()
            {
                TradeObjectName = model.TradeObjectName,
                HasPredictionImage = canCreateChart,
                RecommendedPurchasePrice = minPrice,
                RecommendedSellingPrice = maxPrice,
                ImagePath = imagePath
            };

            return (true, result);
        }

        #endregion

        #region Private methods

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
        double[] SSA(double[] array)
        {
            #region Преобразование одномерного ряда в многомерный

            var tau = (array.Length + 1) / 2;
            var n = array.Length - tau + 1;

            var rectangleMatrix = Matrix<double>.Build.Dense(tau, n);
            for (var i = 0; i < tau; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    rectangleMatrix[i, j] = array[i + j];
                }
            }

            #endregion

            // Построение ковариационной матрицы
            var transposeMatr = rectangleMatrix.Transpose();
            var multiplicationMatrix = rectangleMatrix * transposeMatr;

            var covariationMatrix = 1 / (double)n * multiplicationMatrix;

            // Определение собственных значений и собственных векторов матрицы
            var evd = covariationMatrix.Evd();
            var matrixEigenvectors = evd.EigenVectors;
            var matrixEigenvalues = evd.EigenValues;

            var matrixEigenvectorsTranspose = matrixEigenvectors.Transpose();

            // Переход к главным компонентам
            // Представление матрицы собственных векторов как матрицу перехода к главным компонентам
            var mainComponents = matrixEigenvectorsTranspose * rectangleMatrix;

            // SSA-сглаживание
            var eigenvaluesArray = matrixEigenvalues.ToList();
            eigenvaluesArray.Sort((x, y) =>
            {
                return x.Magnitude < y.Magnitude ? 1 : x.Magnitude == y.Magnitude ? 0 : -1;
            });

            var neededLambda = eigenvaluesArray.TakeWhile(_ => _.Magnitude > 0.1);

            var max = matrixEigenvalues.AbsoluteMaximum();
            var subMatrixEigenvectors = new List<Vector<double>>(); // понадобится для предсказаний
            for (var i = 0; i < matrixEigenvalues.Count; i++)
            {
                // зануляем столбцы собсвтенных векторов, которые незначимы (сглаживаем)
                if (neededLambda.Contains(matrixEigenvalues[i]))
                {
                    subMatrixEigenvectors.Add(matrixEigenvectors.Column(i));
                    continue;
                }

                matrixEigenvectors.ClearColumn(i);
            }

            var newX = (matrixEigenvectors * mainComponents).ToArray();

            // Восстанавливаю сигнал
            // var N = array.Length;
            // var result = SignalRecovery(newX, N, tau);

            // предсказываем только четвертую часть от исходных данных
            var predictions = ForecastingSignal(subMatrixEigenvectors, array, tau, array.Length / 4);

            return predictions;
        }

        /// <summary>
        ///     Предсказывает сигнал
        /// </summary>
        /// <param name="subEigenvectors"> Отобранные собственные вектора </param>
        /// <param name="array"> Исходыный массив </param>
        /// <param name="tau"> Кол-во задержек </param>
        /// <param name="neededForecastingCount"> Кол-во предсказаний </param>
        private double[] ForecastingSignal(List<Vector<double>> subEigenvectors, double[] array, int tau, int neededForecastingCount)
        {
            var list = array.ToList();
            var result = new List<double>();
            if (!subEigenvectors.Any())
            {
                return Array.Empty<double>();
            }

            for (var k = 0; k < neededForecastingCount; k++)
            {
                var Q = Matrix<double>.Build.Dense(tau - 1, 1);
                var index = 0;
                for (var i = list.Count - tau + 1; i < list.Count && index < tau - 1; i++, index++)
                {
                    Q[index, 0] = list[i];
                }

                var (tauMatrixEigenvector, Vstar) = PreparedMatrixForForecasting(subEigenvectors, tau);
                var numerator = (tauMatrixEigenvector * Vstar.Transpose() * Q)[0, 0];
                var denominator = (1 - tauMatrixEigenvector * tauMatrixEigenvector.Transpose())[0, 0];
                var newX = numerator / denominator;

                list.Add(newX);
                result.Add(newX);
            }

            return result.ToArray();
        }

        /// <summary>
        ///     Подготавливает матрицы для предсказания по алгоритму
        /// </summary>
        /// <param name="subEigenvectors"> Отобранные собственные вектора </param>
        /// <param name="tau"> Кол-во задержек </param>
        /// <returns>
        ///     <see langword="tauMatrixEigenvector"/> Тау-вектор (последняя строка <paramref name="subEigenvectors"/>) <br/>
        ///     <see langword="Vstar"/> Матрица значимых собственных векторов без последней строки
        /// </returns>
        private (Matrix<double> tauMatrixEigenvector, Matrix<double> Vstar) PreparedMatrixForForecasting(
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

            return (tauMatrixEigenvector, Vstar);
        }

        /// <summary>
        ///     Пфтается создать файл с графиком
        /// </summary>
        private bool CanCreateChart(double[] original, double[] predictions, DateTime startTime, string pair, out string imagePath)
        {
            imagePath = string.Empty;
            try
            {
                var plt = new ScottPlot.Plot();
                var timeForReal = new double[original.Length];
                var timeForPredictions = new double[predictions.Length];
                for (var i = 0; i < timeForReal.Length; i++)
                {
                    startTime = startTime.AddSeconds(1);
                    timeForReal[i] = startTime.ToOADate();
                }

                for (var i = 0; i < timeForPredictions.Length; i++)
                {
                    startTime = startTime.AddSeconds(1);
                    timeForPredictions[i] = startTime.ToOADate();
                }

                plt.AddScatter(timeForReal, original, label: "Real");
                plt.AddScatter(timeForPredictions, predictions, label: "Predicted");

                plt.XAxis.TickLabelFormat("t", dateTimeFormat: true);

                plt.XAxis.DateTimeFormat(true);
                plt.YAxis.Label("Price");
                plt.XAxis.Label("Time");
                plt.XAxis2.Label(pair);
                plt.Legend();

                var directoryPath = Path.Combine(_baseDirectory, "Grafics");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                imagePath = Path.Combine(directoryPath, $"{pair}.png");
                plt.SaveFig(imagePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to create and save image for {pair}");

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Возвращает значения минимальной цены и СЛЕДУЮЩЕЙ после него максимальной цены
        /// </summary>
        private (double minPrice, double maxPrice) GetMinMaxPredictedPrice(double[] predictions)
        {
            var minPrice = predictions.Min();
            var minIndex = Array.IndexOf(predictions, minPrice);
            var subArray = new double[predictions.Length - minIndex];
            Array.Copy(predictions, minIndex, subArray, 0, subArray.Length);
            var maxPrice = subArray.Max();

            return (minPrice, maxPrice);
        }

        /// <summary>
        ///     Сохраняет предсказанные значения в бд
        /// </summary>
        private async Task SavePredictionAsync(
            IUnitOfWork database,
            string tradeObjectName,
            DateTime predictionTime,
            double[] predictions,
            CancellationToken cancellationToken)
        {
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
            catch(Exception ex)
            {
                _logger.Error(ex, "Failed to save predicted data");
            }            
        }

        #region Signal recovery

        /// <summary>
        ///     Восстанавливает сигнал
        /// </summary>
        /// <param name="newX"> Восстановленная исходная матрица </param>
        /// <param name="N"> Кол-во значений в первоначальном векторе-сигнале </param>
        /// <param name="tau"> Число задержек </param>
        private double[] SignalRecovery(double[,] newX, int N, int tau)
        {
            var result = new List<double>();
            var n = N - tau + 1;
            for (var s = 1; s <= N; s++)
            {
                if (s < tau)
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
                    for (var i = 0; i < tau; i++)
                    {
                        item += newX[i, s - i - 1];
                    }

                    result.Add(item / tau);
                    continue;
                }

                var lastItem = 0d;
                for (var i = 0; i < (N - s + 1); i++)
                {
                    lastItem += newX[i + s - n, n - i - 1];
                }

                result.Add(lastItem / (N - s + 1));
            }

            return result.ToArray();
        }

        #endregion

        #endregion
    }
}
