using BinanceExchange;
using ExchangeLibrary;
using NLog;
using Redis;
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
    public class Service : IDisposable
    {
        #region Fields

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly SignalSenderConfig _settings;
        private readonly IExchange _exchange;
        private readonly ITelegramClient _telegramClient;
        private readonly string _baseUrl = "https://www._exchange.com/ru/trade/";
        private readonly string _baseTicker = "USDT";
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region .ctor

        public Service(SignalSenderConfig settings, IRedisDatabase redisDatabase)
        {
            _settings = settings;
            var binanceOptions = new BinanceExchangeOptions() { ApiKey = settings.ApiKey, SecretKey = settings.SecretKey };
            _exchange = BinanceExchangeFactory.CreateExchange(binanceOptions, redisDatabase);
            _telegramClient = new TelegramClient(settings.TelegramToken);
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Запускает бота
        /// </summary>
        public async Task RunAsync()
        {
            var builder = new TelegramMessageBuilder();
            builder.SetChatId(_settings.ChannelId);

            var cancellationToken = _cancellationTokenSource.Token;
            var pairs = (await _exchange.GetSymbolPriceTickerAsync(null))
                .Where(_ => _.Symbol.Contains(_baseTicker, StringComparison.CurrentCultureIgnoreCase))
                .ToDictionary(_ => _.Symbol, _ => new List<double>());
            _logger.Info($"Всего пар: {pairs.Count}");

            var delay = TimeSpan.FromMinutes(1);
            var percent = 2.55;
            try
            {
                while (true)
                {
                    var newPairs = (await _exchange.GetSymbolPriceTickerAsync(null))
                        .Where(_ => _.Symbol.Contains(_baseTicker, StringComparison.CurrentCultureIgnoreCase))
                        .ToList();
                    _logger?.Trace("Новые данные получены");

                    newPairs.ForEach(async pair =>
                    {
                        var pricesCount = pairs[pair.Symbol].Count;
                        if (pricesCount > 250)
                        {
                            pairs[pair.Symbol].RemoveRange(0, pricesCount - 5);
                        }

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
                            _logger?.Info($"Покупай {pair.Symbol} Новая разница {newDeviation:0.00} Разница за последние 3 таймфрейма: {sumDeviation:0.00}");

                            try
                            {
                                var pairSymbols = pair.Symbol.Insert(pair.Symbol.Length - _baseTicker.Length, "/");
                                var pairUrl = pairSymbols.Replace("/", "_");
                                var message = $"**{pair.Symbol}**\n\nНовая разница: {newDeviation:0.00}\n\nРазница за последние 3 таймфрейма: {sumDeviation:0.00}";
                                builder.SetMessageText(message);
                                builder.SetInlineButton("Купить", $"{_baseUrl}{pairUrl}");
                                var telegramMessage = builder.GetResult();
                                await _telegramClient.SendMessageAsync(telegramMessage, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger?.Error(ex, "Failed to send to Telegram");
                            }
                        }

                        pairs[pair.Symbol].Add(pair.Price);
                    });

                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
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
