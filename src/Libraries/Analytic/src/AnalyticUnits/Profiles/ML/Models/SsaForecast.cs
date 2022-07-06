namespace Analytic.AnalyticUnits.Profiles.ML.Models
{
    /// <summary>
    ///     Модель прогноза цены пары при использовании SSA
    /// </summary>
    internal class SsaForecast
    {
        /// <summary>
        ///     Прогнозы цены
        /// </summary>
        public float[] Forecast { get; set; }
    }
}
