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
        public static MinMaxPriceModel Create(string tradeObjectName, double[] predictions)
        {
            var minPrice = predictions.Min();
            var minIndex = Array.IndexOf(predictions, minPrice);
            var maxPrice = predictions.Max();
            var maxIndex = Array.IndexOf(predictions, maxPrice);

            var model = new MinMaxPriceModel
            {
                PredictedPrices = predictions,
                TradeObjectName = tradeObjectName,
                MinPrice = minPrice,
                MinIndex = minIndex,
                MaxPrice = maxPrice,
                MaxIndex = maxIndex,
            };

            return model;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Название пары
        /// </summary>
        public string TradeObjectName { get; init; }

        /// <summary>
        ///     Спрогнозированная цена
        /// </summary>
        public double[] PredictedPrices { get; init; }

        /// <summary>
        ///     Минимальная цена предсказания
        /// </summary>
        public double MinPrice { get; init; }

        /// <summary>
        ///     Позиция минимальной цены в массиве предсказаний
        /// </summary>
        public int MinIndex { get; init; }

        /// <summary>
        ///     Максимальная цена предсказания
        /// </summary>
        public double MaxPrice { get; init; }

        /// <summary>
        ///     Позиция максимальной цены в массиве предсказаний
        /// </summary>
        public int MaxIndex { get; init; }

        #endregion
    }
}
