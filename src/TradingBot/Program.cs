using BinanceExchange;
using Common.Models;
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
        private readonly ILogger _logger = LogManager.LoadConfiguration("nlog.config").GetLogger("Program");

        static async Task Main()
        {
            var apiKey = ConfigurationManager.AppSettings.Get(ConfigKeys.API_KEY);
            var secretKey = ConfigurationManager.AppSettings.Get(ConfigKeys.SECRET_KEY);
            var binanceOptions = new BinanceExchangeOptions() { ApiKey = apiKey, SecretKey = secretKey };
            var binance = BinanceExchangeFactory.CreateExchange(binanceOptions);
            using var cts = new CancellationTokenSource();
            //var res = (await binance.GetAllOrdersAsync("SOLUSDT"));
            //foreach (var item in res)
            //{
            //    Console.WriteLine(item.IsWorking + " " + item.OrderSide + " " + item.Price + " " + item.Status);
            //}

            //Console.WriteLine(await binance.CreateNewLimitOrderAsync(
            //   "ARPABNB",
            //   "BUY",
            //   "GTC",
            //   0.0001813,
            //   100000,
            //   cancellationToken: cts.Token));

            var properties = typeof(TickerStreamModel).GetProperties();
            using var webSocket = await binance.SubscribeNewStreamAsync<TickerStreamModel>(
                "ETCUSDT",
                "@ticker",
                _ =>
                {
                    //foreach (var e in _)
                    //{
                    foreach (var property in properties)
                    {
                        Console.Write($"{property.Name}: {property.GetValue(_)} ");
                    }

                    Console.WriteLine();
                    //}
                    return Task.CompletedTask;
                },
                () => { Console.WriteLine("Stream Was Closed"); },
                cancellationToken: cts.Token);

            Task.Run(async () =>await webSocket.ConnectAsync(cts.Token));

            using var webSocket1 = await binance.SubscribeNewStreamAsync<TickerStreamModel>(
                "SOLUSDT",
                "@ticker",
                _ =>
                {
                    //foreach (var e in _)
                    //{
                    foreach (var property in properties)
                    {
                        Console.Write($"{property.Name}: {property.GetValue(_)} ");
                    }

                    Console.WriteLine();
                    //}
                    return Task.CompletedTask;
                },
                () => { Console.WriteLine("Stream Was Closed"); },
                cancellationToken: cts.Token);

            Task.Run(async () => await webSocket1.ConnectAsync(cts.Token));

            await Task.Delay(70000);
        }
    }
}
