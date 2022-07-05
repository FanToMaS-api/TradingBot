using Analytic.AnalyticUnits.Profiles.ML.Models;
using System.Collections.Generic;

namespace Analytic.DataLoaders
{
    /// <summary>
    ///     Помогает загружать данные с бд
    /// </summary>
    public interface IDataLoader
    {
        /// <summary>
        ///     Получить объекты для обучения модели SSA
        /// </summary>
        IEnumerable<IObjectForMachineLearning> GetDataForSsa(
            string tradeObjectName,
            int numberPricesToTake);
    }
}
