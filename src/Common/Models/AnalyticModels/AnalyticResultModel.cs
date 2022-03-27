namespace Common.Models
{
    /// <summary>
    ///     Модель результата работы аналитического сервиса
    /// </summary>
    public class AnalyticResultModel
    {
        /// <summary>
        ///     Название объекта торговли
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     Рекомендуемая цена покупки
        /// </summary>
        public double RecommendedPurchasePrice { get; set; }

        /// <summary>
        ///     Рекомендуемая цена продажи
        /// </summary>
        public double RecommendedSellingPrice { get; set; }
    }
}
