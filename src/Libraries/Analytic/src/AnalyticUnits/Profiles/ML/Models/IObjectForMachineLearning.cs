using Analytic.Filters.Enums;
using Microsoft.ML.Data;
using System;

namespace Analytic.AnalyticUnits.Profiles.ML.Models
{
    /// <summary>
    ///     Объект, использующийся для обучения модели машинного обучения
    /// </summary>
    public interface IObjectForMachineLearning
    {
        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public float ClosePrice { get; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        [NoColumn]
        public double ClosePriceDouble { get; }

        /// <summary>
        ///     Тип агрегирования данных
        /// </summary>
        [NoColumn]
        public AggregateDataIntervalType AggregateDataInterval { get; }

        /// <summary>
        ///     Время события
        /// </summary>
        public DateTime EventTime { get; }
    }
}