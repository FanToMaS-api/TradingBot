using Logger;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelegramServiceDatabase.Entities;

namespace TelegramServiceDatabase.Repositories.Impl
{
    /// <summary>
    ///     Репозиторий состояний пользователя
    /// </summary>
    internal class UserStateRepository : IUserStateRepository
    {
        #region Fields

        private readonly static ILoggerDecorator Log = LoggerManager.CreateDefaultLogger();
        private readonly TelegramDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="UserStateRepository"/>
        public UserStateRepository(TelegramDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<UserStateEntity> CreateQuery() => _dbContext.UsersStates.AsQueryable();

        /// <inheritdoc />
        public async Task<UserStateEntity> GetAsync(long userId, CancellationToken cancellationToken = default)
        {
            var userState = await _dbContext.UsersStates
                .Where(_ => _.UserId == userId)
                .Include(_ => _.User)
                .FirstOrDefaultAsync(cancellationToken);
            if (userState is null)
            {
                await Log.ErrorAsync($"Cannot find state of user with id: {userId}", cancellationToken: cancellationToken);
            }

            return userState;
        }

        /// <inheritdoc />
        public async Task<UserStateEntity> CreateAsync(Action<UserStateEntity> action, CancellationToken cancellationToken = default)
        {
            var userState = new UserStateEntity();
            action(userState);

            var conflictState = await _dbContext.UsersStates
                .Where(_ => _.UserId == userState.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictState is not null)
            {
                await Log.ErrorAsync("Cannot add user state, because it already exists", cancellationToken: cancellationToken);
                return userState;
            }

            await _dbContext.UsersStates.AddAsync(userState, cancellationToken);

            return userState;
        }

        /// <inheritdoc />
        public async Task<UserStateEntity> UpdateAsync(long id, Action<UserStateEntity> action, CancellationToken cancellationToken = default)
        {
            var userState = await _dbContext.UsersStates
                .Where(_ => _.Id == id)
                .Include(_ => _.User)
                .FirstOrDefaultAsync(cancellationToken);
            if (userState == null)
            {
                await Log.ErrorAsync($"Cannot find userState with id: {id}", cancellationToken: cancellationToken);

                return null;
            }

            action(userState);

            var conflictingState = await _dbContext.UsersStates
                .Where(_ => _.UserId == userState.UserId && _.Id != id)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictingState != null)
            {
                await Log.ErrorAsync("Cannot add user state, because it already exists", cancellationToken: cancellationToken);

                return userState;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return userState;
        }

        #endregion
    }
}
