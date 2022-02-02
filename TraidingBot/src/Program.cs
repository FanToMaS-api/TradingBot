using ExchangeLibrary;
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

            Console.WriteLine(await binance.GetAllCoinsInformationAsync(7000, cts.Token));
        }
    }
}
