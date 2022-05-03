using Microsoft.Extensions.DependencyInjection;
using TelegramServiceDatabase.Repositories;

namespace TelegramServiceDatabase
{
    /// <inheritdoc cref="ITelegramDatabaseFactory"/>
    internal class TelegramDatabaseFactory : ITelegramDatabaseFactory
    {
        #region Fields

        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramDatabaseFactory"/>
        public TelegramDatabaseFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public ITelegramDbUnitOfWork CreateScopeDatabase()
        {
            return _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITelegramDbUnitOfWork>();
        }

        #endregion
    }
}
