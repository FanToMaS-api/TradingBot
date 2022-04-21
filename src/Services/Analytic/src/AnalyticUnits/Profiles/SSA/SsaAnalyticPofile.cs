using Analytic.AnalyticUnits.Profiles.SSA.Models;
using Analytic.Models;
using BinanceDatabase;
using BinanceDatabase.Entities;
using Logger;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.DependencyInjection;
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

        private readonly ILoggerDecorator _logger;
        private readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private const int _numberPricesToTake = 2498; // кол-во данных участвующих в предсказании цены
        private const int _numberOfMinutesOfImageStorage = 2; // кол-во минут хранения изображения
                                                              // (не будут присылаться сообщения с изображением, дабы не спамить)
        internal static string GraficsFolder = "Grafics"; // папка для хранения графиков
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

            var minMaxPriceModel = MinMaxPriceModel.Create(pairName, predictions);
            if (minMaxPriceModel.MinPrice <= 0)
            {
                return (false, null);
            }

            var canCreateChart = CanCreateChart(
                prices,
                enities.Select(_ => _.ReceivedTime),
                minMaxPriceModel,
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
        private HotMiniTickerEntity[] GetHotMiniTickers(IServiceScopeFactory serviceScopeFactory, string pairName)
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
                prices.Length / 4);

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
        ///     Пытается создать файл с графиком
        /// </summary>
        /// <param name="original"> Оригинальные данные </param>
        /// <param name="predictions"> Предсказанные данные </param>
        /// <param name="dateTimes"> Время получения оригинальных данных </param>
        /// <param name="pair"> Название пары </param>
        /// <param name="minMaxPriceModel"> <see cref="MinMaxPriceModel"/> </param>
        /// <param name="imagePath"> Путь до изображения </param>
        /// <returns> <see langword="true"/> если изображение было успешно создано </returns>
        internal bool CanCreateChart(
            double[] original,
            IEnumerable<DateTime> dateTimes,
            MinMaxPriceModel minMaxPriceModel,
            out string imagePath)
        {
            var directoryPath = GetOrCreateFolderPath(GraficsFolder);
            imagePath = Path.Combine(directoryPath, $"{minMaxPriceModel.PairName}.png");
            if (File.Exists(imagePath) 
                && File.GetCreationTime(imagePath).AddMinutes(_numberOfMinutesOfImageStorage) > DateTime.Now)
            {
                _logger.TraceAsync($"The graph for '{minMaxPriceModel.PairName}' was created recently, " +
                    "saving and processing is canceled. Path: {directoryPath}");
                return false;
            }

            try
            {
                var plot = new ScottPlot.Plot(1000, 800)
                {
                    Palette = ScottPlot.Palette.OneHalfDark
                };

                // массив времени для предсказанных данных
                var startTimeForPredictionsData = dateTimes.Last();
                var endTime = dateTimes.First();
                var offset = (int)Math.Ceiling(Math.Abs((startTimeForPredictionsData - endTime).TotalSeconds / dateTimes.Count()));
                var timeForPredictions = CreatePredictionsDates(startTimeForPredictionsData, minMaxPriceModel.PredictedPrices.Length, offset);
                AddCaptionsToChart(plot, minMaxPriceModel, original, timeForPredictions, dateTimes);

                plot.SaveFig(imagePath);

                _logger.TraceAsync($"Successful create grafic for model {minMaxPriceModel.PairName}. Path: {imagePath}");
            }
            catch (Exception ex)
            {
                _logger.ErrorAsync(ex, $"Failed to create and save image for {minMaxPriceModel.PairName}").Wait(5 * 1000);

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Добавляет подписи к графику
        /// </summary>
        private void AddCaptionsToChart(
            ScottPlot.Plot plot,
            MinMaxPriceModel minMaxPriceModel,
            double[] original,
            double[] timeForPredictions,
            IEnumerable<DateTime> dateTimes)
        {
            plot.AddScatter(dateTimes.Select(_ => _.ToOADate()).ToArray(), original, label: "Real");
            plot.AddScatter(timeForPredictions, minMaxPriceModel.PredictedPrices, label: "Predicted");
            plot.XAxis.TickLabelFormat("g", dateTimeFormat: true);

            plot.AddText(
                $"Min: {minMaxPriceModel.MinPrice:0.0000}",
                timeForPredictions[minMaxPriceModel.MinIndex],
                minMaxPriceModel.MinPrice, 17);
            plot.AddText(
                $"Max: {minMaxPriceModel.MaxPrice:0.0000}",
                timeForPredictions[minMaxPriceModel.MaxIndex],
                minMaxPriceModel.MaxPrice, 17);

            plot.YAxis.Label("Price");
            plot.XAxis.Label("Time");
            plot.XAxis.TickLabelStyle(rotation: 45);
            plot.XAxis2.Label(minMaxPriceModel.PairName);
            plot.Legend(true, ScottPlot.Alignment.LowerLeft);

            plot.Style(ScottPlot.Style.Gray1);
            var bnColor = System.Drawing.ColorTranslator.FromHtml("#2e3440");
            plot.Style(figureBackground: bnColor, dataBackground: bnColor, tick: System.Drawing.Color.WhiteSmoke);
        }

        /// <summary>
        ///     Возвращает путь до указанной папки, создает, если ее не существовало
        /// </summary>
        internal string GetOrCreateFolderPath(string directoryName)
        {
            var directoryPath = Path.Combine(_baseDirectory, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                _logger.TraceAsync($"Successful created directory {directoryPath}");
            }

            return directoryPath;
        }

        /// <summary>
        ///     Возвращает массив дат в OA формате для подписей осей предсказанных данных
        /// </summary>
        private static double[] CreatePredictionsDates(DateTime from, int predictionDataLength, int offset)
        {
            var timesForPredictions = new double[predictionDataLength];
            for (var i = 0; i < predictionDataLength; i++)
            {
                from = from.AddSeconds(offset);
                timesForPredictions[i] = from.ToOADate();
            }

            return timesForPredictions;
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
