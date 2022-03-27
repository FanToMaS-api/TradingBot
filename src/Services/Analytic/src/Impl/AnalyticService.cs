using ExchangeLibrary;

namespace Analytic.Impl
{
    /// <inheritdoc cref="IAnalyticService"/>
    internal class AnalyticService : IAnalyticService
    {
        #region Fields

        private readonly IExchange _exchange;

        #endregion

        #region .ctor

        /// <inheritdoc cref="AnalyticService"/>
        public AnalyticService(IExchange exchange)
        {
            _exchange = exchange;
        }

        #endregion
    }
}
