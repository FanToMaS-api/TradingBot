using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories
{
    /// <summary>
    ///     Общее хранилище
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <inheritdoc cref="IHotUnitOfWork"/>
        IHotUnitOfWork HotUnitOfWork { get; }

        /// <inheritdoc cref="IHotUnitOfWork"/>
        IColdUnitOfWork ColdUnitOfWork { get; }

        /// <summary>
        ///     Сохранить данные
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
