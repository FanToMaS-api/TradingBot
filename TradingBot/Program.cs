using BinanceExchange;
using ExchangeLibrary;
using NLog;
using System;
using System.Configuration;
using System.Linq;
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
            var binance = BinanceExchangeFactory.CreateExchange(ExchangeType.Binance, binanceOptions);
            using var cts = new CancellationTokenSource();
            var res1 = (await binance.GetExchangeInfoAsync()).ToList();
            var res = (await binance.GetCandlstickAsync(res1[0].Symbol, "1M"));
            foreach(var item in res)
            {
                Console.WriteLine(item.OpenPrice + " " + item.ClosePrice + " " + item.MaxPrice);
            }

            // Console.WriteLine(await binance.CreateNewLimitOrderAsync(
            //    "ARPABNB",
            //    "BUY",
            //    "GTC",
            //    0.000205,
            //    100,
            //    cancellationToken: cts.Token));

            // await binance.SubscribeCandlestickStreamAsync<string>(
            //    "BNBBTC",
            //    "1m",
            //     _ =>
            //     {
            //         return Task.CompletedTask;
            //     },
            //     cts.Token);

            await Task.Delay(70000);
        }
    }
}
