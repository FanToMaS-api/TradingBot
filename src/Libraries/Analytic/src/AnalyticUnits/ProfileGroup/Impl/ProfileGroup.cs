using Analytic.AnalyticUnits;
using Analytic.Models;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits.ProfileGroup.Impl
{
    /// <summary>
    ///     Группа профилей аналитики
    /// </summary>
    public class ProfileGroup : IProfileGroup
    {
        #region Fields

        private readonly ILoggerDecorator _logger;

        #endregion

        #region .ctor

        /// <inheritdoc cref="ProfileGroup"/>
        public ProfileGroup(ILoggerDecorator logger, string name, bool isActive = true)
        {
            _logger = logger;
            Name = name;
            IsActive = isActive;
        }

        #endregion

        #region Implementation of IProfileGroup

        #region Properties

        /// <inheritdoc />
        public List<IAnalyticProfile> AnalyticProfiles { get; } = new();

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsActive { get; private set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IServiceScopeFactory serviceScopeFactory,
            InfoModel model,
            CancellationToken cancellationToken)
        {
            AnalyticResultModel analyticResultModel = null;

            var count = 0;
            var isOneSuccessful = false;
            foreach (var analyticProfile in AnalyticProfiles)
            {
                try
                {
                    var (isSuccessful, resultModel) = await analyticProfile.TryAnalyzeAsync(
                        serviceScopeFactory,
                        model,
                        cancellationToken);
                    if (!isSuccessful)
                    {
                        continue;
                    }

                    // если мы тут, значит анализ прошел впервые успешно
                    if (!isOneSuccessful)
                    {
                        analyticResultModel = resultModel;
                        isOneSuccessful = true;
                        count++;
                        continue;
                    }

                    analyticResultModel.RecommendedPurchasePrice += resultModel.RecommendedPurchasePrice;
                    analyticResultModel.RecommendedSellingPrice += resultModel.RecommendedSellingPrice;
                    count++;
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync(
                        ex,
                        $"Failed to analyze model '{model.TradeObjectName}' with analyticProfile '{analyticProfile.Name}'",
                        cancellationToken: cancellationToken);
                }
            }

            // TODO: Усреднять через список полученных моделей
            if (isOneSuccessful)
            {
                analyticResultModel.RecommendedPurchasePrice /= count == 0 ? 1 : count;
                analyticResultModel.RecommendedSellingPrice /= count == 0 ? 1 : count;
            }

            return (isOneSuccessful, analyticResultModel);
        }

        /// <inheritdoc />
        public void AddAnalyticUnit(IAnalyticProfile unit) => AnalyticProfiles.Add(unit);

        /// <inheritdoc />
        public bool Remove(string name)
        {
            foreach (var unit in AnalyticProfiles)
            {
                if (unit.Name == name)
                {
                    return AnalyticProfiles.Remove(unit);
                }
            }

            return false;
        }

        /// <inheritdoc />
        public void ChangeProfileActivity(bool isActive) => IsActive = isActive;

        #endregion

        #endregion
    }
}
