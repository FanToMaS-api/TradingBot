using BinanceDatabase.Repositories.ColdRepositories;

namespace BinanceDatabase.Repositories
{
    /// <inheritdoc cref="IColdUnitOfWork"/>
    internal class ColdUnitOfWork : IColdUnitOfWork
    {
        #region .ctor

        /// <inheritdoc cref="ColdUnitOfWork"/>
        public ColdUnitOfWork(IMiniTickerRepository miniTickerRepository)
        {
            MiniTickers = miniTickerRepository;
        }

        #endregion

        #region Repositories

        /// <inheritdoc />
        public IMiniTickerRepository MiniTickers { get; }

        #endregion
    }
}
