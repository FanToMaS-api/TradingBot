using System;

namespace Analytic.AnalyticUnits.Profiles.ML.Models
{
    /// <summary>
    ///     Модель объекта торговли
    /// </summary>
    internal class TradeObjectModel
    {
        #region Features

        /// <summary>
        ///     Время события
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        ///     Тип агрегирования данных
        /// </summary>
        public float AggregateDataInterval { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public float ClosePrice { get; set; }

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
