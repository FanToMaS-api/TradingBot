﻿using Analytic;
using Analytic.AnalyticUnits;
using Analytic.AnalyticUnits.Profiles.SSA;
using Analytic.Filters;
using Analytic.Filters.Impl.FilterManagers;
using Analytic.Models;
using AutoMapper;
using BinanceDataService;
using DataServiceLibrary;
using ExchangeLibrary;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Scheduler;
using SignalsSender.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Builder;
using Telegram.Client;

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
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        private readonly IRecurringJobScheduler _scheduler;
        private readonly IAnalyticService _analyticService;
        private readonly string _baseUrl = "https://www.binance.com/en/trade/<pair>/?layout=pro";
        private readonly string[] _baseTickers = new[] { "USDT", "BTC", "ETH" };
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region .ctor

        public Service(
            SignalSenderConfig settings,
            IServiceScopeFactory scopeFactory,
            IRecurringJobScheduler scheduler,
            IExchange exchange,
            IAnalyticService analyticService,
            ITelegramClient telegramClient,
            IMapper mapper)
        {
            _settings = settings;
            _scheduler = scheduler;
            _mapper = mapper;
            _scopeFactory = scopeFactory;
            _exchange = exchange;
            _analyticService = analyticService;
            _telegramClient = telegramClient;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Запускает бота
        /// </summary>
        public async Task RunAsync()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            using var scope = _scopeFactory.CreateScope();

            var filterManager = new DefaultFilterManager();
            var paramountFilterGroup = new FilterGroup("ParamountFilterGroup", FilterGroupType.Primary, null);
            var nameFilter = new NameFilter("NameFilter", _baseTickers);
            paramountFilterGroup.AddFilter(nameFilter);
            filterManager.AddFilterGroup(paramountFilterGroup);

            var specialFilterGroupUSDT = new FilterGroup("USDT_SpecialFilterGroup", FilterGroupType.Special, "USDT");
            var priceDeviationFilter = new PriceDeviationFilter("AnyFilter", ComparisonType.GreaterThan, 2.55);
            var usdtPriceFilter = new PriceFilter("USDTFilter", ComparisonType.LessThan, 20);
            specialFilterGroupUSDT.AddFilter(priceDeviationFilter);
            specialFilterGroupUSDT.AddFilter(usdtPriceFilter);
            filterManager.AddFilterGroup(specialFilterGroupUSDT);

            var specialFilterGroupBTC = new FilterGroup("BTC_SpecialFilterGroup", FilterGroupType.Special, "BTC");
            var btcDeviationFilter = new PriceDeviationFilter("BTCFilter", ComparisonType.GreaterThan, 5.7);
            specialFilterGroupBTC.AddFilter(btcDeviationFilter);
            filterManager.AddFilterGroup(specialFilterGroupBTC);

            var specialFilterGroupETH = new FilterGroup("ETH_SpecialFilterGroup", FilterGroupType.Special, "ETH");
            var ethDeviationFilter = new PriceDeviationFilter("ETHFilter", ComparisonType.GreaterThan, 4.5);
            specialFilterGroupETH.AddFilter(ethDeviationFilter);
            filterManager.AddFilterGroup(specialFilterGroupETH);

            var commonFilterGroup = new FilterGroup("CommonFilterGroup", FilterGroupType.Common, null);
            commonFilterGroup.AddFilter(priceDeviationFilter);
            filterManager.AddFilterGroup(commonFilterGroup);

            var commonLatestFilterGroup = new FilterGroup("CommonLatestFilterGroup", FilterGroupType.CommonLatest, null);
            var volumeFilter = new VolumeFilter("VolumeBidFilter", VolumeType.Bid, VolumeComparisonType.GreaterThan, limit: 1);
            commonLatestFilterGroup.AddFilter(volumeFilter);
            filterManager.AddFilterGroup(commonLatestFilterGroup);

            _analyticService.OnModelsFiltered += OnModelsFilteredReceived;
            _analyticService.OnSuccessfulAnalize += OnModelsToBuyReceived;

            var ssaProfile = new SsaAnalyticPofile("SsaProfile");
            var profileGroup = new ProfileGroup("DefaultGroupProfile");
            profileGroup.AddAnalyticUnit(ssaProfile);
            _analyticService.AddProfileGroup(profileGroup);
            await _analyticService.RunAsync(filterManager, cancellationToken);
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
                    var message = $"*{pairSymbols}*\nНовая разница: *{model.LastDeviation:0.00000}%*" +
                        $"\nРазница за последние 5 таймфреймов: *{model.SumDeviations:0.0000000}%*" +
                        $"\nПоследняя цена: *{model.LastPrice}*" +
                        $"\nОбъем спроса: *{model.BidVolume:0,0.0}*" +
                        $"\nОбъем предложения: *{model.AskVolume:0,0.0}*";
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

        /// <summary>
        ///     Обработчик получения модели на покупку
        /// </summary>
        private void OnModelsToBuyReceived(object sender, AnalyticResultModel[] models)
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
                    var message = $"*{pairSymbols}*\n*Минимальная цена предсказания: {model.RecommendedPurchasePrice:0.00000}*";
                    if (model.RecommendedSellingPrice is not null)
                    {
                        message += $"\n*Максимальная цена предсказания: {model.RecommendedSellingPrice.Value:0.00000}*";
                    }

                    builder.SetMessageText(message);
                    if (model.HasPredictionImage)
                    {
                        builder.SetImage(model.ImagePath);
                    }
                    else
                    {
                        var url = _baseUrl.Replace("<pair>", pairName);
                        builder.SetInlineButton("Купить", $"{url}");
                    }

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

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        #endregion
    }
}
