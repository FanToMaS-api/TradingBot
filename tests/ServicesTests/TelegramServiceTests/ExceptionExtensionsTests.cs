using Logger;
using System;
using Xunit;

namespace TelegramServiceTests
{
    /// <summary>
    ///     Тестирует <see cref="ExceptionExtensions"/>
    /// </summary>
    public class ExceptionExtensionsTests
    {
        /// <summary>
        ///     Тест получения полного стек трейса
        /// </summary>
        [Fact(DisplayName = "Exception get full message Test")]
        public static void ExceptionGetFullMessage_Test()
        {
            var exception1 = new Exception("1");
            var exception2 = new Exception("2", exception1);
            var expectedText = "2\n1";
            Assert.Equal(expectedText, exception2.GetFullMessage());

            var expectedTextForAggregateEx = "2\n1 (1) (2)\n1\n2";
            var aggregateEx = new AggregateException(expectedText, exception1, exception2);
            Assert.Equal(expectedTextForAggregateEx, aggregateEx.GetFullMessage());
        }
    }
}
