using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories
{
    /// <summary>
    ///     Базовый репозиторий
    /// </summary>
    public interface IRepositoryBase<T>
    {
        /// <summary>
        ///     Создать запрос к таблице
        /// </summary>
        IQueryable<T> CreateQuery();

        /// <summary>
        ///     Добавить сущность
        /// </summary>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
    }
}
