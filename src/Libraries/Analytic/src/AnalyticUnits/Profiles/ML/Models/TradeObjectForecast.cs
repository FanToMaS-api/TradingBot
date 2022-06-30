namespace Analytic.AnalyticUnits.Profiles.ML.Models
{
    /// <summary>
    ///     Модель прогноза цены пары
    /// </summary>
    internal class TradeObjectForecast
    {
        /// <summary>
        ///     Прогноз цены
        /// </summary>
        public float[] Forecast { get; set; }
    }
}
