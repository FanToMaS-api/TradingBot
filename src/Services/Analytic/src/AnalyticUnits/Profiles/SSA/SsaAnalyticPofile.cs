using Analytic.Models;
using BinanceDatabase;
using BinanceDatabase.Entities;
using BinanceDatabase.Repositories;
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
        private const int _numberPricesToTake = 2498; // кол-во данных участвующих в предсказании цены
        private const int _numberOfMinutesOfImageStorage = 2; // кол-во минут хранения изображения (не будут присылаться сообщения с изображением, дабы не спамить)
        private const string _graficsFolder = "Grafics"; // папка для хранения графиков

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
            var databaseFactory = scope.ServiceProvider.GetRequiredService<IBinanceDbContextFactory>();
            using var database = databaseFactory.CreateScopeDatabase();
            var enities = database.HotUnitOfWork.HotMiniTickers.GetArray(model.TradeObjectName, _numberPricesToTake);
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

            var minMaxPriceModel = MinMaxPriceModel.Create(predictions);
            if (minMaxPriceModel.MinPrice <= 0)
            {
                return (false, null);
            }

            var canCreateChart = CanCreateChart(
                data,
                predictions,
                enities.Select(_ => _.ReceivedTime),
                model.TradeObjectName,
                minMaxPriceModel,
                out var imagePath);
            

            var result = new AnalyticResultModel()
            {
                TradeObjectName = model.TradeObjectName,
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

            var neededLambdas = eigenvaluesArray.Where(_ => _.Magnitude > 0.0002);
            var subMatrixEigenvectors = GetSubListEigenvectors(matrixEigenvalues, neededLambdas, matrixEigenvectors);
            if (!subMatrixEigenvectors.Any())
            {
                return Array.Empty<double>();
            }

            var newX = (matrixEigenvectors * mainComponents).ToArray();

            // Восстанавливаю сигнал
            // var N = array.Length;
            // var result = SignalRecovery(newX, N, tau);

            // предсказываем только четвертую часть от исходных данных
            var (Vstar, tauMatrixEigenvector) = GetPreparedMatrixForForecasting(subMatrixEigenvectors, tau);
            var predictions = ForecastingSignal(
                Vstar,
                tauMatrixEigenvector,
                array,
                tau,
                array.Length / 4);

            return predictions;
        }

        /// <summary>
        ///     Возвращает список значимых векторов, для предсказаний <br/>
        ///     Зануляем столбцы собственных векторов, которые незначимы
        /// </summary>
        private List<Vector<double>> GetSubListEigenvectors(
            Vector<System.Numerics.Complex> matrixEigenvalues,
            IEnumerable<System.Numerics.Complex> neededLambdas,
            Matrix<double> matrixEigenvectors)
        {
            var subEigenvectors = new List<Vector<double>>(); // понадобится для предсказаний
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
        private (Matrix<double> Vstar, Matrix<double> tauMatrixEigenvector) GetPreparedMatrixForForecasting(
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
        /// <param name="tau"> Кол-во задержек </param>
        /// <param name="neededForecastingCount"> Кол-во предсказаний </param>
        private double[] ForecastingSignal(
            Matrix<double> Vstar,
            Matrix<double> tauMatrixEigenvector,
            double[] array, 
            int tau, 
            int neededForecastingCount)
        {
            var list = array.ToList();
            var result = new List<double>();

            for (var k = 0; k < neededForecastingCount; k++)
            {
                var Q = Matrix<double>.Build.Dense(tau - 1, 1);
                var index = 0;
                for (var i = list.Count - tau + 1; i < list.Count && index < tau - 1; i++, index++)
                {
                    Q[index, 0] = list[i];
                }

                var numerator = (tauMatrixEigenvector * Vstar.Transpose() * Q)[0, 0];
                var denominator = (1 - tauMatrixEigenvector * tauMatrixEigenvector.Transpose())[0, 0];
                var newX = numerator / denominator;

                list.Add(newX);
                result.Add(newX);
            }

            return result.ToArray();
        }

        /// <summary>
        ///     Пытается создать файл с графиком
        /// </summary>
        /// <param name="original"> Оригинальные данные </param>
        /// <param name="predictions"> Предсказанные данные </param>
        /// <param name="dateTimes"> Время получения оригинальных данных </param>
        /// <param name="pair"> Название пары </param>
        /// <param name="minMaxPriceModel"> <see cref="MinMaxPriceModel"/> </param>
        /// <param name="imagePath"> Путь до изображения </param>
        /// <returns> <see langword="true"/> если изображение было успешно создано </returns>
        private bool CanCreateChart(
            double[] original,
            double[] predictions,
            IEnumerable<DateTime> dateTimes,
            string pair,
            MinMaxPriceModel minMaxPriceModel,
            out string imagePath)
        {
            var directoryPath = GetPathToCurrentAppDirectoryFolder(_graficsFolder);
            imagePath = Path.Combine(directoryPath, $"{pair}.png");
            if (File.Exists(imagePath) && File.GetCreationTime(imagePath).AddMinutes(_numberOfMinutesOfImageStorage) > DateTime.Now)
            {
                _logger.Trace($"The graph for '{pair}' was created recently, saving and processing is canceled. Path: {directoryPath}");
                return false;
            }

            try
            {
                var plt = new ScottPlot.Plot(1000, 800)
                {
                    Palette = ScottPlot.Palette.OneHalfDark
                };

                // массив времени для предсказанных данных
                var startTimeForPredictionsData = dateTimes.Last();
                var endTime = dateTimes.First();
                var offset = (int)Math.Ceiling(Math.Abs((startTimeForPredictionsData - endTime).TotalSeconds / dateTimes.Count()));
                var timeForPredictions = CreatePredictionsDates(startTimeForPredictionsData, predictions.Length, offset);

                plt.AddScatter(dateTimes.Select(_ => _.ToOADate()).ToArray(), original, label: "Real");
                plt.AddScatter(timeForPredictions, predictions, label: "Predicted");
                plt.XAxis.TickLabelFormat("g", dateTimeFormat: true);
                plt.AddText(
                    $"Min: {minMaxPriceModel.MinPrice:0.0000}",
                    timeForPredictions[minMaxPriceModel.MinIndex],
                    minMaxPriceModel.MinPrice, 17);
                plt.AddText(
                    $"Max: {minMaxPriceModel.MaxPrice:0.0000}",
                    timeForPredictions[minMaxPriceModel.MaxIndex],
                    minMaxPriceModel.MaxPrice, 17);

                plt.YAxis.Label("Price");
                plt.XAxis.Label("Time");
                plt.XAxis.TickLabelStyle(rotation: 45);
                plt.XAxis2.Label(pair);
                plt.Legend(true, ScottPlot.Alignment.LowerLeft);

                plt.Style(ScottPlot.Style.Gray1);
                var bnColor = System.Drawing.ColorTranslator.FromHtml("#2e3440");
                plt.Style(figureBackground: bnColor, dataBackground: bnColor, tick: System.Drawing.Color.WhiteSmoke);

                plt.SaveFig(imagePath);

                _logger.Trace($"Successful create grafic for model {pair}. Path: {imagePath}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to create and save image for {pair}");

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Возвращает путь до директории указанной директории, создает, если ее не существовало
        /// </summary>
        private string GetPathToCurrentAppDirectoryFolder(string directoryName)
        {
            var directoryPath = Path.Combine(_baseDirectory, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                _logger.Trace($"Successful created directory {directoryPath}");
            }

            return directoryPath;
        }

        /// <summary>
        ///     Возвращает массив дат в OA формате для подписей осей предсказанных данных
        /// </summary>
        private double[] CreatePredictionsDates(DateTime from, int predictionDataLength, int offset)
        {
            var timeForPredictions = new double[predictionDataLength];
            for (var i = 0; i < predictionDataLength; i++)
            {
                from = from.AddSeconds(offset);
                timeForPredictions[i] = from.ToOADate();
            }

            return timeForPredictions;
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
            catch (Exception ex)
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
