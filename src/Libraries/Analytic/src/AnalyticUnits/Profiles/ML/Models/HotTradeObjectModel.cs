using System;

namespace Analytic.AnalyticUnits.Profiles.ML.Models
{
    /// <summary>
    ///     Модель объекта торговли
    /// </summary>
    internal class HotTradeObjectModel
    {
        #region Features

        /// <summary>
        ///     Время события
        /// </summary>
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public float Price { get; set; }

        #endregion
    }
}
