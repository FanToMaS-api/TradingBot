using Analytic.Models;
using ExchangeLibrary;
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
        List<IAnalyticProfile> AnalyticUnits { get; }

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
        ///     Удаляет анализатор или профиль анализаторов
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        bool Remove(string name);

        /// <summary>
        ///     Анализирует объект торговли
        /// </summary>
        /// <param name="exchange"> Биржа </param>
        /// <param name="model"> Модель </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        /// <returns> 
        ///     <see langword="true, model"/> Если объект стоит купить <br/>
        ///     <see langword="false"/> Если объект не стоит покупать
        /// </returns>
        Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IExchange exchange,
            InfoModel model,
            CancellationToken cancellationToken);
    }
}
