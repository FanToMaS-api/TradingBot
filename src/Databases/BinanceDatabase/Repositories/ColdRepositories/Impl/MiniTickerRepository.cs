using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using Microsoft.EntityFrameworkCore;
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

        #region Implementation IMiniTickerRepository

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
        public async Task<double> GetPricePercentDeviationSumAsync(
            string pair,
            AggregateDataIntervalType interval,
            int count,
            CancellationToken cancellationToken)
            => 
            await CreateQuery()
                .Where(_ => _.ShortName == pair && _.AggregateDataInterval == interval)
                .OrderByDescending(_ => _.EventTime)
                .Select(_ => _.PriceDeviationPercent)
                .Take(count)
                .SumAsync(cancellationToken);

        #endregion
    }
}
