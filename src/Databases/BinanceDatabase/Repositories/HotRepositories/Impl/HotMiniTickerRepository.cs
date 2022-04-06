using BinanceDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories.HotRepositories.Impl
{
    /// <summary>
    ///     Репозиторий усеченных данных о торговле тикером
    /// </summary>
    internal class HotMiniTickerRepository : IHotMiniTickerRepository
    {
        #region Fields

        private readonly AppDbContext _appDbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="HotMiniTickerRepository"/>
        public HotMiniTickerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<HotMiniTickerEntity> CreateQuery() => _appDbContext.HotMiniTickers.AsQueryable();

        /// <inheritdoc />
        public async Task AddAsync(HotMiniTickerEntity entity, CancellationToken cancellationToken = default)
            => await _appDbContext.HotMiniTickers.AddAsync(entity, cancellationToken);

        /// <inheritdoc />
        public async Task AddRangeAsync(IEnumerable<HotMiniTickerEntity> miniTickerEntities, CancellationToken cancellationToken = default)
            => await _appDbContext.HotMiniTickers.AddRangeAsync(miniTickerEntities, cancellationToken);

        /// <inheritdoc />
        public void RemoveUntil(DateTime until)
        {
            var result = CreateQuery().Where(_ => _.ReceivedTime < until);

            _appDbContext.HotMiniTickers.RemoveRange(result);
        }

        #endregion
    }
}
