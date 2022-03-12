using RichardSzalay.MockHttp;
using System;
using System.IO;
using System.Net.Http;

namespace BinanceExchangeTests.BinanceTests.EndpointSendersTests
{
    /// <summary>
    ///     Содержит тестовые методы общие для многих тестов
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        ///     Создает мок HttpClient
        /// </summary>
        /// <param name="filePath"> Путь к файлу с json-response </param>
        public static HttpClient CreateMockHttpClient(string url, string filePath)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, filePath);
            var json = File.ReadAllText(path);
            using var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url).Respond("application/json", json);

            return new HttpClient(mockHttp);
        }
    }
}
