using BinanceDatabase.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories.ColdRepositories.Impl
{
    /// <summary>
    ///     Репозиторий усеченных данных о торговле тикером
    /// </summary>
    internal class MiniTickerRepository : IMiniTickerRepository
    {
        #region Fields

        private readonly AppDbContext _appDbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="MiniTickerRepository"/>
        public MiniTickerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<MiniTickerEntity> CreateQuery() => _appDbContext.MiniTickers.AsQueryable();

        /// <inheritdoc />
        public async Task AddAsync(MiniTickerEntity entity, CancellationToken cancellationToken = default)
            => await _appDbContext.MiniTickers.AddAsync(entity, cancellationToken);

        /// <inheritdoc />
        public async Task AddRangeAsync(IEnumerable<MiniTickerEntity> miniTickerEntities, CancellationToken cancellationToken = default)
            => await _appDbContext.MiniTickers.AddRangeAsync(miniTickerEntities, cancellationToken);

        #endregion
    }
}
