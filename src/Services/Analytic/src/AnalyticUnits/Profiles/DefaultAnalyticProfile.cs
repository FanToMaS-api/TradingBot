using Analytic.Models;
using ExchangeLibrary;
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
            IExchange exchange,
            InfoModel model,
            CancellationToken cancellationToken)
        {
            var averagePrice = await exchange.GetAveragePriceAsync(model.TradeObjectName, cancellationToken);
            var bestAskPrice = (await exchange.GetBestSymbolOrdersAsync(model.TradeObjectName, cancellationToken))
                .OrderByDescending(_ => _.AskPrice)
                .FirstOrDefault()?.AskPrice ?? double.MaxValue;
            var isSuccessfulAnalyze = bestAskPrice < averagePrice;
            var resultModel = new AnalyticResultModel
            {
                TradeObjectName = model.TradeObjectName,
                RecommendedPurchasePrice = bestAskPrice,
            };

            return (isSuccessfulAnalyze, resultModel);
        }

        /// <inheritdoc />
        public bool Remove(string name) => false;

        #endregion
    }
}
