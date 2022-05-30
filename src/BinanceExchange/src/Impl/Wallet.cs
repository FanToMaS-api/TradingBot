using AutoMapper;
using BinanceExchange.EndpointSenders;
using BinanceExchange.Exceptions;
using BinanceExchange.Models;
using BinanceExchange.RedisRateLimits;
using BinanceExchange.RequestWeights;
using Common.Models;
using ExchangeLibrary;
using NLog;
using Redis;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.Impl
{
    /// <inheritdoc cref="IWallet"/>
    internal class Wallet : IWallet
    {
        #region Private methods

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly WalletRequestWeightStorage _weightStorage = new();
        private readonly IWalletSender _walletSender;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IMapper _mapper;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Wallet"/>
        public Wallet(IWalletSender walletSender, IRedisDatabase redisDatabase, IMapper mapper)
        {
            _walletSender = walletSender;
            _redisDatabase = redisDatabase;
            _mapper = mapper;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<bool> GetSystemStatusAsync(CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.SystemStatusWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var result = await _walletSender.GetSystemStatusAsync(cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return result.Status == 0 && result.Message == "normal";
        }

        /// <inheritdoc />
        public async Task<TradingAccountInfoModel> GetAccountTradingStatusAsync(long recvWindow = 5000, CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.AccountStatusWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder()
                .SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();
            var result = await _walletSender.GetAccountTradingStatusAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<TradingAccountInfoModel>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Common.Models.TradeFeeModel>> GetTradeFeeAsync(
            string symbol = null,
            long recvWindow = 5000,
            CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.TradeFeeWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder()
                .SetSymbol(symbol, true)
                .SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();
            var result = await _walletSender.GetTradeFeeAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<Common.Models.TradeFeeModel>>(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeObject>> GetAllTradeObjectInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default)
        {
            var requestWeight = _weightStorage.AllCoinsInfoWeight;
            if (RedisHelper.CheckLimit(_redisDatabase, requestWeight.Type, out var rateLimit))
            {
                throw new TooManyRequestsException(rateLimit.Expiration, rateLimit.Value, rateLimit.Key);
            }

            var builder = new Builder()
                .SetRecvWindow(recvWindow);
            var query = builder.GetResult().GetQuery();
            var result = await _walletSender.GetAllCoinsInformationAsync(query, cancellationToken);

            RedisHelper.IncrementCallsMade(_redisDatabase, requestWeight, RequestWeightModel.GetDefaultKey());

            return _mapper.Map<IEnumerable<TradeObject>>(result);
        }

        #endregion
    }
}
