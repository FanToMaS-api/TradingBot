using BinanceExchange.Client.Helpers;
using Xunit;

namespace BinanceExchangeTests.BinanceTests
{
    /// <summary>
    ///     Тестирует <see cref="BinanceUrlHelper"/>
    /// </summary>
    public class BinanceUrlHelperTests
    {
        /// <summary>
        ///     Тест подписи источника
        /// </summary>
        [Fact(DisplayName = "Sign data Test")]
        public void Sing_Test()
        {
            var source = "Test Data For Sign";
            var key = "Me Key";
            var expectedResult = "a028fb04a2f9912ce9b56c5b21569ce7e7d9be0b1c816edc18f6119acd31122c";
            var actualResult = BinanceUrlHelper.Sign(source, key);

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
