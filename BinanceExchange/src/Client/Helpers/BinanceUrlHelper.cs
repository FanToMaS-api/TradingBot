using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BinanceExchange.Client.Helpers
{
    /// <summary>
    ///     Создает запросы к Binance
    /// </summary>
    internal static class BinanceUrlHelper
    {
        #region Public methods

        /// <summary>
        ///     Строит запрос к Binance
        /// </summary>
        public static StringBuilder BuildQueryString(Dictionary<string, object> queryParameters, StringBuilder builder)
        {
            foreach (var queryParameter in queryParameters)
            {
                if (!string.IsNullOrWhiteSpace(queryParameter.Value?.ToString()))
                {
                    if (builder.Length > 0)
                    {
                        builder.Append("&");
                    }

                    builder.Append($"{queryParameter.Key}={HttpUtility.UrlEncode(queryParameter.Value.ToString())}");
                }
            }

            return builder;
        }

        /// <summary>
        ///     Подписывает источник
        /// </summary>
        public static string Sign(string source, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var sourceBytes = Encoding.UTF8.GetBytes(source);

                var hash = hmacsha256.ComputeHash(sourceBytes);

                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        #endregion
    }
}
