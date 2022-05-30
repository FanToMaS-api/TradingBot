using BinanceDatabase.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories.ColdRepositories.Impl
{
    /// <inheritdoc cref="IPredictionRepository"/>
    internal class PredictionRepository : IPredictionRepository
    {
        #region Fields

        private readonly AppDbContext _appDbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="PredictionRepository"/>
        public PredictionRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        #endregion

        #region Implementation IPredictionRepository

        /// <inheritdoc />
        public IQueryable<PredictionEntity> CreateQuery() => _appDbContext.Predictions.AsQueryable();

        /// <inheritdoc />
        public async Task AddAsync(PredictionEntity entity, CancellationToken cancellationToken = default)
            => await _appDbContext.Predictions.AddAsync(entity, cancellationToken);

        /// <inheritdoc />
        public async Task AddRangeAsync(IEnumerable<PredictionEntity> entities, CancellationToken cancellationToken = default)
            => await _appDbContext.Predictions.AddRangeAsync(entities, cancellationToken);

        #endregion
    }
}
