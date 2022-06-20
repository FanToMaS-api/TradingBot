using BinanceExchange.Client;
using BinanceExchange.Client.Http.Request.Builders;
using BinanceExchange.Models;
using Common.JsonConvertWrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceExchange.EndpointSenders.Impl
{
    /// <summary>
    ///     Отвечает за отправку запросов к конечным точкам кошелька
    /// </summary>
    internal class WalletSender : IWalletSender
    {
        #region Fields

        private readonly IBinanceClient _client;
        private readonly JsonDeserializerWrapper _converter = new();

        #endregion

        #region .ctor

        /// <inheritdoc cref="WalletSender" />
        public WalletSender(IBinanceClient client)
        {
            _client = client;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<SystemStatusModel> GetSystemStatusAsync(CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.SYSTEM_STATUS)
                .SetHttpMethod(HttpMethod.Get)
                .GetResult();
            var response = await _client.SendPublicAsync(request, cancellationToken);

            return _converter.Deserialize<SystemStatusModel>(response);
        }

        /// <inheritdoc />
        public async Task<AccountTradingStatusModel> GetAccountTradingStatusAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.ACCOUNT_API_TRADING_STATUS)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<AccountTradingStatusModel>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.TRADE_FEE)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<IEnumerable<TradeFeeModel>>(response);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CoinModel>> GetAllCoinsInformationAsync(
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestUrlBuilder()
                .SetEndpoint(BinanceEndpoints.ALL_COINS_INFORMATION)
                .SetHttpMethod(HttpMethod.Get)
                .SetParameters(parameters)
                .GetResult();
            var response = await _client.SendSignedAsync(request, cancellationToken);

            return _converter.Deserialize<List<CoinModel>>(response);
        }

        #endregion
    }
}
