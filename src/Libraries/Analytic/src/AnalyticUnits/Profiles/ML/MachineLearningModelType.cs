namespace Analytic.AnalyticUnits.Profiles.ML
{
    /// <summary>
    ///     Тип моделей машинного обучения для прогноза цен
    /// </summary>
    public enum MachineLearningModelType
    {
        /// <summary>
        ///     Модель использует алгоритм сингулярного спектрального анализа для прогноза цены
        /// </summary>
        SSA = 0,

        /// <summary>
        ///     Модель использует тренера FastTree для прогнозов
        /// </summary>
        FastTree = 1,

        // TODO: other
    }
}
