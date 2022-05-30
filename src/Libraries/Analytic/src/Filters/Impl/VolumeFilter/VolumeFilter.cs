using Analytic.Models;
using ExchangeLibrary;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.Filters
{
    /// <summary>
    ///     Фильтр объемов спроса и предложения
    /// </summary>
    public class VolumeFilter : IFilter
    {
        #region .ctor

        /// <summary>
        ///     Фильтр объемов спроса и предложения
        /// </summary>
        /// <param name="filterName"> Название фильтра </param>
        /// <param name="volumeType"> Тип объемов для фильтрации </param>
        /// <param name="volumeComparisonType"> Тип фильтра </param>
        /// <param name="percentDeviation"> Отклонение для объемов при дефолтной фильтрации </param>
        /// <param name="orderNumber"> 
        ///     Необходимое кол-во ордеров <br />
        ///     Возможные значения: 5, 10, 20, 50, 100, 500, 1000, 5000
        /// </param>
        public VolumeFilter(
            string filterName,
            VolumeType volumeType = VolumeType.Bid,
            VolumeComparisonType volumeComparisonType = VolumeComparisonType.GreaterThan,
            double percentDeviation = 0.05,
            int orderNumber = 1000)
        {
            FilterName = filterName;
            VolumeType = volumeType;
            VolumeComparisonType = volumeComparisonType;
            PercentDeviation = percentDeviation;
            OrderNumber = orderNumber;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string FilterName { get; }

        /// <summary>
        ///     Тип сравнения объемов
        /// </summary>
        public VolumeComparisonType VolumeComparisonType { get; }

        /// <summary>
        ///     Тип фильтруемых объемов
        /// </summary>
        public VolumeType VolumeType { get; }

        /// <summary>
        ///     Базовое отклонение объема спроса от объема предложения
        /// </summary>
        public double PercentDeviation { get; }

        /// <summary>
        ///     Кол-во ордеров, получаемых из книги ордеров при запрсое к бинансу
        /// </summary>
        /// <remarks>
        ///     Возможные значения: 5, 10, 20, 50, 100, 500, 1000, 5000
        /// </remarks>
        public int OrderNumber { get; }

        /// <inheritdoc />
        public FilterType Type => FilterType.VolumeFilter;

        #endregion

        #region Public methods

        /// <inheritdoc />
        /// <remarks>
        ///     Данный метод также получает с бинанса значение <see cref="InfoModel.BidVolume"/> и <see cref="InfoModel.AskVolume"/>
        /// </remarks>
        public async Task<bool> CheckConditionsAsync(
            IServiceScopeFactory serviceScopeFactory,
            InfoModel model,
            CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var exchange = scope.ServiceProvider.GetService<IExchange>()
                ?? throw new InvalidOperationException($"{nameof(IExchange)} not registered!");
            var extendedModel = await exchange.Marketdata.GetOrderBookAsync(model.TradeObjectName, OrderNumber, cancellationToken);
            model.BidVolume = extendedModel.Bids.Sum(_ => _.Quantity);
            model.AskVolume = extendedModel.Asks.Sum(_ => _.Quantity);

            return VolumeComparisonType switch
            {
                VolumeComparisonType.GreaterThan => IsSatisfiesCondition(model, (x, y) => x > y * (1 + PercentDeviation)),
                VolumeComparisonType.LessThan => IsSatisfiesCondition(model, (x, y) => x <= y * (1 + PercentDeviation)),
                _ => throw new NotImplementedException(),
            };
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверка условия в соответствии с типом фильтруемых объемов
        /// </summary>
        /// <param name="filterFunc"> Функция для фильтрации </param>
        private bool IsSatisfiesCondition(InfoModel model, Func<double, double, bool> filterFunc) =>
            VolumeType switch
            {
                VolumeType.Bid => filterFunc(model.BidVolume, model.AskVolume),
                VolumeType.Ask => filterFunc(model.AskVolume, model.BidVolume),
                _ => throw new NotImplementedException(),
            };

        #endregion
    }
}
