using System;
using System.Collections.Generic;

namespace Analytic.Models
{
    /// <summary>
    ///     Модель результата работы аналитического сервиса
    /// </summary>
    public class AnalyticResultModel
    {
        /// <summary>
        ///     Название объекта торговли
        /// </summary>
        public string TradeObjectName { get; internal set; }

        /// <summary>
        ///     Рекомендуемая цена покупки
        /// </summary>
        public double RecommendedPurchasePrice { get; internal set; }

        /// <summary>
        ///     Рекомендуемая цена продажи
        /// </summary>
        public double? RecommendedSellingPrice { get; internal set; }

        /// <summary>
        ///     Объем спроса
        /// </summary>
        [Obsolete("Не используется")]
        public double BidVolume { get; internal set; }

        /// <summary>
        ///     Объем предложения
        /// </summary>
        [Obsolete("Не используется")]
        public double AskVolume { get; internal set; }

        /// <summary>
        ///     Последнее отклонение
        /// </summary>
        [Obsolete("Не используется")]
        public double LastDeviation { get; internal set; }

        /// <summary>
        ///     Суммарное отклонение за 5 таймфреймов
        /// </summary>
        [Obsolete("Не используется")]
        public double SumDeviations { get; internal set; }

        /// <summary>
        ///     Отклонения цены за последние таймфреймы в процентах
        /// </summary>
        [Obsolete("Не используется")]
        public List<double> PricePercentDeviations { get; internal set; } = new();

        /// <summary>
        ///     Если график с предсказанной ценой
        /// </summary>
        public bool HasPredictionImage { get; internal set; }

        /// <summary>
        ///     Путь к изображению
        /// </summary>
        public string ImagePath { get; internal set; }
    }
}
