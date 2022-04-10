using BinanceDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories.HotRepositories
{
    /// <summary>
    ///     Репозиторий таблицы "горячих" усеченных данных о торговом объекте
    /// </summary>
    public interface IHotMiniTickerRepository : IRepositoryBase<HotMiniTickerEntity>
    {
        /// <summary>
        ///     Множественное добавление сущностей
        /// </summary>
        Task AddRangeAsync(IEnumerable<HotMiniTickerEntity> miniTickerEntities, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить необходимое кол-во объектов
        /// </summary>
        Task<HotMiniTickerEntity[]> GetArrayAsync(string pair, int neededCount = 2000, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Удаляет все записи до указанной даты
        /// </summary>
        int RemoveUntil(DateTime until);
    }
}
