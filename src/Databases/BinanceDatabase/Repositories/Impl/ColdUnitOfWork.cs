using BinanceDatabase.Repositories.ColdRepositories;

namespace BinanceDatabase.Repositories
{
    /// <inheritdoc cref="IColdUnitOfWork"/>
    internal class ColdUnitOfWork : IColdUnitOfWork
    {
        #region .ctor

        /// <inheritdoc cref="ColdUnitOfWork"/>
        public ColdUnitOfWork(IMiniTickerRepository miniTickerRepository, IPredictionRepository predictionRepository)
        {
            MiniTickers = miniTickerRepository;
            Predictions = predictionRepository;
        }

        #endregion

        #region Repositories

        /// <inheritdoc />
        public IMiniTickerRepository MiniTickers { get; }

        /// <inheritdoc />
        public IPredictionRepository Predictions { get; }

        #endregion
    }
}
