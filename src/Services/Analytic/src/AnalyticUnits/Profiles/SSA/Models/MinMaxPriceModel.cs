using System;
using System.Linq;

namespace Analytic.AnalyticUnits.Profiles.SSA.Models
{
    /// <summary>
    ///     Содержит в себе информацию о максимальной и минимальной цене предсказания
    /// </summary>
    internal class MinMaxPriceModel
    {
        #region .ctor

        /// <summary>
        ///     Создает модель
        /// </summary>
        /// <param name="predictions"> Предсказанные значения </param>
        public static MinMaxPriceModel Create(string pairName, double[] predictions)
        {
            var minPrice = predictions.Min();
            var minIndex = Array.IndexOf(predictions, minPrice);
            var maxPrice = predictions.Max();
            var maxIndex = Array.IndexOf(predictions, maxPrice);

            return new MinMaxPriceModel(pairName, predictions, minPrice, minIndex, maxPrice, maxIndex);
        }

        private MinMaxPriceModel(string pairName, double[] predictions, double minPrice, int minIndex, double maxPrice, int maxIndex)
        {
            PairName = pairName;
            PredictedPrices = predictions;
            MinPrice = minPrice;
            MinIndex = minIndex;
            MaxPrice = maxPrice;
            MaxIndex = maxIndex;
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
        public double[] PredictedPrices { get; }

        /// <summary>
        ///     Минимальная цена предсказания
        /// </summary>
        public double MinPrice { get; }

        /// <summary>
        ///     Позиция минимальной цены в массиве предсказаний
        /// </summary>
        public int MinIndex { get; }

        /// <summary>
        ///     Максимальная цена предсказания
        /// </summary>
        public double MaxPrice { get; }

        /// <summary>
        ///     Позиция максимальной цены в массиве предсказаний
        /// </summary>
        public int MaxIndex { get; }

        #endregion
    }
}
