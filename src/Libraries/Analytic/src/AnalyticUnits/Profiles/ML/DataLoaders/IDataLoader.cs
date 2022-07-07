using Analytic.AnalyticUnits.Profiles.ML.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Analytic.DataLoaders
{
    /// <summary>
    ///     Помогает загружать данные с бд
    /// </summary>
    public interface IDataLoader
    {
        /// <summary>
        ///     Получить объекты для обучения
        /// </summary>
        IEnumerable<IObjectForMachineLearning> GetData(
            IServiceScopeFactory serviceScopeFactory,
            string tradeObjectName,
            int numberPricesToTake);
    }
}
