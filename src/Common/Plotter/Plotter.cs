using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Plotter
{
    /// <summary>
    ///     Графопостроитель
    /// </summary>
    public class Plotter
    {
        #region Fields

        private const int _numberOfMinutesOfImageStorage = 0; // кол-во минут хранения изображения, по истечении которых будет создано новое (опция отключена  = 0)
        internal static string GraficsFolder = "Grafics"; // папка для хранения графиков
        private readonly ILoggerDecorator _logger;
        private readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Plotter"/>
        public Plotter(ILoggerDecorator logger)
        {
            _logger = logger;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Название пары
        /// </summary>
        public string PairName { get; set; }

        /// <summary>
        ///     Предсказанные значения цены
        /// </summary>
        public double[] PredictedPrices { get; set; }

        /// <summary>
        ///     Минимальная цена предсказания
        /// </summary>
        public double MinPrice { get; set; }

        /// <summary>
        ///     Позиция минимальной цены в массиве предсказаний
        /// </summary>
        public int MinIndex { get; set; }

        /// <summary>
        ///     Максимальная цена предсказания
        /// </summary>
        public double MaxPrice { get; set; }

        /// <summary>
        ///     Позиция максимальной цены в массиве предсказаний
        /// </summary>
        public int MaxIndex { get; set; }

        #endregion

        /// <summary>
        ///     Пытается создать файл с графиком
        /// </summary>
        /// <param name="original"> Оригинальные данные </param>
        /// <param name="dateTimes"> Время получения оригинальных данных </param>
        /// <param name="imagePath"> Путь до изображения </param>
        /// <returns> <see langword="true"/> если изображение было успешно создано </returns>
        public bool CanCreateChart(
            double[] original,
            IEnumerable<DateTime> dateTimes,
            out string imagePath)
        {
            if (!IsValid())
            {
                imagePath = string.Empty;
                return false;
            }

            var directoryPath = GetOrCreateFolderPath(GraficsFolder);
            imagePath = Path.Combine(directoryPath, $"{PairName}.png");
            if (File.Exists(imagePath)
                && File.GetCreationTime(imagePath).AddMinutes(_numberOfMinutesOfImageStorage) > DateTime.Now)
            {
                _logger.TraceAsync($"The graph for '{PairName}' was created recently, " +
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
                var timeForPredictions = CreatePredictionsDates(startTimeForPredictionsData, PredictedPrices.Length, offset);
                AddCaptionsToChart(plot, original, timeForPredictions, dateTimes);

                plot.SaveFig(imagePath);

                _logger.TraceAsync($"Successful create grafic for model {PairName}. Path: {imagePath}");
            }
            catch (Exception ex)
            {
                _logger.ErrorAsync(ex, $"Failed to create and save image for {PairName}").Wait(5 * 1000);

                return false;
            }

            return true;
        }

        #region Private methods

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
        ///     Проверяет валидность данных перед построением
        /// </summary>
        private bool IsValid()
        {
            if (string.IsNullOrEmpty(PairName))
            {
                _logger.WarnAsync($"Failed to create image, '{nameof(PairName)}' is null or empty");
                return false;
            }

            var length = PredictedPrices.Length;
            if (length == 0)
            {
                return false;
            }

#pragma warning disable IDE0046 // Convert to conditional expression
            if (MinIndex >= length)
            {
                return false;
            }
#pragma warning restore IDE0046 // Convert to conditional expression

            return MaxIndex < length;
        }

        /// <summary>
        ///     Добавляет подписи к графику
        /// </summary>
        private void AddCaptionsToChart(
            ScottPlot.Plot plot,
            double[] original,
            double[] timeForPredictions,
            IEnumerable<DateTime> dateTimes)
        {
            plot.AddScatter(dateTimes.Select(_ => _.ToOADate()).ToArray(), original, label: "Real");
            plot.AddScatter(timeForPredictions, PredictedPrices, label: "Predicted");
            plot.XAxis.TickLabelFormat("g", dateTimeFormat: true);

            plot.AddText(
                $"Min: {MinPrice:0.0000}",
                timeForPredictions[MinIndex],
                MinPrice, 17);
            plot.AddText(
                $"Max: {MaxPrice:0.0000}",
                timeForPredictions[MaxIndex],
                MaxPrice, 17);

            plot.YAxis.Label("Price");
            plot.XAxis.Label("Time");
            plot.XAxis.TickLabelStyle(rotation: 45);
            plot.XAxis2.Label(PairName);
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

        #endregion
    }
}
