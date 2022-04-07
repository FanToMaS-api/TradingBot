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

        // IHttpClientFactory httpClientFactory, // TODO: https://markshevchenko.pro/2019/01/23/how-to-use-ihttpclientfactory/

        #endregion

        #region Properties

        /// <inheritdoc />
        public IWallet Wallet { get; } // _wallet ??= new Wallet(new WalletSender(_client), _redisDatabase, _mapper);

        /// <inheritdoc />
        public IMarketdata Marketdata {get;} // _marketdata ??= new Marketdata(new MarketdataSender(_client), _redisDatabase, _mapper);

        /// <inheritdoc />
        public IMarketdataStreams MarketdataStreams {get;} // _marketdataStreams ??= new MarketdataStreams(_mapper);

        /// <inheritdoc />
        public ISpotTrade SpotTrade {get;} // _spotTrade ??= new SpotTrade(new SpotTradeSender(_client), _redisDatabase, _mapper);

        #endregion
    }
}
