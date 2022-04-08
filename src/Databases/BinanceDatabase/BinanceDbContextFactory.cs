using BinanceDatabase.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceDatabase
{
    /// <inheritdoc cref="IBinanceDbContextFactory"/>
    internal class BinanceDbContextFactory : IBinanceDbContextFactory
    {
        #region Fields

        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceDbContextFactory"/>
        public BinanceDbContextFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public IUnitOfWork CreateScopeDatabase()
        {
            return _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();
        }

        #endregion

    }
}
