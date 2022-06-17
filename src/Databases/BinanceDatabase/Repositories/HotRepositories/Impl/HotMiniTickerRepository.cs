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
        public async Task<HotMiniTickerEntity[]> GetArrayAsync(string pair, CancellationToken cancellationToken, int neededCount = 2000)
            =>
            neededCount >= 2500
                ? throw new Exception($"{nameof(neededCount)} should be less than 2500")
                : await CreateQuery()
                    .Where(_ => _.Pair == pair)
                    .OrderByDescending(_ => _.ReceivedTime)
                    .Take(neededCount)
                    .OrderBy(_ => _.ReceivedTime)
                    .ToArrayAsync(cancellationToken);

        /// <inheritdoc />
        public async Task<int> RemoveUntilAsync(DateTime until, CancellationToken cancellationToken)
        {
            var allCount = await _appDbContext.HotMiniTickers
                .AsNoTracking()
                .CountAsync(_ => _.ReceivedTime < until, cancellationToken);
            var entitiesNumberInOneDeletion = 250; // обусловлено экономией RAM 
            var pagesCount = (int)Math.Ceiling(allCount / (double)entitiesNumberInOneDeletion);
            var removedCount = 0;
            for (var page = 0; page < pagesCount; page++)
            {
                var entitiesToRemove = _appDbContext.HotMiniTickers.AsNoTracking()
                    .Where(_ => _.ReceivedTime < until)
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
