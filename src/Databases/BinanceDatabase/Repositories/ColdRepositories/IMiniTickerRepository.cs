using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories.ColdRepositories
{
    /// <summary>
    ///     Репозиторий таблицы усеченных данных о торговом объекте
    /// </summary>
    public interface IMiniTickerRepository : IRepositoryBase<MiniTickerEntity>
    {
        /// <summary>
        ///     Множественное добавление сущностей
        /// </summary>
        Task AddRangeAsync(IEnumerable<MiniTickerEntity> miniTickerEntities, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Удаляет все указанные сущности
        /// </summary>
        void RemoveRange(IEnumerable<MiniTickerEntity> entities);

        /// <summary>
        ///     Получить необходимое кол-во самых актуальных объектов, отсортированных в порядке получения
        /// </summary>
        IEnumerable<MiniTickerEntity> GetEntities(
            string pair,
            AggregateDataIntervalType aggregateDataInterval = AggregateDataIntervalType.Default,
            int? neededCount = null);

        /// <summary>
        ///     Возвращает отклонение цены при заданном интервале между первым и последним элементом
        ///     Например: <paramref name="count"/> = 5 <br />
        ///     Цены в бд: 2 5 6 4 3 8 9 10 <br />
        ///     Выборка: 4 3 8 9 10 <br />
        ///     Отклонение будет высчитываться между элементами 4 и 10
        /// </summary>
        /// <param name="pair"> Название пары </param>
        /// <param name="interval"> Определяет с каким интервалом идет подсчет отклонений </param>
        /// <param name="count"> Кол-во элементов между которым мы рассматриваем отклонение цены </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<double> GetPricePercentDeviationAsync(
            string pair,
            AggregateDataIntervalType interval,
            int count,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Удаляет все записи до указанной даты
        /// </summary>
        /// <param name="before">
        ///     Дата, до которой удаляются данные
        /// </param>
        /// <remarks>
        ///     Контекст сохраняется автоматически внутри функции
        /// </remarks>
        Task<int> RemoveUntilAsync(DateTime before, CancellationToken cancellationToken);
    }
}
