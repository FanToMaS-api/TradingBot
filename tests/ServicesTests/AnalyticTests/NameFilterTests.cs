using Analytic.Filters;
using Analytic.Models;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует <see cref="NameFilter"/>
    /// </summary>
    public class NameFilterTests
    {
        #region Fields

        private readonly string[] _tickers = { "BTC", "USDT" };
        private readonly InfoModel _infoModel_BTCUSDT = new("BTCUSDT", 15);
        private readonly InfoModel _infoModel_ETHBUSD = new("ETHUSDT", 10);
        private readonly InfoModel _infoModel_GALABNB = new("GALABNB", 10);
        private readonly InfoModel _infoModel_INJBNB = new("INJBNB", 10);

        #endregion

        /// <summary>
        ///     Тест выборки только тех пар, в которых участвуют нужные тикеры
        /// </summary>
        [Fact(DisplayName = "Filter pairs by pair name Test")]
        public void FilterPairsByPairName_Test()
        {
            var filter = new NameFilter(
                "NameFilter",
                _tickers);

            Assert.True(filter.CheckConditions(_infoModel_BTCUSDT));
            Assert.True(filter.CheckConditions(_infoModel_ETHBUSD));
            Assert.False(filter.CheckConditions(_infoModel_GALABNB));
            Assert.False(filter.CheckConditions(_infoModel_INJBNB));
        }
    }
}
