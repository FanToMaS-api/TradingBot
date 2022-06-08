using Analytic;
using Analytic.AnalyticUnits.ProfileGroup.Impl;
using Analytic.AnalyticUnits.Profiles.SSA;
using Analytic.Filters;
using Analytic.Filters.Builders;
using Analytic.Filters.Builders.FilterBuilders;
using Analytic.Filters.Builders.FilterGroupBuilders;
using Analytic.Filters.Enums;
using Analytic.Filters.FilterGroup.Impl;
using Analytic.Filters.Impl.FilterManagers;
using Analytic.Models;
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

        #region Implementation of IService

        /// <summary>
        ///     Запускает бота
        /// </summary>
        public async Task RunAsync()
        {
            var nameFilter= new NameFilterBuilder()
                .SetFilterName("NameFilter")
                .AddTradeObjectNames(_baseTickers)
                .GetResult();
            var primaryGroup = new FilterGroupBuilder()
                .SetFilterGroupName("ParamountFilterGroup")
                .SetFilterGroupType(FilterGroupType.Primary)
                .SetTargetTradeObjectName(null)
                .AddFilter(nameFilter)
                .GetResult();
            
            var priceDeviationFilter = new PriceDeviationFilterBuilder()
                .SetFilterName("AnyFilter")
                .SetAggregateDataIntervalType(AggregateDataIntervalType.OneMinute)
                .SetComparisonType(ComparisonType.GreaterThan)
                .SetLimit(2.55)
                .GetResult();
            var priceFilter = new PriceFilterBuilder()
                .SetFilterName("USDT_PriceFilter")
                .SetComparisonType(ComparisonType.LessThan)
                .SetLimit(20)
                .GetResult();
            var specialFilterGroupForUSDT_Builder = new FilterGroupBuilder()
                .SetFilterGroupName("USDT_SpecialFilterGroup")
                .SetFilterGroupType(FilterGroupType.Special)
                .SetTargetTradeObjectName("USDT")
                .AddFilter(priceDeviationFilter)
                .AddFilter(priceFilter);
            var specialFilterGroupForUSDT = specialFilterGroupForUSDT_Builder.GetResult();

            var priceDeviationFilterForBTC = new PriceDeviationFilterBuilder()
                .SetFilterName("BTC_PriceDeviationFilter")
                .SetAggregateDataIntervalType(AggregateDataIntervalType.OneMinute)
                .SetComparisonType(ComparisonType.GreaterThan)
                .SetLimit(5.7)
                .GetResult();
            var specialFilterGroupForBTC = new FilterGroupBuilder()
                .SetFilterGroupName("BTC_SpecialFilterGroup")
                .SetFilterGroupType(FilterGroupType.Special)
                .SetTargetTradeObjectName("BTC")
                .AddFilter(priceDeviationFilterForBTC)
                .GetResult();

            var priceDeviationFilterForETH = new PriceDeviationFilterBuilder()
                .SetFilterName("ETH_PriceDeviationFilter")
                .SetAggregateDataIntervalType(AggregateDataIntervalType.OneMinute)
                .SetComparisonType(ComparisonType.GreaterThan)
                .SetLimit(4.5)
                .GetResult();
            var specialFilterGroupForETH = new FilterGroupBuilder()
                .SetFilterGroupName("ETH_SpecialFilterGroup")
                .SetFilterGroupType(FilterGroupType.Special)
                .SetTargetTradeObjectName("ETH")
                .AddFilter(priceDeviationFilterForETH)
                .GetResult();
            
            var commonFilterGroupFor = new FilterGroupBuilder()
                .SetFilterGroupName("CommonFilterGroup")
                .SetFilterGroupType(FilterGroupType.Common)
                .SetTargetTradeObjectName("ETH")
                .AddFilter(priceDeviationFilter)
                .GetResult();

            var volumeFilter = new VolumeFilterBuilder()
                .SetFilterName("VolumeBidFilter")
                .SetVolumeType()
                .SetVolumeComparisonType()
                .SetPercentDeviation()
                .GetResult();
            var commonLatestFilterGroup = new FilterGroupBuilder()
                .SetFilterGroupType(FilterGroupType.CommonLatest)
                .SetTargetTradeObjectName(null)
                .AddFilter(volumeFilter)
                .GetResult();

            var builder = new FilterManagerBuilder(_logger);
            builder.AddFilterGroup(primaryGroup);
            builder.AddFilterGroup(specialFilterGroupForUSDT);
            builder.AddFilterGroup(specialFilterGroupForBTC);
            builder.AddFilterGroup(specialFilterGroupForETH);
            builder.AddFilterGroup(commonFilterGroupFor);
            builder.AddFilterGroup(commonLatestFilterGroup);
            
            var growingPairFilterManager = builder.GetResult();

            builder.Reset();

            var priceDeviationCommonFilter = new PriceDeviationFilterBuilder()
                .SetFilterName("FallingPriceDeviationFilter")
                .SetAggregateDataIntervalType(AggregateDataIntervalType.FiveMinutes)
                .SetComparisonType(ComparisonType.LessThan)
                .SetLimit(-7.8)
                .SetTimeframeNumber(20)
                .GetResult();
            var commonFilterGroupForFallingTickers = new FilterGroupBuilder()
                .SetTargetTradeObjectName(null)
                .SetFilterGroupName("CommonLatestFilterGroupForFallingPairs")
                .SetFilterGroupType(FilterGroupType.Common)
                .AddFilter(priceDeviationCommonFilter)
                .GetResult();

            var volumeBidFilterForFallingPairs = new VolumeFilterBuilder()
                .SetFilterName("VolumeBidFilterForFallingPairs")
                .SetVolumeType(VolumeType.Ask)
                .SetVolumeComparisonType(VolumeComparisonType.GreaterThan)
                .SetPercentDeviation(0.35)
                .GetResult();
            var commonLatestFilterGroupForFallingPairs = new FilterGroupBuilder()
                .SetTargetTradeObjectName(null)
                .SetFilterGroupName("CommonLatestFilterGroupForFallingPairs")
                .AddFilter(volumeBidFilterForFallingPairs)
                .SetFilterGroupType(FilterGroupType.CommonLatest)
                .GetResult();

            builder.AddFilterGroup(primaryGroup);
            specialFilterGroupForUSDT_Builder.RemoveFilter(priceDeviationFilter);
            builder.AddFilterGroup(specialFilterGroupForUSDT);
            builder.AddFilterGroup(commonFilterGroupForFallingTickers);
            builder.AddFilterGroup(commonLatestFilterGroupForFallingPairs);
            
            var fallingTickersFilterManager = builder.GetResult();

            var cancellationToken = _cancellationTokenSource.Token;
            using var scope = _scopeFactory.CreateScope();

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

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        #endregion

        #region Private methods

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

                    var url = _baseUrl.Replace("<pair>", pairName);
                    var telegramMessage = builder
                        .SetMessageText(message)
                        .SetInlineButton("Перейти", $"{url}")
                        .GetResult();
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
    }
}
