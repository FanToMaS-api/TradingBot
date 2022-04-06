using BinanceDatabase.Repositories.HotRepositories;

namespace BinanceDatabase.Repositories
{
    /// <inheritdoc cref="IColdUnitOfWork"/>
    internal class HotUnitOfWork : IHotUnitOfWork
    {
        #region .ctor

        /// <inheritdoc cref="HotUnitOfWork"/>
        public HotUnitOfWork(IHotMiniTickerRepository hotMiniTickerRepository)
        {
            HotMiniTickers = hotMiniTickerRepository;
        }

        #endregion

        #region Repositories

        /// <inheritdoc />
        public IHotMiniTickerRepository HotMiniTickers { get; }

        #endregion
    }
}
