using Analytic.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Группа профилей аналитики
    /// </summary>
    public interface IProfileGroup
    {
        /// <summary>
        ///     Уникальное название группы анализа
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Профили аналитики
        /// </summary>
        List<IAnalyticProfile> AnalyticProfiles { get; }

        /// <summary>
        ///     Используется ли профиль
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        ///     Добавить профиль аналитики
        /// </summary>
        void AddAnalyticUnit(IAnalyticProfile unit);

        /// <summary>
        ///     Изменить статус профиля
        /// </summary>
        void ChangeProfileActivity(bool isAsctive);

        /// <summary>
        ///     Удаляет профиль аналитики
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        bool Remove(string name);

        /// <summary>
        ///     Анализирует объект торговли
        /// </summary>
        /// <param name="model"> Модель </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        /// <returns> 
        ///     <see langword="true, model"/> Если объект был успешно проанализирован<br/>
        ///     <see langword="false, null"/> Если объект был успешно проанализирован
        /// </returns>
        Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IServiceScopeFactory serviceScopeFactory,
            InfoModel model,
            CancellationToken cancellationToken);
    }
}
