using ExchangeLibrary.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TraidingBot.Exchanges.Binance.Client;

namespace TraidingBot.Exchanges.Binance
{
    /// <summary>
    ///     Binance биржа
    /// </summary>
    internal class BinanceExchange : IExchange
    {
        #region Fields

        private IBinanceClient _client;

        #endregion

        /// <inheritdoc />
        public async Task<double> GetCryptoPrice(string cryptoName, CancellationToken cancellationToken)
        { }
    }
}
