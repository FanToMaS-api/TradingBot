﻿using BinanceExchange.Enums;
using BinanceExchange.Models;
using Common.Enums;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Xunit;

namespace BinanceExchangeTests.BinanceTests
{
    /// <summary>
    ///     Содержит тестовые методы общие для многих тестов
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        ///     Создает мок HttpClient
        /// </summary>
        /// <param name="filePath"> Путь к файлу с json-response </param>
        public static HttpClient CreateMockHttpClient(string url, string filePath)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, filePath);
            var json = File.ReadAllText(path);
            using var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url).Respond("application/json", json);

            return new HttpClient(mockHttp);
        }

        /// <summary>
        ///     Проверка утверждений
        /// </summary>
        public static void CheckingAssertions<T1, T2>(T1 expected, T2 actual)
        {
            var properties = expected.GetType().GetProperties();
            foreach(var property in properties)
            {
                Assert.Equal(property.GetValue(expected), property.GetValue(actual));
            }
        }

        #region Candlestick

        /// <summary>
        ///     Возвращает модели для прохождения тестов свечи для пары
        /// </summary>
        public static CandlestickModel[] GetBinanceCandlestickModels() =>
            new CandlestickModel[]
            {
                new CandlestickModel
                {
                    OpenTimeUnix = 1499040000000,
                    OpenPrice = 0.01634790,
                    MaxPrice = 0.80000000,
                    MinPrice = 0.01575800,
                    ClosePrice = 0.01577100,
                    Volume = 148976.11427815,
                    CloseTimeUnix = 1499644799999,
                    QuoteAssetVolume = 2434.19055334,
                    TradesNumber = 308,
                    BasePurchaseVolume = 1756.87402397,
                    QuotePurchaseVolume = 28.46694368,
                },
                new CandlestickModel
                {
                    OpenTimeUnix = 12,
                    OpenPrice = 0.12,
                    MaxPrice = 0.12,
                    MinPrice = 0.12,
                    ClosePrice = 0.12,
                    Volume = 12.11427815,
                    CloseTimeUnix = 12,
                    QuoteAssetVolume = 12,
                    TradesNumber = 12,
                    BasePurchaseVolume = 12.87402397,
                    QuotePurchaseVolume = 12,
                }
            };

        /// <summary>
        ///     Возвращает модели свечи для пары для проверки результатов метода
        /// </summary>
        public static Common.Models.CandlestickModel[] GetExpectedCandlestickModels() =>
            new Common.Models.CandlestickModel[]
            {
                new Common.Models.CandlestickModel
                {
                    OpenTimeUnix = 1499040000000,
                    OpenPrice = 0.01634790,
                    MaxPrice = 0.80000000,
                    MinPrice = 0.01575800,
                    ClosePrice = 0.01577100,
                    Volume = 148976.11427815,
                    CloseTimeUnix = 1499644799999,
                    QuoteAssetVolume = 2434.19055334,
                    TradesNumber = 308,
                    BasePurchaseVolume = 1756.87402397,
                    QuotePurchaseVolume = 28.46694368,
                },
                new Common.Models.CandlestickModel
                {
                    OpenTimeUnix = 12,
                    OpenPrice = 0.12,
                    MaxPrice = 0.12,
                    MinPrice = 0.12,
                    ClosePrice = 0.12,
                    Volume = 12.11427815,
                    CloseTimeUnix = 12,
                    QuoteAssetVolume = 12,
                    TradesNumber = 12,
                    BasePurchaseVolume = 12.87402397,
                    QuotePurchaseVolume = 12,
                }
            };

        #endregion

        #region Day price change

        /// <summary>
        ///     Возвращает модель изменения цены за 1 день по паре
        /// </summary>
        public static DayPriceChangeModel GetBinanceDayPriceChangeModels() =>
            new DayPriceChangeModel
            {
                Symbol = "BNBBTC",
                PriceChange = -94.99999800,
                PriceChangePercent = -95.960,
                WeightedAvgPrice = 0.29628482,
                PrevClosePrice = 0.10002000,
                LastPrice = 4.00000200,
                LastQty = 200.00000000,
                BidPrice = 4.00000000,
                BidQty = 100.00000000,
                AskPrice = 4.00000200,
                AskQty = 100.00000000,
                OpenPrice = 99.00000000,
                HighPrice = 100.00000000,
                LowPrice = 0.10000000,
                Volume = 8913.30000000,
                QuoteVolume = 15.30000000,
                OpenTimeUnix = 1499783499040,
                CloseTimeUnix = 1499869899040,
                FirstId = 28385,
                LastId = 28460,
                Count = 76,
            };

        /// <summary>
        ///     Возвращает модели изменения цены за 1 день по паре
        /// </summary>
        public static Common.Models.DayPriceChangeModel GetExpectedDayPriceChangeModels() =>
            new Common.Models.DayPriceChangeModel
            {
                Symbol = "BNBBTC",
                PriceChange = -94.99999800,
                PriceChangePercent = -95.960,
                WeightedAvgPrice = 0.29628482,
                PrevClosePrice = 0.10002000,
                LastPrice = 4.00000200,
                LastQty = 200.00000000,
                BidPrice = 4.00000000,
                BidQty = 100.00000000,
                AskPrice = 4.00000200,
                AskQty = 100.00000000,
                OpenPrice = 99.00000000,
                HighPrice = 100.00000000,
                LowPrice = 0.10000000,
                Volume = 8913.30000000,
                QuoteVolume = 15.30000000,
                OpenTimeUnix = 1499783499040,
                CloseTimeUnix = 1499869899040,
                FirstId = 28385,
                LastId = 28460,
                Count = 76,
            };

        #endregion

        #region Symbol price ticker

        /// <summary>
        ///     Возвращает модель текущей цены пары
        /// </summary>
        public static SymbolPriceTickerModel GetBinanceSymbolPriceTickerModel(string symbol, double price) =>
            new SymbolPriceTickerModel
            {
                Symbol = symbol,
                Price = price
            };

        /// <summary>
        ///     Возвращает модель текущей цены пары
        /// </summary>
        public static Common.Models.SymbolPriceModel GetExpectedSymbolPriceTickerModel(string symbol, double price) =>
            new Common.Models.SymbolPriceModel
            {
                Symbol = symbol,
                Price = price
            };

        #endregion

        #region Symbol order book ticker

        /// <summary>
        ///     Возвращает модель лучшей цены/кол-ва из стакана для пары
        /// </summary>
        public static SymbolOrderBookTickerModel GetBinanceSymbolOrderBookTickerModel(
            string symbol,
            double bidPrice,
            double bidQty,
            double askPrice,
            double askQty) =>
            new SymbolOrderBookTickerModel
            {
                Symbol = symbol,
                BidPrice = bidPrice,
                BidQty = bidQty,
                AskPrice = askPrice,
                AskQty = askQty,
            };

        /// <summary>
        ///     Возвращает модель лучшей цены/кол-ва из стакана для пары
        /// </summary>
        public static Common.Models.BestSymbolOrderModel GetExpectedBestSymbolOrderModel(
            string symbol,
            double bidPrice,
            double bidQty,
            double askPrice,
            double askQty) =>
            new Common.Models.BestSymbolOrderModel
            {
                Symbol = symbol,
                BidPrice = bidPrice,
                BidQuantity = bidQty,
                AskPrice = askPrice,
                AskQuantity = askQty,
            };

        #endregion

        #region Full order response

        /// <summary>
        ///     Возвращает модель ответа на запрос о создании нового ордера
        /// </summary>
        public static FullOrderResponseModel GetBinanceFullOrderResponseModel(
            string symbol,
            double price,
            double qty,
            OrderStatusType status,
            TimeInForceType timeInForce,
            OrderType orderType,
            OrderSideType orderSide) =>
            new()
            {
                Symbol = symbol,
                OrderId = 28,
                ClientOrderId = "6gCrw2kRUAF9CvJDGP16IP",
                TransactTimeUnix = 1507725176595,
                OrderListId = -1,
                Price = price,
                OrigQty = qty,
                ExecutedQty = 10.1,
                CumulativeQuoteQty = 10.2,
                Status = status,
                TimeInForce = timeInForce,
                OrderType = orderType,
                OrderSide = orderSide,
                Fills = new List<FillModel>
                {
                    new FillModel
                    {
                        Price = 4000.00000000,
                        Quantity = 1.00000000,
                        Commission = 4.00000000,
                        CommissionAsset = "USDT",
                        TradeId = 56
                    },
                    new FillModel
                    {
                        Price = 3999.00000000,
                        Quantity = 5.00000000,
                        Commission = 19.99500000,
                        CommissionAsset = "USDT",
                        TradeId = 57
                    },
                    new FillModel
                    {
                        Price = 3998.00000000,
                        Quantity = 2.00000000,
                        Commission = 7.99600000,
                        CommissionAsset = "USDT",
                        TradeId = 58
                    },
                    new FillModel
                    {
                        Price = 3997.00000000,
                        Quantity = 1.00000000,
                        Commission = 3.99700000,
                        CommissionAsset = "USDT",
                        TradeId = 59
                    },
                    new FillModel
                    {
                        Price = 3995.00000000,
                        Quantity = 1.00000000,
                        Commission = 3.99500000,
                        CommissionAsset = "USDT",
                        TradeId = 60
                    },
                }
            };

        /// <summary>
        ///     Возвращает модель ответа на запрос о создании нового ордера
        /// </summary>
        public static Common.Models.FullOrderResponseModel GetExpectedFullOrderResponseModel(
            string symbol,
            double price,
            double origQty,
            string status,
            string timeInForce,
            string orderType,
            OrderSideType sideType) =>
            new()
            {
                Symbol = symbol,
                OrderId = 28,
                ClientOrderId = "6gCrw2kRUAF9CvJDGP16IP",
                TransactTimeUnix = 1507725176595,
                OrderListId = -1,
                Price = price,
                OrigQty = origQty,
                ExecutedQty = 10.1,
                CumulativeQuoteQty = 10.2,
                Status = status,
                TimeInForce = timeInForce,
                OrderType = orderType,
                OrderSide = sideType,
                Fills = new List<Common.Models.FillModel>
                {
                    new Common.Models.FillModel
                    {
                        Price = 4000.00000000,
                        Quantity = 1.00000000,
                        Commission = 4.00000000,
                        CommissionAsset = "USDT",
                        TradeId = 56
                    },
                    new Common.Models.FillModel
                    {
                        Price = 3999.00000000,
                        Quantity = 5.00000000,
                        Commission = 19.99500000,
                        CommissionAsset = "USDT",
                        TradeId = 57
                    },
                    new Common.Models.FillModel
                    {
                        Price = 3998.00000000,
                        Quantity = 2.00000000,
                        Commission = 7.99600000,
                        CommissionAsset = "USDT",
                        TradeId = 58
                    },
                    new Common.Models.FillModel
                    {
                        Price = 3997.00000000,
                        Quantity = 1.00000000,
                        Commission = 3.99700000,
                        CommissionAsset = "USDT",
                        TradeId = 59
                    },
                    new Common.Models.FillModel
                    {
                        Price = 3995.00000000,
                        Quantity = 1.00000000,
                        Commission = 3.99500000,
                        CommissionAsset = "USDT",
                        TradeId = 60
                    },
                }
            };

        #endregion
    }
}
