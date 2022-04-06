using BinanceExchange;
using NLog;
using Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Builder;
using Telegram.Client;
using Telegram.Client.Impl;
using TradingBot.Configuration;

namespace TradingBot
{
    internal class Program
    {
        private static readonly ILogger _logger = LogManager.LoadConfiguration("nlog.config").GetLogger("Program");

        static async Task Main()
        {
            var apiKey = ConfigurationManager.AppSettings.Get(ConfigKeys.API_KEY);
            var secretKey = ConfigurationManager.AppSettings.Get(ConfigKeys.SECRET_KEY);
            var redisConnectionString = ConfigurationManager.AppSettings.Get(ConfigKeys.REDIS_CONNECTION_STRING);
            var binanceOptions = new BinanceExchangeOptions() { ApiKey = apiKey, SecretKey = secretKey };
            var binance = BinanceExchangeFactory.CreateExchange(
                binanceOptions, new RedisDatabase(ConnectionMultiplexer.Connect(redisConnectionString)));
            using var cts = new CancellationTokenSource();

            var socket = binance.MarketdataStreams.SubscribeAllMarketTickersStream(
                (models, ct) =>
                {
                    foreach (var model in models.Where(_ => _.ShortName.Contains("APEUSDT")))
                    {
                        Console.WriteLine(model.ShortName + " " + model.LastPrice);
                    }

                    return Task.CompletedTask;
                },
                cts.Token);
            await socket.ConnectAsync(cts.Token);


            //var info = await binance.GetAccountInformationAsync(cts.Token);
            //var properties = typeof(AccountInformationModel).GetProperties();
            //foreach (var property in properties)
            //{
            //    Console.Write($"{property.Name} {property.GetValue(info)} ");
            //    if (property.Name == "Balances")
            //    {
            //        var properties1 = typeof(BalanceModel).GetProperties();
            //        foreach (var balance in info.Balances)
            //        {
            //            Console.WriteLine();
            //            foreach (var property2 in properties1)
            //            {
            //                Console.Write($"{property2.Name} {property2.GetValue(balance)} ");
            //            }
            //        }
            //    }
            //}

            var botToken = ConfigurationManager.AppSettings.Get(ConfigKeys.TELEGRAM_TOKEN);
            var chatId = long.Parse(ConfigurationManager.AppSettings.Get(ConfigKeys.TELEGRAM_CHANNEL_ID));

            ITelegramClient telegramClient = new TelegramClient(botToken);
            var builder = new TelegramMessageBuilder();
            builder.SetChatId(chatId);
            builder.SetMessageText("**Test/Test**\n\nНовая разница: 10.23\n\nРазница за последние 3 таймфрейма: -12.25");
            builder.SetInlineButton("Test Inline button", "https://en.wikipedia.org/wiki/Site");
            var message = builder.GetResult();
            await telegramClient.SendMessageAsync(message, cts.Token);

            var pairs = (await binance.Marketdata.GetSymbolPriceTickerAsync(null))
                .Where(_ => _.Name.Contains("USDT", StringComparison.CurrentCultureIgnoreCase))
                .ToDictionary(_ => _.Name, _ => new List<double>());
            _logger.Info($"Всего пар: {pairs.Count}");

            var delay = TimeSpan.FromMinutes(1);
            var percent = 2.55;
            while (true)
            {
                var newPairs = (await binance.Marketdata.GetSymbolPriceTickerAsync(null))
                    .Where(_ => _.Name.Contains("USDT", StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
                _logger.Trace("Новые данные получены");

                newPairs.ForEach(pair =>
                {
                    var pricesCount = pairs[pair.Name].Count;
                    if (pricesCount == 0)
                    {
                        pairs[pair.Name].Add(pair.Price);
                        return;
                    }

                    var newDeviation = GetDeviation(pairs[pair.Name].Last(), pair.Price);
                    var lastDeviation = 0d;
                    var preLastDeviation = 0d;
                    if (pricesCount > 1)
                    {
                        lastDeviation = GetDeviation(pairs[pair.Name][pricesCount - 2], pairs[pair.Name].Last());
                    }

                    if (pricesCount > 2)
                    {
                        preLastDeviation = GetDeviation(pairs[pair.Name][pricesCount - 3], pairs[pair.Name][pricesCount - 2]);
                    }

                    var sumDeviation = newDeviation + lastDeviation + preLastDeviation;
                    if (newDeviation >= percent || sumDeviation >= percent)
                    {
                        _logger.Warn($"Покупай {pair.Name} Новая разница {newDeviation:0.00} Разница за последние 3 таймфрейма: {sumDeviation:0.00}");
                    }

                    pairs[pair.Name].Add(pair.Price);
                });

                await Task.Delay(delay);
            }
        }

        /// <summary>
        ///     Возвращает процентное отклонение новой цены от старой
        /// </summary>
        private static double GetDeviation(double oldPrice, double newPrice) => (newPrice / (double)oldPrice - 1) * 100;
    }
}
