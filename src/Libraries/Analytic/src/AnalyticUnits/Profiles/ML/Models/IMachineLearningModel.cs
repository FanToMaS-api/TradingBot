using System;
using System.Collections.Generic;

namespace Analytic.AnalyticUnits.Profiles.ML.Models
{
    /// <summary>
    ///     Определяет модель машинного обучения для прогнозов
    /// </summary>
    public interface IMachineLearningModel
    {
        /// <summary>
        ///     Кол-во данных, участвующих в прогнозе цены
        /// </summary>
        public int NumberPricesToTake { get; }

        /// <summary>
        ///     Прогнозирует цену
        /// </summary>
        /// <param name="data">
        ///     Данные, на которых будет обучаться модель
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Если <paramref name="data"/> пусто
        /// </exception>
        float[] Forecast(IEnumerable<IObjectForMachineLearning> data);
    }
}
