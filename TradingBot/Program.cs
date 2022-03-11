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
            var res = (await binance.GetAllOrdersAsync("SOLUSDT"));
            foreach (var item in res)
            {
                Console.WriteLine(item.IsWorking + " " + item.OrderSide + " " + item.Price + " " + item.Status);
            }

            //Console.WriteLine(await binance.CreateNewLimitOrderAsync(
            //   "ARPABNB",
            //   "BUY",
            //   "GTC",
            //   0.0001813,
            //   100000,
            //   cancellationToken: cts.Token));

            //await binance.SubscribeCandlestickStreamAsync(
            //   "BNBBTC",
            //   "1m",
            //    _ =>
            //    {
            //        Console.WriteLine($"{_.Symbol} {_.Interval} {_.TradesNumber} {_.MinPrice}");
            //        return Task.CompletedTask;
            //    },
            //    cts.Token);

            await Task.Delay(70000);
        }
    }
}
