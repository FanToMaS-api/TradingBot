using Analytic;
using Analytic.AnalyticUnits;
using Analytic.Binance;
using Analytic.Filters;
using Analytic.Models;
using BinanceExchange;
using ExchangeLibrary;
using NLog;
using Redis;
using Scheduler;
using SignalsSender.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Builder;
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

            //  TODO добавить группы фильтров
            var nameFilter = new NameFilter("NameFilter", _baseTickers);
            var priceFilter = new PriceFilter("AnyFilter", null, PriceFilterType.GreaterThan, 2.55);
            var allFilter = new PriceFilter("AllFilter", null, PriceFilterType.LessThan, 20);
            var btcFilter = new PriceFilter("BTCFilter", "BTC", PriceFilterType.GreaterThan, 5.7);
            var ethFilter = new PriceFilter("ETHFilter", "ETH", PriceFilterType.GreaterThan, 4.5);
            var volumeFilter = new VolumeFilter("VolumeFilter", null);
            _analyticService.AddFilter(nameFilter);
            _analyticService.AddFilter(priceFilter);
            _analyticService.AddFilter(allFilter);
            _analyticService.AddFilter(btcFilter);
            _analyticService.AddFilter(ethFilter);
            _analyticService.AddFilter(volumeFilter);
            _analyticService.OnModelsFiltered += OnModelsFilteredReceived;
            _analyticService.OnModelsReadyToBuy += OnModelsToBuyReceived;

            var profile = new DefaultAnalyticProfile("DefaultProfile");
            var profileGroup = new ProfileGroup("DefaultGroupProfile");
            profileGroup.AddAnalyticUnit(profile);
            _analyticService.AddProfileGroup(profileGroup);
            await _analyticService.RunAsync(cancellationToken);
        }

        /// <summary>
        ///     Обработчик получения отфильтрованных моделей
        /// </summary>
        private void OnModelsFilteredReceived(object sender, InfoModel[] models)
        {
            var tasks = new List<Task>();
            var builder = new TelegramMessageBuilder();
            builder.SetChatId(_settings.ChannelId);
            foreach (var model in models)
            {
                try
                {
                    var symbol = _baseTickers.FirstOrDefault(_ =>
                        model.TradeObjectName.Contains(_, StringComparison.InvariantCultureIgnoreCase));
                    if (string.IsNullOrEmpty(symbol))
                    {
                        _logger?.Error("Failed to parse symbol");
                        return;
                    }

                    var pairSymbols = model.TradeObjectName.Insert(model.TradeObjectName.Length - symbol.Length, "/");
                    var pairName = pairSymbols.Replace("/", "_");
                    var message = $"*{pairSymbols}*\nНовая разница: *{model.LastDeviation:0.00}%*" +
                        $"\nРазница за последние 5 таймфреймов: *{model.Sum5Deviations:0.00}%*" +
                        $"\nПоследняя цена: *{model.LastPrice:0.0000}*" +
                        $"\nОбъем спроса: *{model.AskVolume:0,0.0}*" +
                        $"\nОбъем предложения: *{model.BidVolume:0,0.0}*";
                    message = message.Replace(".", "\\.");
                    message = message.Replace("-", "\\-");
                    builder.SetMessageText(message);
                    var url = _baseUrl.Replace("<pair>", pairName);
                    builder.SetInlineButton("Перейти", $"{url}");
                    var telegramMessage = builder.GetResult();
                    tasks.Add(_telegramClient.SendMessageAsync(telegramMessage, CancellationToken.None));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to send message wtih filtered models to telegram");
                }
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send message with filtered models to telegram");
            }
        }

        private void OnModelsToBuyReceived(object sender, AnalyticResultModel[] models)
        {
            _logger.Info("Models to buy received!");
            var tasks = new List<Task>();
            var builder = new TelegramMessageBuilder();
            builder.SetChatId(_settings.ChannelId);
            foreach (var model in models)
            {
                try
                {
                    var symbol = _baseTickers.FirstOrDefault(_ =>
                        model.TradeObjectName.Contains(_, StringComparison.InvariantCultureIgnoreCase));
                    if (string.IsNullOrEmpty(symbol))
                    {
                        _logger?.Error("Failed to parse symbol");
                        return;
                    }

                    var pairSymbols = model.TradeObjectName.Insert(model.TradeObjectName.Length - symbol.Length, "/");
                    var pairName = pairSymbols.Replace("/", "_");
                    var message = $"*Совет купить {pairSymbols}*\n*По цене: {model.RecommendedPurchasePrice:0.0000000}*";
                    message = message.Replace(".", "\\.");
                    message = message.Replace("-", "\\-");
                    builder.SetMessageText(message);
                    var url = _baseUrl.Replace("<pair>", pairName);
                    builder.SetInlineButton("Купить", $"{url}");
                    var telegramMessage = builder.GetResult();
                    tasks.Add(_telegramClient.SendMessageAsync(telegramMessage, CancellationToken.None));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to send message with models to buy to telegram");
                }
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send message with filtered models to telegram");
            }
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
