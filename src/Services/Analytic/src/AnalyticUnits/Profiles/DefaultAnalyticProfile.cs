using Analytic.Models;
using ExchangeLibrary;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Протейший анализатор покупки AveragePrice > BestAskPrice
    /// </summary>
    public class DefaultAnalyticProfile : IAnalyticProfile
    {
        #region .ctor

        /// <inheritdoc cref="DefaultAnalyticProfile"/>
        public DefaultAnalyticProfile(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IServiceScopeFactory serviceScopeFactory,
            InfoModel model,
            CancellationToken cancellationToken)
        {

            using var scope = serviceScopeFactory.CreateScope();
            var exchange = scope.ServiceProvider.GetRequiredService<IExchange>();
            var averagePrice = await exchange.Marketdata.GetAveragePriceAsync(model.TradeObjectName, cancellationToken);
            var bestAskPrice = (await exchange.Marketdata.GetBestSymbolOrdersAsync(model.TradeObjectName, cancellationToken))
                .OrderByDescending(_ => _.AskPrice)
                .FirstOrDefault()?.AskPrice ?? double.MaxValue;
            var isSuccessfulAnalyze = bestAskPrice < averagePrice;
            if (!isSuccessfulAnalyze)
            {
                return (false, null);
            }

            var resultModel = new AnalyticResultModel
            {
                TradeObjectName = model.TradeObjectName,
                RecommendedPurchasePrice = bestAskPrice,
            };

            return (isSuccessfulAnalyze, resultModel);
        }

        #endregion
    }
}
