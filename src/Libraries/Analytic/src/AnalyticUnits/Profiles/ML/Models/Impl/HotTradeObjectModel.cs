using Analytic.Filters.Enums;
using Microsoft.ML.Data;
using System;

namespace Analytic.AnalyticUnits.Profiles.ML.Models.Impl
{
    /// <summary>
    ///     Модель объекта торговли
    /// </summary>
    internal class HotTradeObjectModel : IObjectForMachineLearning
    {
        #region Features

        /// <inheritdoc />
        public float ClosePrice { get; set; }

        /// <inheritdoc />
        [NoColumn]
        public double ClosePriceDouble { get; set;}

        /// <inheritdoc />
        [NoColumn]
        public AggregateDataIntervalType AggregateDataInterval { get; set; }

        /// <inheritdoc />
        public DateTime EventTime { get; set; }

        #endregion
    }
}
