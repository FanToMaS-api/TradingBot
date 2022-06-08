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
        ///     Обединяет данные в строку, разделяя их сепаратором
        /// </summary>
        internal static string JoinToString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
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
