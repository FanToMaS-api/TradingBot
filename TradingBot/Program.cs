using Common.JsonConvertWrapper;
using ExchangeLibrary;
using ExchangeLibrary.Binance;
using ExchangeLibrary.Binance.Enums.Helper;
using ExchangeLibrary.Binance.Models;
using ExchangeLibrary.Binance.WebSocket.Marketdata;
using NLog;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using TradingBot.Configuration;

namespace TradingBot
{
    internal class Program
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        static async Task Main()
        {
            var apiKey = ConfigurationManager.AppSettings.Get(ConfigKeys.API_KEY);
            var secretKey = ConfigurationManager.AppSettings.Get(ConfigKeys.SECRET_KEY);
            var binanceOptions = new BinanceExchangeOptions() { ApiKey = apiKey, SecretKey = secretKey };
            var binance = ExchangeFactory.CreateExchange(ExchangeType.Binance, binanceOptions);
            using var cts = new CancellationTokenSource();

            Console.WriteLine(await binance.CreateNewLimitOrderAsync(
                "ARPABNB",
                "BUY",
                "GTC",
                0.000205,
                100,
                cancellationToken: cts.Token));

            await binance.SubscribeCandlestickStreamAsync<string>(
                "BNBBTC",
                "1m",
                 _ =>
                 {
                     return Task.CompletedTask;
                 },
                 cts.Token);

            await Task.Delay(70000);
        }
    }
}
