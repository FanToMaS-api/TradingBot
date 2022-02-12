using ExchangeLibrary;
using ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata.Impl;
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

            var webSoket = MarketdataWebSocket<CandleStickStreamDto>.CreateCandleStickStream(
                "BNBBTC",
                ExchangeLibrary.Binance.Enums.CandleStickIntervalType.OneMinute);

            webSoket.AddOnMessageReceivedFunc(
                _ =>
                {
                    Console.WriteLine($"Interval: {_.Kline.Interval} Open Price: {_.Kline.OpenPrice} Close Price: {_.Kline.ClosePrice}");
                    return Task.CompletedTask;
                },
                cts.Token);

            await webSoket.ConnectAsync(cts.Token);

            await Task.Delay(70000);
            webSoket.Dispose();
        }
    }
}
