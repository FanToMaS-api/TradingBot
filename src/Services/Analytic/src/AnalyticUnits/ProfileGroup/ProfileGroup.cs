using Analytic.Models;
using ExchangeLibrary;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Группа профилей аналитики
    /// </summary>
    public class ProfileGroup : IProfileGroup
    {
        #region .ctor

        /// <inheritdoc cref="ProfileGroup"/>
        public ProfileGroup(string name, bool isActive = true)
        {
            Name = name;
            IsActive = isActive;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public List<IAnalyticProfile> AnalyticUnits { get; } = new();

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsActive { get; private set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IExchange exchange,
            InfoModel model,
            CancellationToken cancellationToken)
        {
            var analyticResultModel = new AnalyticResultModel()
            {
                TradeObjectName = model.TradeObjectName,
            };

            var count = 0;
            var isOneSuccessful = false;
            foreach (var unit in AnalyticUnits)
            {
                var (isSuccessful, resultModel) = await unit.TryAnalyzeAsync(exchange, model, cancellationToken);
                if (isSuccessful)
                {
                    isOneSuccessful = true;
                    analyticResultModel.RecommendedPurchasePrice += resultModel.RecommendedPurchasePrice;
                    count++;
                }
            }

            analyticResultModel.RecommendedPurchasePrice /= count == 0 ? 0 : count;
            return (isOneSuccessful, analyticResultModel);
        }

        /// <inheritdoc />

        public void AddAnalyticUnit(IAnalyticProfile unit) => AnalyticUnits.Add(unit);

        /// <inheritdoc />
        public bool Remove(string name)
        {
            foreach (var unit in AnalyticUnits)
            {
                if (unit.Name == name)
                {
                    return AnalyticUnits.Remove(unit);
                }
            }

            return false;
        }

        /// <inheritdoc />
        public void ChangeProfileActivity(bool isActive) => IsActive = isActive;

        #endregion
    }
}
