using BinanceDatabase.Entities;
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
    }
}
