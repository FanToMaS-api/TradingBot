using BinanceDatabase.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories.ColdRepositories
{
    /// <summary>
    ///     Репозиторий предсказанных ботом данных
    /// </summary>
    public interface IPredictionRepository : IRepositoryBase<PredictionEntity>
    {
        /// <summary>
        ///     Множественное добавление сущностей
        /// </summary>
        Task AddRangeAsync(IEnumerable<PredictionEntity> entities, CancellationToken cancellationToken = default);
    }
}
