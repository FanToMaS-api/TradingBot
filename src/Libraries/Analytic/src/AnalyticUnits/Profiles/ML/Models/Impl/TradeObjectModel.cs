using Analytic.Filters.Enums;
using Microsoft.ML.Data;
using System;

namespace Analytic.AnalyticUnits.Profiles.ML.Models.Impl
{
    /// <summary>
    ///     Модель объекта торговли для прогноза цены с помощью машинного обучения
    /// </summary>
    internal class TradeObjectModel : IObjectForMachineLearning
    {
        #region Features

        /// <summary>
        ///     Время события
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        ///     Тип агрегирования данных
        /// </summary>
        [NoColumn]
        public AggregateDataIntervalType AggregateDataInterval { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public float ClosePrice { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public double ClosePriceDouble { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        public float OpenPrice { get; set; }

        /// <summary>
        ///     Разница между ценой закрытия и открытия
        /// </summary>
        public float PriceDeviationPercent { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        public float MinPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        public float MaxPrice { get; set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        public float BasePurchaseVolume { get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        public float QuotePurchaseVolume { get; set; }

        #endregion
    }
}
