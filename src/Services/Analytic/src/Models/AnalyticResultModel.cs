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
    }
}
