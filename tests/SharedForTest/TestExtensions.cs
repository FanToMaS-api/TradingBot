using Logger;
using System;
using Xunit;

namespace SharedForTest
{
    /// <summary>
    ///     Содержит расширения для тестирования
    /// </summary>
    public static class TestExtensions
    {
        /// <summary>
        ///     Проверка утверждений
        /// </summary>
        public static void CheckingAssertions<T1, T2>(T1 expected, T2 actual)
        {
            var properties = expected.GetType().GetProperties();
            foreach (var property in properties)
            {
                Assert.Equal(property.GetValue(expected), property.GetValue(actual));
            }
        }
    }
}
