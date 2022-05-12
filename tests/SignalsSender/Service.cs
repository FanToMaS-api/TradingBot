using Analytic;
using Analytic.AnalyticUnits;
using Analytic.AnalyticUnits.Profiles.SSA;
using Analytic.Filters;
using Analytic.Filters.Impl.FilterManagers;
using Analytic.Models;
using BinanceDatabase.Enums;
using Logger;
using Microsoft.Extensions.DependencyInjection;
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

        private readonly ILoggerDecorator _logger;
        private readonly SignalSenderConfig _settings;
        private readonly ITelegramClient _telegramClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAnalyticService _analyticService;
        private readonly string _baseUrl = "https://www.binance.com/en/trade/<pair>/?layout=pro";
        private readonly string[] _baseTickers = new[] { "USDT", "BTC", "ETH" };
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region .ctor

        public Service(
            SignalSenderConfig settings,
            IServiceScopeFactory scopeFactory,
            IAnalyticService analyticService,
            ITelegramClient telegramClient,
            ILoggerDecorator logger)
        {
            _settings = settings;
            _scopeFactory = scopeFactory;
            _analyticService = analyticService;
            _logger = logger;
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

            var growingPairFilterManager = new DefaultFilterManager(_logger);
            var fallingTickersFilterManager = new DefaultFilterManager(_logger);

            var paramountFilterGroup = new FilterGroup("ParamountFilterGroup", FilterGroupType.Primary, null);
            var nameFilter = new NameFilter("NameFilter", _baseTickers);
            paramountFilterGroup.AddFilter(nameFilter);
            growingPairFilterManager.AddFilterGroup(paramountFilterGroup);
            fallingTickersFilterManager.AddFilterGroup(paramountFilterGroup);

            var specialFilterGroupUSDT = new FilterGroup("USDT_SpecialFilterGroup", FilterGroupType.Special, "USDT");
            var specialFilterGroupUSDTWithoutPriceDeviation = new FilterGroup(
                "USDT_SpecialFilterGroupWithoutPriceDeviation", 
                FilterGroupType.Special,
                "USDT");

            var priceDeviationFilter = new PriceDeviationFilter("AnyFilter", AggregateDataIntervalType.OneMinute, ComparisonType.GreaterThan, 2.55);
            var usdtPriceFilter = new PriceFilter("USDTFilter", ComparisonType.LessThan, 20);
            specialFilterGroupUSDT.AddFilter(priceDeviationFilter);
            specialFilterGroupUSDT.AddFilter(usdtPriceFilter);
            growingPairFilterManager.AddFilterGroup(specialFilterGroupUSDT);

            specialFilterGroupUSDTWithoutPriceDeviation.AddFilter(usdtPriceFilter);
            fallingTickersFilterManager.AddFilterGroup(specialFilterGroupUSDTWithoutPriceDeviation);

            var specialFilterGroupBTC = new FilterGroup("BTC_SpecialFilterGroup", FilterGroupType.Special, "BTC");
            var btcDeviationFilter = new PriceDeviationFilter("BTCFilter", AggregateDataIntervalType.OneMinute, ComparisonType.GreaterThan, 5.7);
            specialFilterGroupBTC.AddFilter(btcDeviationFilter);
            growingPairFilterManager.AddFilterGroup(specialFilterGroupBTC);

            var specialFilterGroupETH = new FilterGroup("ETH_SpecialFilterGroup", FilterGroupType.Special, "ETH");
            var ethDeviationFilter = new PriceDeviationFilter("ETHFilter", AggregateDataIntervalType.OneMinute, ComparisonType.GreaterThan, 4.5);
            specialFilterGroupETH.AddFilter(ethDeviationFilter);
            growingPairFilterManager.AddFilterGroup(specialFilterGroupETH);

            var commonFilterGroup = new FilterGroup("CommonFilterGroup", FilterGroupType.Common, null);
            commonFilterGroup.AddFilter(priceDeviationFilter);
            growingPairFilterManager.AddFilterGroup(commonFilterGroup);

            var commonFilterGroupForFallingPairs = new FilterGroup("CommonFilterGroupForFallingPairs", FilterGroupType.Common, null);
            var fallingPriceDeviationFilter = new PriceDeviationFilter(
                "FallingPriceDeviationFilter", AggregateDataIntervalType.FiveMinutes, ComparisonType.LessThan, -7.8, 20);
            commonFilterGroupForFallingPairs.AddFilter(fallingPriceDeviationFilter);
            fallingTickersFilterManager.AddFilterGroup(commonFilterGroupForFallingPairs);

            var commonLatestFilterGroup = new FilterGroup("CommonLatestFilterGroup", FilterGroupType.CommonLatest, null);
            var volumeFilter = new VolumeFilter(
                "VolumeBidFilter",
                VolumeType.Bid,
                VolumeComparisonType.GreaterThan,
                percentDeviation: 0.05);
            commonLatestFilterGroup.AddFilter(volumeFilter);
            growingPairFilterManager.AddFilterGroup(commonLatestFilterGroup);

            var commonLatestFilterGroupForFallingPairs = new FilterGroup(
                "CommonLatestFilterGroupForFallingPairs", 
                FilterGroupType.CommonLatest,
                null);
            var volumeFilterForFallingPairs = new VolumeFilter(
                "VolumeBidFilterForFallingPairs", 
                VolumeType.Ask,
                VolumeComparisonType.GreaterThan,
                percentDeviation: 0.35);
            commonLatestFilterGroupForFallingPairs.AddFilter(volumeFilterForFallingPairs);
            fallingTickersFilterManager.AddFilterGroup(commonLatestFilterGroupForFallingPairs);

            _analyticService.OnModelsFiltered += OnModelsFilteredReceived;
            _analyticService.OnSuccessfulAnalize += OnModelsToBuyReceived;

            var ssaProfile = new SsaAnalyticPofile(_logger, "SsaProfile");
            var profileGroup = new ProfileGroup(_logger, "DefaultGroupProfile");
            profileGroup.AddAnalyticUnit(ssaProfile);
            _analyticService.AddProfileGroup(profileGroup);
            _analyticService.AddFilterManager(growingPairFilterManager);
            _analyticService.AddFilterManager(fallingTickersFilterManager);
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
                        _logger?.ErrorAsync("Failed to parse symbol").Wait(7 * 1000);
                        return;
                    }

                    var pairSymbols = model.TradeObjectName.Insert(model.TradeObjectName.Length - symbol.Length, "/");
                    var pairName = pairSymbols.Replace("/", "_");
                    var message = $"*{pairSymbols}*\nНовая разница: *{model.LastDeviation:0.00000}%*" +
                        $"\nРазница за последние несколько таймфреймов: *{model.DeviationsSum:0.0000000}%*" +
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
                    tasks.Add(_logger.ErrorAsync(ex, "Failed to send message wtih filtered models to telegram"));
                }
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                _logger.ErrorAsync(ex, "Failed to send message with filtered models to telegram").Wait(7 * 1000);
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
                        _logger?.ErrorAsync("Failed to parse symbol").Wait(7 * 1000);
                        return;
                    }

                    var pairSymbols = model.TradeObjectName.Insert(model.TradeObjectName.Length - symbol.Length, "/");
                    var pairName = pairSymbols.Replace("/", "_");
                    var message = $"*{pairSymbols}*\n*Минимальная цена прогноза: {model.RecommendedPurchasePrice:0.00000}*";
                    if (model.RecommendedSellingPrice is not null)
                    {
                        message += $"\n*Максимальная цена прогноза: {model.RecommendedSellingPrice.Value:0.00000}*";
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
                    tasks.Add(_logger.ErrorAsync(ex, "Failed to send message with models to buy to telegram"));
                }
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                _logger.ErrorAsync(ex, "Failed to send message with filtered models to telegram").Wait(7 * 1000);
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
