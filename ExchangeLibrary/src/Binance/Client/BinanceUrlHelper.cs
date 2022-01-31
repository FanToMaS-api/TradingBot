using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.Client
{
    /// <summary>
    ///     Создает запросы к Binance
    /// </summary>
    internal class BinanceUrlHelper
    {
        #region Fields

        private const string DEFAULT_SPOT_BASE_URL = "https://api.binance.com";

        #endregion

        #region Public methods

        /// <summary>
        ///     Дополняет url заданными параметрами
        /// </summary>
        public string SetUrlParameters(string url, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (url.Length != 0)
                {
                    url += $"&{parameter.Key}={parameter.Value}";
                    continue;
                }

                url += $"{parameter.Key}={parameter.Value}";
            }

            return url;
        }

        #endregion
    }
}
