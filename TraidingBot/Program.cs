using ExchangeLibrary;
using ExchangeLibrary.Binance.WebSocket.Marketdata;
using NLog;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using TraidingBot.Configuration;

namespace TraidingBot
{
    internal class Program
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        static async Task Main()
        {
            var apiKey = ConfigurationManager.AppSettings.Get(ConfigKeys.API_KEY);
            var secretKey = ConfigurationManager.AppSettings.Get(ConfigKeys.SECRET_KEY);
            var binance = ExchangeFactory.CreateExchange(ExchangeType.Binance, apiKey, secretKey);
            using var cts = new CancellationTokenSource();

            Console.WriteLine(await binance.GetDayPriceChangeAsync(null, cts.Token));

            var webSoket = MarketdataWebSocket.CreateCandleStickStream(
                "BNBBTC",
                ExchangeLibrary.Binance.Enums.CandleStickIntervalType.OneMinute);

            webSoket.AddOnMessageReceivedFunc(
                _ =>
                {
                    Console.WriteLine(_);
                    return Task.CompletedTask;
                },
                cts.Token);

            await webSoket.ConnectAsync(cts.Token);

            await Task.Delay(70000);
            webSoket.Dispose();
        }
    }
}
