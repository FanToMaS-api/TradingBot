using Analytic.Models;
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
        public AnalyticProfile(IExchange exchange, string name)
        {
            _exchange = exchange;
            Name = name;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(InfoModel model, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool Remove(string name) => false;

        #endregion
    }
}
