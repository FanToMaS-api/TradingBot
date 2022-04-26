using BinanceExchange.Client;
using BinanceExchange.Client.Impl;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Impl;
using ExchangeLibrary;
using ExtensionsLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace BinanceExchange
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Добавить биржу бинанса
        /// </summary>
        public static void AddBinanceExchange(this IServiceCollection services, IConfiguration configuration)
        {
            services.LoadOptions<BinanceExchangeOptions>(configuration);
            services.AddHttpClient(); // регистрирую IHttpClientFactory
            services.AddSingleton<IBinanceClient, BinanceClient>();

            services.AddSingleton<ISpotTradeSender, SpotTradeSender>();
            services.AddSingleton<IWalletSender, WalletSender>();
            services.AddSingleton<IMarketdataSender, MarketdataSender>();

            services.AddSingleton<IMarketdata, Marketdata>();
            services.AddSingleton<IWallet, Wallet>();
            services.AddSingleton<ISpotTrade, SpotTrade>();
            services.AddSingleton<IMarketdataStreams, MarketdataStreams>();

            services.AddSingleton<IExchange, BinanceExchange>();
        }
    }
}
