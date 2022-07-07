using BinanceDatabase.Entities;
using Microsoft.EntityFrameworkCore;
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

        #region Implementation of IHotMiniTickerRepository

        /// <inheritdoc />
        public IQueryable<HotMiniTickerEntity> CreateQuery() => _appDbContext.HotMiniTickers.AsQueryable();

        /// <inheritdoc />
        public async Task AddAsync(HotMiniTickerEntity entity, CancellationToken cancellationToken = default)
            => await _appDbContext.HotMiniTickers.AddAsync(entity, cancellationToken);

        /// <inheritdoc />
        public async Task AddRangeAsync(IEnumerable<HotMiniTickerEntity> miniTickerEntities, CancellationToken cancellationToken = default)
            => await _appDbContext.HotMiniTickers.AddRangeAsync(miniTickerEntities, cancellationToken);

        /// <inheritdoc />
        public IEnumerable<HotMiniTickerEntity> GetEntities(
            string pair,
            int? neededCount = null)
        {
            var query = CreateQuery()
                .Where(_ => _.Pair == pair)
                .OrderByDescending(_ => _.ReceivedTime);

            return neededCount.HasValue
                ? query.Take(neededCount.Value).OrderBy(_ => _.ReceivedTime)
                : query.OrderBy(_ => _.ReceivedTime);
        }

        /// <inheritdoc />
        public async Task<int> RemoveUntilAsync(DateTime before, CancellationToken cancellationToken)
        {
            var isNonEnumerated = _appDbContext.HotMiniTickers
                .AsNoTracking()
                .Where(_ => _.ReceivedTime < before)
                .TryGetNonEnumeratedCount(out var allCount);
            var entitiesNumberInOneDeletion = 250; // обусловлено экономией RAM 
            var pagesCount = (int)Math.Ceiling(allCount / (double)entitiesNumberInOneDeletion);
            var removedCount = 0;
            for (var page = 0; page < pagesCount; page++)
            {
                var entitiesToRemove = _appDbContext.HotMiniTickers.AsNoTracking()
                    .Where(_ => _.ReceivedTime < before)
                    .OrderBy(_ => _.ReceivedTime)
                    .Skip(page * entitiesNumberInOneDeletion)
                    .Take(entitiesNumberInOneDeletion);

                _appDbContext.HotMiniTickers.RemoveRange(entitiesToRemove);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                removedCount += entitiesToRemove.Count();
            }

            return removedCount;
        }

        #endregion
    }
}
