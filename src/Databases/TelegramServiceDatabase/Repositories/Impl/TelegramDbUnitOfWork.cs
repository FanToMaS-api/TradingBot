using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramServiceDatabase.Repositories.Impl
{
    /// <summary>
    ///     Контекст базы данных
    /// </summary>
    public sealed class TelegramDbUnitOfWork : ITelegramDbUnitOfWork
    {
        #region Fields

        private readonly TelegramDbContext _dbContext;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <summary>
        /// .ctor
        /// </summary>
        public TelegramDbUnitOfWork(
            TelegramDbContext dbContext,
            IUserRepository userRepository,
            IUserStateRepository userStateRepository)
        {
            _dbContext = dbContext;
            Users = userRepository;
            UserStates = userStateRepository;

            _dbContext.Database.BeginTransaction();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IUserRepository Users { get; init; }

        /// <inheritdoc />
        public IUserStateRepository UserStates { get; init; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        /// <inheritdoc />
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            await (_dbContext.Database.CurrentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _dbContext.Database.CurrentTransaction?.Commit();
            _dbContext.Dispose();
            _isDisposed = true;
        }

        #endregion
    }
}
