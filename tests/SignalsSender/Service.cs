using Analytic;
using Analytic.Binance;
using Analytic.Filters;
using BinanceExchange;
using ExchangeLibrary;
using NLog;
using Redis;
using Scheduler;
using SignalsSender.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Client;
using Telegram.Client.Impl;

namespace SignalsSender
{
    /// <summary>
    ///     Содержит основную логику работы
    /// </summary>
    public class Service : IService
    {
        #region Fields

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly SignalSenderConfig _settings;
        private readonly IExchange _exchange;
        private readonly ITelegramClient _telegramClient;
        private readonly IRecurringJobScheduler _scheduler;
        private readonly IAnalyticService _analyticService;
        private readonly string _baseUrl = "https://www.binance.com/en/trade/<pair>/?layout=pro";
        private readonly string[] _baseTickers = new[] { "USDT", "BTC", "ETH" };
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region .ctor

        public Service(SignalSenderConfig settings, IRedisDatabase redisDatabase, IRecurringJobScheduler scheduler)
        {
            _settings = settings;
            _scheduler = scheduler;
            var binanceOptions = new BinanceExchangeOptions() { ApiKey = settings.ApiKey, SecretKey = settings.SecretKey };
            _exchange = BinanceExchangeFactory.CreateExchange(binanceOptions, redisDatabase);
            _analyticService = new BinanceAnalyticService(_exchange, _scheduler, _cancellationTokenSource);
            _telegramClient = new TelegramClient(settings.TelegramToken);
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Запускает бота
        /// </summary>
        public async Task RunAsync()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            var nameFilter = new NameFilter("NameFilter", new[] { "USDT", "BTC", "ETH" });
            var priceFilter = new PriceFilter("AnyFilter", null, PriceFilterType.GreaterThan, 2.55);
            var btcFilter = new PriceFilter("BTCFilter", "BTC", PriceFilterType.GreaterThan, 4.5);
            var ethFilter = new PriceFilter("ETHFilter", "ETH", PriceFilterType.GreaterThan, 3.5);
            var volumeFilter = new VolumeFilter("VolumeFilter", null);
            _analyticService.AddFilter(nameFilter);
            _analyticService.AddFilter(priceFilter);
            _analyticService.AddFilter(btcFilter);
            _analyticService.AddFilter(ethFilter);
            _analyticService.AddFilter(volumeFilter);
            _analyticService.OnModelsFiltered += (obj, models) =>
            {
                _logger.Info("Filtered models received");
            };
            await _analyticService.RunAsync(cancellationToken);

            //var builder = new TelegramMessageBuilder();
            //builder.SetChatId(_settings.ChannelId);

            //var pairs = (await _exchange.GetSymbolPriceTickerAsync(null))
            //    .Where(p => _baseTickers.Any(_ => p.ShortName.Contains(_, StringComparison.InvariantCultureIgnoreCase)))
            //    .ToDictionary(_ => _.ShortName, _ => new List<double>());
            //_logger.Info($"Всего пар: {pairs.Count}");

            //var delay = TimeSpan.FromMinutes(1);
            //var percent = 2.55;
            //try
            //{
            //    {
            //        while (true)
            //        {
            //            var newPairs = (await _exchange.GetSymbolPriceTickerAsync(null))
            //                .Where(p => _baseTickers.Any(_ => p.ShortName.Contains(_, StringComparison.InvariantCultureIgnoreCase)))
            //                .ToList();
            //            _logger?.Trace("Новые данные получены");

            //            newPairs.ForEach(async pair =>
            //            {
            //                var pricesCount = pairs[pair.ShortName].Count;
            //                if (pricesCount > 250)
            //                {
            //                    pairs[pair.ShortName].RemoveRange(0, pricesCount - 5);
            //                    pricesCount = pairs[pair.ShortName].Count;
            //                }

            //                if (pricesCount == 0)
            //                {
            //                    pairs[pair.ShortName].Add(pair.Price);
            //                    return;
            //                }

            //                var newDeviation = GetDeviation(pairs[pair.ShortName].Last(), pair.Price);
            //                var lastDeviation = 0d;
            //                var preLastDeviation = 0d;
            //                if (pricesCount > 1)
            //                {
            //                    lastDeviation = GetDeviation(pairs[pair.ShortName][pricesCount - 2], pairs[pair.ShortName].Last());
            //                }

            //                if (pricesCount > 2)
            //                {
            //                    preLastDeviation = GetDeviation(pairs[pair.ShortName][pricesCount - 3], pairs[pair.ShortName][pricesCount - 2]);
            //                }

            //                var sumDeviation = newDeviation + lastDeviation + preLastDeviation;
            //                if (newDeviation >= percent || sumDeviation >= percent)
            //                {
            //                    _logger?.Info($"Покупай {pair.ShortName} Новая разница {newDeviation:0.00} Разница за последние 3 таймфрейма: {sumDeviation:0.00}");

            //                    try
            //                    {
            //                        var symbol = _baseTickers.FirstOrDefault(_ => pair.ShortName.Contains(_, StringComparison.InvariantCultureIgnoreCase));
            //                        if (string.IsNullOrEmpty(symbol))
            //                        {
            //                            _logger?.Error("Failed to parse symbol");
            //                            return;
            //                        }

            //                        var pairSymbols = pair.ShortName.Insert(pair.ShortName.Length - symbol.Length, "/");
            //                        var pairUrl = pairSymbols.Replace("/", "_");
            //                        var message = $"*{pairSymbols}*\nНовая разница: *{newDeviation:0.00}*\nРазница за последние 3 таймфрейма: *{sumDeviation:0.00}*\nПоследняя цена: {pair.Price}";
            //                        message = message.Replace(".", "\\.");
            //                        message = message.Replace("-", "\\-");
            //                        builder.SetMessageText(message);
            //                        var url = _baseUrl.Replace("<pair>", pairUrl);
            //                        builder.SetInlineButton("Купить", $"{url}");
            //                        var telegramMessage = builder.GetResult();
            //                        await _telegramClient.SendMessageAsync(telegramMessage, cancellationToken);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        _logger?.Error(ex, "Failed to send to Telegram");
            //                    }
            //                }

            //                pairs[pair.ShortName].Add(pair.Price);
            //            });

            //            await Task.Delay(delay);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger?.Error(ex);
            //}
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Возвращает процентное отклонение новой цены от старой
        /// </summary>
        private static double GetDeviation(double oldPrice, double newPrice) => (newPrice / (double)oldPrice - 1) * 100;

        /// <inheritdoc />
        public void Dispose() => _cancellationTokenSource?.Dispose();

        #endregion
    }
}
