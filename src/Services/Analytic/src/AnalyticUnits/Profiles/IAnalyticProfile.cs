using Analytic.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Профиль анализа (содержит одну определенную логику анализа)
    /// </summary>
    public interface IAnalyticProfile
    {
        /// <summary>
        ///     Уникальное название элемента анализа
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Анализирует объект торговли
        /// </summary>
        /// <param name="model"> Модель </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        /// <returns> 
        ///     <see langword="true, model"/> Если объект стоит купить <br/>
        ///     <see langword="false"/> Если объект не стоит покупать
        /// </returns>
        Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IServiceScopeFactory serviceScopeFactory,
            InfoModel model,
            CancellationToken cancellationToken);
    }
}
