using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using Common.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
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

        #region Implementation of IMiniTickerRepository

        /// <inheritdoc />
        public IQueryable<MiniTickerEntity> CreateQuery() => _appDbContext.MiniTickers.AsQueryable();

        /// <inheritdoc />
        public async Task AddAsync(MiniTickerEntity entity, CancellationToken cancellationToken = default)
            => await _appDbContext.MiniTickers.AddAsync(entity, cancellationToken);

        /// <inheritdoc />
        public async Task AddRangeAsync(
            IEnumerable<MiniTickerEntity> miniTickerEntities,
            CancellationToken cancellationToken = default)
            => await _appDbContext.MiniTickers.AddRangeAsync(miniTickerEntities, cancellationToken);

        /// <inheritdoc />
        public void RemoveRange(IEnumerable<MiniTickerEntity> entities) => _appDbContext.RemoveRange(entities);

        /// <inheritdoc />
        public IEnumerable<MiniTickerEntity> GetEntities(
            string pair,
            AggregateDataIntervalType aggregateDataInterval = AggregateDataIntervalType.Default,
            int? neededCount = null)
        {
            var query = CreateQuery()
                .Where(_ => _.ShortName == pair && _.AggregateDataInterval == aggregateDataInterval)
                .OrderByDescending(_ => _.EventTime);

            return neededCount.HasValue
                ? query.Take(neededCount.Value).OrderBy(_ => _.EventTime)
                : query.OrderBy(_ => _.EventTime);
        }

        /// <inheritdoc />
        public async Task<double> GetPricePercentDeviationAsync(
            string pair,
            AggregateDataIntervalType interval,
            int count,
            CancellationToken cancellationToken)
        {
            var query = CreateQuery()
                .Where(_ => _.ShortName == pair && _.AggregateDataInterval == interval)
                .OrderByDescending(_ => _.EventTime)
                .Select(_ => _.PriceDeviationPercent)
                .Take(count);

            Assert.True(await query.AnyAsync());

            var oldPrice = await query.FirstOrDefaultAsync();
            Assert.True(oldPrice > 0);

            var newPrice = await query.LastOrDefaultAsync();
            Assert.True(newPrice > 0);

            return CommonHelper.GetPercentDeviation(oldPrice, newPrice);
        }


        /// <inheritdoc />
        public async Task<int> RemoveUntilAsync(DateTime before, CancellationToken cancellationToken)
        {
            var isNonEnumerated = _appDbContext.MiniTickers
                .AsNoTracking()
                .Where(_ => _.EventTime < before)
                .TryGetNonEnumeratedCount(out var allCount);
            var entitiesNumberInOneDeletion = 250; // обусловлено экономией RAM 
            var pagesCount = (int)Math.Ceiling(allCount / (double)entitiesNumberInOneDeletion);
            var removedCount = 0;
            for (var page = 0; page < pagesCount; page++)
            {
                var entitiesToRemove = _appDbContext.MiniTickers.AsNoTracking()
                    .Where(_ => _.EventTime < before)
                    .OrderBy(_ => _.EventTime)
                    .Skip(page * entitiesNumberInOneDeletion)
                    .Take(entitiesNumberInOneDeletion);

                _appDbContext.MiniTickers.RemoveRange(entitiesToRemove);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                removedCount += entitiesToRemove.Count();
            }

            return removedCount;
        }

        #endregion
    }
}
