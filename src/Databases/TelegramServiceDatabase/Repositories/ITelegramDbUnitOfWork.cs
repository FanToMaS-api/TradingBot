using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramServiceDatabase.Repositories
{
    /// <summary>
    ///     Удиница работы с базой данных пользователей телеграмма
    /// </summary>
    public interface ITelegramDbUnitOfWork : IDisposable
    {
        #region Properties

        /// <inheritdoc cref="IUserRepository"/>
        public IUserRepository Users { get; }

        /// <inheritdoc cref="IUserStateRepository"/>
        public IUserStateRepository UserStates { get; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Сохранить контекст БД
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
