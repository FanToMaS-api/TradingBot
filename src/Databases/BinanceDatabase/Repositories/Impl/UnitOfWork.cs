using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories
{
    /// <inheritdoc cref="IUnitOfWork"/>
    internal class UnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly AppDbContext _appDbContext;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <inheritdoc cref="IUnitOfWork"/>
        public UnitOfWork(
            AppDbContext dbContext,
            IHotUnitOfWork hotUnitOfWork,
            IColdUnitOfWork coldUnitOfWork)
        {
            _appDbContext = dbContext;
            HotUnitOfWork = hotUnitOfWork;
            ColdUnitOfWork = coldUnitOfWork;

            _appDbContext.Database.BeginTransaction();
        }

        #endregion

        /// <inheritdoc />
        public IHotUnitOfWork HotUnitOfWork { get; init; }

        /// <inheritdoc />
        public IColdUnitOfWork ColdUnitOfWork { get; init; }


        #region Public methods

        /// <inheritdoc />
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _appDbContext.SaveChangesAsync(cancellationToken);

            await (_appDbContext.Database.CurrentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
        }

        #endregion


        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _appDbContext.Database.CurrentTransaction?.Commit();
            _appDbContext.Dispose();
            _isDisposed = true;
        }

        #endregion
    }
}
