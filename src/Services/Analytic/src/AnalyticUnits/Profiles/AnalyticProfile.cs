using Common.Models;
using ExchangeLibrary;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Профиль анализа (содержит одну определенную логику анализа)
    /// </summary>
    internal class AnalyticProfile : IAnalyticProfile
    {
        #region Fields

        private readonly IExchange _exchange;

        #endregion

        #region .ctor

        /// <inheritdoc cref="AnalyticProfile"/>
        public AnalyticProfile(IExchange exchange)
        {
            _exchange = exchange;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public Task<AnalyticResultModel> AnalyzeAsync(string name, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
