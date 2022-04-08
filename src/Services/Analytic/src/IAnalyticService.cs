using Analytic.AnalyticUnits;
using Analytic.Filters;
using Analytic.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic
{
    /// <summary>
    ///     Сервис аналитики
    /// </summary>
    public interface IAnalyticService : IDisposable
    {
        /// <summary>
        ///     Профили аналитики
        /// </summary>
        List<IProfileGroup> ProfileGroups { get; }
        
        /// <summary>
        ///     Фильтры данных
        /// </summary>
        List<IFilterGroup> FilterGroups { get; }

        /// <summary>
        ///     Событие, возникающее после фильтрации полученных данных 
        /// </summary>
        EventHandler<InfoModel[]> OnModelsFiltered { get; set; }

        /// <summary>
        ///     Событие, возникающее, если есть торговые объекты для покупки
        /// </summary>
        EventHandler<AnalyticResultModel[]> OnModelsReadyToBuy { get; set; }

        /// <summary>
        ///     Запускает сервис аналитики
        /// </summary>
        Task RunAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Останавливает сервис аналитики
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Добавить группу профилей аналитики
        /// </summary>
        void AddProfileGroup(IProfileGroup profileGroup);

        /// <summary>
        ///     Удалить группу профилей аналитики
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        bool RemoveProfileGroup(IProfileGroup profileGroup);

        /// <summary>
        ///     Удалить профиль аналитики
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        bool RemoveProfile(string profileName);

        /// <summary>
        ///     Добавить группу фильтров
        /// </summary>
        void AddFilterGroup(IFilterGroup filterGroup);

        /// <summary>
        ///     Удалить группу фильтров
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        bool RemoveFilterGroup(IFilterGroup filterGroup);

        /// <summary>
        ///     Удалить фильтр
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        bool RemoveFilter(string filterName);
    }
}
