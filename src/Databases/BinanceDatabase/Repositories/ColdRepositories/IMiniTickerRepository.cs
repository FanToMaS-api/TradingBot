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
        ///     Возвращает суммарное отклонение цены при заданном интервале
        /// </summary>
        /// <param name="pair"> Название пары </param>
        /// <param name="interval"> Определяет с каким интервалом идет подсчет отклонений </param>
        /// <param name="count"> Нужное кол-во отклонений в сумме </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task<double> GetPricePercentDeviationSumAsync(
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
