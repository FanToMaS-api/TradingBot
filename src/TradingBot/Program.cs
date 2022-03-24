using BinanceExchange;
using Common.Models;
using Common.WebSocket;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var binanceOptions = new BinanceExchangeOptions() { ApiKey = apiKey, SecretKey = secretKey };
            var binance = BinanceExchangeFactory.CreateExchange(binanceOptions);
            using var cts = new CancellationTokenSource();
            var pairs = (await binance.GetSymbolPriceTickerAsync(null))
                .Where(_ => _.Symbol.Contains("USDT", StringComparison.CurrentCultureIgnoreCase))
                .ToDictionary(_ => _.Symbol, _ => new List<double>());
            _logger.Info($"Всего пар: {pairs.Count}");

            var delay = TimeSpan.FromMinutes(1);
            var percent = 2.55;
            while (true)
            {
                var newPairs = (await binance.GetSymbolPriceTickerAsync(null))
                    .Where(_ => _.Symbol.Contains("USDT", StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
                _logger.Trace("Новые данные получены");

                newPairs.ForEach(pair =>
                {
                    var pricesCount = pairs[pair.Symbol].Count;
                    if (pricesCount == 0)
                    {
                        pairs[pair.Symbol].Add(pair.Price);
                        return;
                    }

                    var newDeviation = GetDeviation(pairs[pair.Symbol].Last(), pair.Price);
                    var lastDeviation = 0d;
                    var preLastDeviation = 0d;
                    if (pricesCount > 1)
                    {
                        lastDeviation = GetDeviation(pairs[pair.Symbol][pricesCount - 2], pairs[pair.Symbol].Last());
                    }

                    if (pricesCount > 2)
                    {
                        preLastDeviation = GetDeviation(pairs[pair.Symbol][pricesCount - 3], pairs[pair.Symbol][pricesCount - 2]);
                    }

                    var sumDeviation = newDeviation + lastDeviation + preLastDeviation;
                    if (newDeviation >= percent || sumDeviation >= percent)
                    {
                        _logger.Warn($"Покупай {pair.Symbol} Новая разница {newDeviation:0.00} Разница за последние 3 таймфрейма: {sumDeviation:0.00}");
                    }

                    pairs[pair.Symbol].Add(pair.Price);
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
