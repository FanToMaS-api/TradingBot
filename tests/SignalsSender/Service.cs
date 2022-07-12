using Analytic;
using Analytic.AnalyticUnits.ProfileGroup.Impl;
using Analytic.AnalyticUnits.Profiles.ML;
using Analytic.AnalyticUnits.Profiles.ML.DataLoaders.Impl;
using Analytic.AnalyticUnits.Profiles.ML.DataLoaders.Impl.Binance;
using Analytic.Filters;
using Analytic.Filters.Builders;
using Analytic.Filters.Builders.FilterBuilders;
using Analytic.Filters.Builders.FilterGroupBuilders;
using Analytic.Filters.Enums;
using Analytic.Filters.FilterGroup.Impl;
using Analytic.Models;
using AutoMapper;
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
using Telegram.Models;

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
        private readonly IMapper _mapper;
        private readonly IAnalyticService _analyticService;
        private readonly string _baseUrl = "https://www.binance.com/en/trade/<pair>/?layout=pro";
        private readonly string[] _baseTickers = new[] { "USDT", "BTC", "ETH" };
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region .ctor

        public Service(
            SignalSenderConfig settings,
            IAnalyticService analyticService,
            ITelegramClient telegramClient,
            IMapper mapper,
            ILoggerDecorator logger)
        {
            _settings = settings;
            _analyticService = analyticService;
            _logger = logger;
            _telegramClient = telegramClient;
            _mapper = mapper;
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
                .SetOrderNumber(1000)
                .SetComparisonType()
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
                .SetAggregateDataIntervalType(AggregateDataIntervalType.Default)
                .SetComparisonType(ComparisonType.LessThan)
                .SetLimit(-10)
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
                .SetOrderNumber(1000)
                .SetComparisonType(VolumeComparisonType.GreaterThan)
                .SetPercentDeviation(0.35)
                .GetResult();
            var commonLatestFilterGroupForFallingPairs = new FilterGroupBuilder()
                .SetTargetTradeObjectName(null)
                .SetFilterGroupName("CommonLatestFilterGroupForFallingPairs")
                .AddFilter(volumeBidFilterForFallingPairs)
                .SetFilterGroupType(FilterGroupType.CommonLatest)
                .GetResult();

            specialFilterGroupForUSDT_Builder.RemoveFilter(priceDeviationFilter);
            specialFilterGroupForUSDT_Builder.AddFilter(priceDeviationCommonFilter);
            var specialUsdtFilterGroupForFallingPairs = specialFilterGroupForUSDT_Builder.GetResult();

            builder.AddFilterGroup(primaryGroup);
            builder.AddFilterGroup(specialUsdtFilterGroupForFallingPairs);
            builder.AddFilterGroup(commonFilterGroupForFallingTickers);
            builder.AddFilterGroup(commonLatestFilterGroupForFallingPairs);
            
            var fallingTickersFilterManager = builder.GetResult();

            var cancellationToken = _cancellationTokenSource.Token;
            _analyticService.ModelsFiltered += OnModelsFilteredReceived;
            _analyticService.SuccessfulAnalyzed += OnModelsToBuyReceived;

            // var ssaDataLoader = new BinanceDataLoaderForSsa(_logger, _mapper);
            // var ssaMlProfile = new MlAnalyticProfile(_logger, MachineLearningModelType.SSA, ssaDataLoader, "MlSsaProfile");

            var fastTreeDataLoader = new BinanceDataLoader(_logger, _mapper);
            var fastTreeProfile = new MlAnalyticProfile(_logger, MachineLearningModelType.FastTree, fastTreeDataLoader, "MlFastTreeProfile");
            var profileGroup = new ProfileGroup(_logger, "ProfileGroup");
            
            // profileGroup.AddAnalyticUnit(ssaMlProfile);
            profileGroup.AddAnalyticUnit(fastTreeProfile);
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
            _analyticService.ModelsFiltered -= OnModelsFilteredReceived;
            _analyticService.SuccessfulAnalyzed -= OnModelsToBuyReceived;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик получения отфильтрованных моделей
        /// </summary>
        private void OnModelsFilteredReceived(object sender, InfoModel[] models)
        {
            var messageModels = new List<TelegramMessageModel>();
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
                    var message = $"*{pairSymbols}*\nНовая разница: *{model.LastDeviation:e+3}%*" +
                        $"\nРазница за последние несколько таймфреймов: *{model.DeviationsSum:e+3}%*" +
                        $"\nПоследняя цена: *{model.LastPrice:e+3}*" +
                        $"\nОбъем спроса: *{model.BidVolume:e+3}*" +
                        $"\nОбъем предложения: *{model.AskVolume:e+3}*";

                    var url = _baseUrl.Replace("<pair>", pairName);
                    var telegramMessage = builder
                        .SetMessageText(message)
                        .SetInlineButton("Перейти", $"{url}")
                        .GetResult();

                    messageModels.Add(telegramMessage);
                }
                catch (Exception ex)
                {
                    _logger.ErrorAsync(ex, "Failed to send message wtih filtered models to telegram").Wait(5 * 1000);
                }
            }

            try
            {
                _telegramClient.SendManyMessagesAsync(messageModels, _cancellationTokenSource.Token).Wait();
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
            var messageModels = new List<TelegramMessageModel>();
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
                    var message = $"*{pairSymbols}*\n*Минимальная цена прогноза: {model.RecommendedPurchasePrice:e+3}*";
                    if (model.RecommendedSellingPrice is not null)
                    {
                        message += $"\n*Максимальная цена прогноза: {model.RecommendedSellingPrice.Value:e+3}*";
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
                    messageModels.Add(telegramMessage);
                }
                catch (Exception ex)
                {
                    _logger.ErrorAsync(ex, "Failed to send message with models to buy to telegram").Wait(7 * 1000);
                }
            }

            try
            {
                _telegramClient.SendManyMessagesAsync(messageModels, _cancellationTokenSource.Token).Wait();
            }
            catch (Exception ex)
            {
                _logger.ErrorAsync(ex, "Failed to send message with filtered models to telegram").Wait(7 * 1000);
            }
        }

        #endregion
    }
}
