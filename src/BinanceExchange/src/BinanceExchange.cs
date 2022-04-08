using ExchangeLibrary;

namespace BinanceExchange
{
    /// <summary>
    ///     Binance биржа
    /// </summary>
    internal class BinanceExchange : IExchange
    {
        #region .ctor

        /// <inheritdoc cref="BinanceExchange"/>
        public BinanceExchange(
            IWallet wallet,
            IMarketdata marketdata,
            ISpotTrade spotTrade,
            IMarketdataStreams marketdataStreams)
        {
            Wallet = wallet;
            Marketdata = marketdata;
            MarketdataStreams = marketdataStreams;
            SpotTrade = spotTrade;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IWallet Wallet { get; }

        /// <inheritdoc />
        public IMarketdata Marketdata {get;}

        /// <inheritdoc />
        public IMarketdataStreams MarketdataStreams {get;}

        /// <inheritdoc />
        public ISpotTrade SpotTrade {get;}

        #endregion
    }
}
