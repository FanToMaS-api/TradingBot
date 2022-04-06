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
        ///     Удаляет все записи до указанной даты
        /// </summary>
        void RemoveUntil(DateTime until);
    }
}
