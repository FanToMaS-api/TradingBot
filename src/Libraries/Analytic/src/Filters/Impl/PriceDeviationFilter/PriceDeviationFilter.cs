using Analytic.Filters.Enums;
using Analytic.Models;
using BinanceDatabase;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.Filters
{
    /// <summary>
    ///     Фильтр цен
    /// </summary>
    public class BinancePriceDeviationFilter : IFilter
    {
        #region .ctor

        /// <summary>
        ///     Фильтр цен
        /// </summary>
        /// <param name="filterName"> Название фильтра </param>
        /// <param name="interval"> Определяет с каким интервалом идет подсчет отклонений </param>
        /// <param name="comparisonType"> Тип фильтра цен </param>
        /// <param name="limit"> Ограничение </param>
        /// <param name="timeframeNumber"> Кол-во таймфреймов участвующих в анализе </param>
        public BinancePriceDeviationFilter(
            string filterName,
            AggregateDataIntervalType interval,
            ComparisonType comparisonType,
            double limit,
            int timeframeNumber = 5)
        {
            Name = filterName;
            Interval = interval;
            ComparisonType = comparisonType;
            Limit = limit;
            TimeframeNumber = timeframeNumber;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Тип сравнения
        /// </summary>
        public ComparisonType ComparisonType { get; }

        /// <summary>
        ///     Ограничение на отклонение цены
        /// </summary>
        public double Limit { get; }

        /// <summary>
        ///     Определяет с каким интервалом идет подсчет отклонений
        /// </summary>
        public AggregateDataIntervalType Interval { get; }

        /// <inheritdoc />
        public FilterType Type => FilterType.PriceDeviationFilter;

        /// <summary>
        ///     Кол-во таймфреймов, участвующих в анализе
        /// </summary>
        public int TimeframeNumber { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        /// <remarks>
        ///     Данный метод также вычисляет значение <see cref="InfoModel.PricePercentDeviation"/>
        /// </remarks>
        public async Task<bool> CheckConditionsAsync(
            IServiceScopeFactory serviceScopeFactory,
            InfoModel model,
            CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var databaseFactory = scope.ServiceProvider.GetService<IBinanceDbContextFactory>()
                ?? throw new InvalidOperationException($"{nameof(IBinanceDbContextFactory)} not registered!");
            using var database = databaseFactory.CreateScopeDatabase();

            try
            {
                model.PricePercentDeviation = await database.ColdUnitOfWork.MiniTickers
                    .GetPricePercentDeviationAsync(
                        model.TradeObjectName,
                        Interval.CastToBinanceDataAggregateType(),
                        TimeframeNumber,
                        cancellationToken);
            }
            catch (Exception _)
            {
                return false;
            }

            return ComparisonType switch
            {
                ComparisonType.GreaterThan => model.PricePercentDeviation > Limit,
                ComparisonType.LessThan => model.PricePercentDeviation < Limit,
                ComparisonType.Equal => model.PricePercentDeviation == Limit,
                _ => throw new NotImplementedException(),
            };
        }

        #endregion
    }
}
