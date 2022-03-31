using AutoMapper;
using BinanceExchange.Client;
using BinanceExchange.Client.Impl;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Impl;
using Common.JsonConvertWrapper;
using ExchangeLibrary;
using NLog;
using Redis;
using System;
using System.Net.Http;

namespace BinanceExchange
{
    /// <summary>
    ///     Binance биржа
    /// </summary>
    internal class BinanceExchange : IExchange, IDisposable
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IBinanceClient _client;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private IWallet _wallet;
        private IMarketdata _marketdata;
        private IMarketdataStreams _marketdataStreams;
        private ISpotTrade _spotTrade;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceExchange"/>
        public BinanceExchange(BinanceExchangeOptions exchangeOptions, IRedisDatabase redisDatabase)
        {
            _httpClient = new();
            _redisDatabase = redisDatabase;
            _client = new BinanceClient(_httpClient, exchangeOptions.ApiKey, exchangeOptions.SecretKey);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BinanceMapperProfile>();
            });
            _mapper = new Mapper(config);
        }

        /// <inheritdoc cref="BinanceExchange"/>
        public BinanceExchange(
            IHttpClientFactory httpClientFactory, // TODO: https://markshevchenko.pro/2019/01/23/how-to-use-ihttpclientfactory/
            IBinanceClient binanceClient,
            IRedisDatabase redisDatabase,
            IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _client = binanceClient;
            _redisDatabase = redisDatabase;
            _mapper = mapper;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IWallet Wallet => _wallet ??= new Wallet(new WalletSender(_client), _redisDatabase, _mapper);

        /// <inheritdoc />
        public IMarketdata Marketdata => _marketdata ??= new Marketdata(new MarketdataSender(_client), _redisDatabase, _mapper);

        /// <inheritdoc />
        public IMarketdataStreams MarketdataStreams => _marketdataStreams ??= new MarketdataStreams(_mapper);

        /// <inheritdoc />
        public ISpotTrade SpotTrade => _spotTrade ??= new SpotTrade(new SpotTradeSender(_client), _redisDatabase, _mapper);

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _redisDatabase?.Dispose();
            _httpClient.Dispose();
        }

        #endregion
    }
}
