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
        public string TradeObjectName { get; set; }

        /// <summary>
        ///     Рекомендуемая цена покупки
        /// </summary>
        public double RecommendedPurchasePrice { get; set; }

        /// <summary>
        ///     Объем спроса
        /// </summary>
        public double BidVolume { get; internal set; }

        /// <summary>
        ///     Объем предложения
        /// </summary>
        public double AskVolume { get; internal set; }

        /// <summary>
        ///     Последнее отклонение
        /// </summary>
        public double LastDeviation { get; internal set; }

        /// <summary>
        ///     Суммарное отклонение за 5 таймфреймов
        /// </summary>
        public double SumDeviations { get; internal set; }

        /// <summary>
        ///     Отклонения цены за последние таймфреймы в процентах
        /// </summary>
        public List<double> PricePercentDeviations { get; internal set; } = new();
    }
}
