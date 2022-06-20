using Analytic.Filters;
using Analytic.Models;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnalyticTests.FilterTests
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
        public async Task FilterPairsByPairName_Test()
        {
            var filter = new NameFilter(
                "NameFilter",
                _tickers);

            Assert.True(await filter.CheckConditionsAsync(null, _infoModel_BTCUSDT, CancellationToken.None));
            Assert.True(await filter.CheckConditionsAsync(null, _infoModel_ETHBUSD, CancellationToken.None));
            Assert.False(await filter.CheckConditionsAsync(null, _infoModel_GALABNB, CancellationToken.None));
            Assert.False(await filter.CheckConditionsAsync(null, _infoModel_INJBNB, CancellationToken.None));
        }
    }
}
