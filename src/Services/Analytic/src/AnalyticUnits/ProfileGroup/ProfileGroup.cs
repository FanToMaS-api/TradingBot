using Common.Models;
using ExchangeLibrary;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Группа профилей
    /// </summary>
    internal class ProfileGroup : IProfileGroup
    {
        #region Fields

        #endregion

        #region .ctor

        /// <inheritdoc cref="ProfileGroup"/>
        public ProfileGroup()
        {
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public List<IAnalyticUnit> AnalyticUnits { get; } = new();

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<AnalyticResultModel> AnalyzeAsync(string name, CancellationToken cancellationToken)
        {
            var result = new AnalyticResultModel()
            {
                Symbol = name,
            };
            foreach (var unit in AnalyticUnits)
            {
                var resultModel = await unit.AnalyzeAsync(name, cancellationToken);
                result.RecommendedPurchasePrice += resultModel.RecommendedPurchasePrice;
                result.RecommendedSellingPrice += resultModel.RecommendedSellingPrice;
            }

            result.RecommendedPurchasePrice /= AnalyticUnits.Count;
            result.RecommendedSellingPrice /= AnalyticUnits.Count;
            return result;
        }

        /// <inheritdoc />
            
        public void AddAnalyticUnit(IAnalyticUnit unit) => AnalyticUnits.Add(unit);

        /// <inheritdoc />
        public void RemoveAnaliticUnit(IAnalyticUnit unit) => AnalyticUnits.Remove(unit);

        #endregion
    }
}
