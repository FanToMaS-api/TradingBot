using BinanceExchange;
using BinanceExchange.Client;
using BinanceExchange.Client.Impl;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Enums;
using BinanceExchange.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinanceExchangeTests.BinanceTests.EndpointSendersTests
{
    /// <summary>
    ///     Класс, тестирующий <see cref="MarketdataSender"/>
    /// </summary>
    public class MarketdataSenderTest
    {
        #region Fields

        /// <summary>
        ///     Ожидаемый результат выполнения запроса 24го изменения цены
        /// </summary>
        private static readonly List<DayPriceChangeModel> _expectedDayPriceChange = new()
        {
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
            }
        };

        #endregion

        #region Data

        /// <summary>
        ///     Пути к файлам и объекты для проверки для запроса 24х часового изменения цены
        /// </summary>
        public static IEnumerable<object[]> DayPriceChangeData =>
            new List<object[]>
            {
                /// тест при запросе информации о паре
                new object[]
                {
                    "BNBBTC",
                    "../../../BinanceTests/Jsons/Marketdata/DAY_PRICE_CHANGE_SYMBOL.json",
                   _expectedDayPriceChange
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    null,
                    "../../../BinanceTests/Jsons/Marketdata/DAY_PRICE_CHANGE_SYMBOL_IS_NULL.json",
                    _expectedDayPriceChange
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/DAY_PRICE_CHANGE_SYMBOL_IS_NULL.json",
                    _expectedDayPriceChange
                },
            };

        /// <summary>
        ///     Пути к файлам и объекты для проверки для запроса списка свечей по монете
        /// </summary>
        public static IEnumerable<object[]> CandleStickData =>
            new List<object[]>
            {
                new object[]
                {
                    "../../../BinanceTests/Jsons/Marketdata/CANDLESTICK_DATA.json",
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
                    }
                }
            };

        /// <summary>
        ///     Пути к файлам и объекты для проверки для запроса последней цены пары/пар
        /// </summary>
        public static IEnumerable<object[]> SymbolPriceTickerData =>
            new List<object[]>
            {
                /// тест при запросе информации о паре
                new object[]
                {
                    "LTCBTC",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_PRICE_TICKER.json",
                    new List<SymbolPriceTickerModel>
                    {
                        new SymbolPriceTickerModel
                        {
                            Symbol = "LTCBTC",
                            Price = 4.00000200
                        }
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    null,
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_PRICE_TICKERS.json",
                    new List<SymbolPriceTickerModel>
                    {
                        new SymbolPriceTickerModel
                        {
                            Symbol = "LTCBTC",
                            Price = 4.00000200
                        },
                        new SymbolPriceTickerModel
                        {
                            Symbol = "ETHBTC",
                            Price = 0.07946600
                        },
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_PRICE_TICKERS.json",
                    new List<SymbolPriceTickerModel>
                    {
                        new SymbolPriceTickerModel
                        {
                            Symbol = "LTCBTC",
                            Price = 4.00000200
                        },
                        new SymbolPriceTickerModel
                        {
                            Symbol = "ETHBTC",
                            Price = 0.07946600
                        },
                    }
                },
            };

        /// <summary>
        ///     Пути к файлам и объекты для проверки для запроса последней цены пары/пар
        /// </summary>
        public static IEnumerable<object[]> SymbolOrderBookTickerData =>
            new List<object[]>
            {
                /// тест при запросе информации о паре
                new object[]
                {
                    "LTCBTC",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_ORDER_BOOK_TICKER.json",
                    new List<SymbolOrderBookTickerModel>
                    {
                        new SymbolOrderBookTickerModel
                        {
                            Symbol = "LTCBTC",
                            BidPrice = 4.00000000,
                            BidQty = 431.00000000,
                            AskPrice = 4.00000200,
                            AskQty = 9.00000000,
                        }
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    null,
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_ORDER_BOOK_TICKERS.json",
                    new List<SymbolOrderBookTickerModel>
                    {
                        new SymbolOrderBookTickerModel
                        {
                            Symbol = "LTCBTC",
                            BidPrice = 4.00000000,
                            BidQty = 431.00000000,
                            AskPrice = 4.00000200,
                            AskQty = 9.00000000,
                        },
                        new SymbolOrderBookTickerModel
                        {
                            Symbol = "ETHBTC",
                            BidPrice = 0.07946700,
                            BidQty = 9.00000000,
                            AskPrice = 100000.00000000,
                            AskQty = 1000.00000000,
                        },
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_ORDER_BOOK_TICKERS.json",
                    new List<SymbolOrderBookTickerModel>
                    {
                        new SymbolOrderBookTickerModel
                        {
                            Symbol = "LTCBTC",
                            BidPrice = 4.00000000,
                            BidQty = 431.00000000,
                            AskPrice = 4.00000200,
                            AskQty = 9.00000000,
                        },
                        new SymbolOrderBookTickerModel
                        {
                            Symbol = "ETHBTC",
                            BidPrice = 0.07946700,
                            BidQty = 9.00000000,
                            AskPrice = 100000.00000000,
                            AskQty = 1000.00000000,
                        },
                    }
                },
            };

        #endregion

        #region Tests

        /// <summary>
        ///     Тест запроса текущих правил биржевой торговли и информации о символах
        /// </summary>
        [Fact(DisplayName = "Requesting current exchange trading rules and symbol information Test")]
        public async Task GetExchangeInfoAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/EXCHANGE_INFO.json";

            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.EXCHANGE_INFO, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = await marketdataSender.GetExchangeInfoAsync(null, cancellationToken: CancellationToken.None);

            var expectedOrderTypes = new[]
            {
                OrderType.Limit,
                OrderType.LimitMaker,
                OrderType.Market,
                OrderType.StopLoss,
                OrderType.StopLossLimit,
                OrderType.TakeProfit,
                OrderType.TakeProfitLimit
            };

            Assert.Equal(2, result.Symbols.Count);
            for (var i = 0; i < 2; i++)
            {
                Assert.Equal($"ETHBTC_{i}", result.Symbols[i].Symbol);
                Assert.Equal(SymbolStatusType.Trading, result.Symbols[i].Status);
                Assert.Equal("ETH", result.Symbols[i].BaseAsset);
                Assert.Equal("BTC", result.Symbols[i].QuoteAsset);
                Assert.Equal(i + 1, result.Symbols[i].BaseAssetPrecision);
                Assert.Equal(i + 2, result.Symbols[i].QuotePrecision);
                Assert.Equal(expectedOrderTypes, result.Symbols[i].OrderTypes);
                Assert.True(result.Symbols[i].IsIcebergAllowed);
                Assert.True(result.Symbols[i].IsOcoAllowed);
            }
        }

        /// <summary>
        ///     Тест запроса списка ордеров для конкретной монеты
        /// </summary>
        [Fact(DisplayName = "Requesting a list of orders for a specific coin Test")]
        public async Task GetOrderBookAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/ORDER_BOOK.json";

            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.ORDER_BOOK, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = await marketdataSender.GetOrderBookAsync(null, cancellationToken: CancellationToken.None);

            Assert.Equal(2, result.Bids.Count);
            Assert.Equal(2, result.Asks.Count);
            Assert.Equal(4.00000000, result.Bids[0].Price);
            Assert.Equal(431.00000000, result.Bids[0].Quantity);
            Assert.Equal(5.00000000, result.Bids[1].Price);
            Assert.Equal(15.00000000, result.Bids[1].Quantity);
            Assert.Equal(4.00000200, result.Asks[0].Price);
            Assert.Equal(12.00000000, result.Asks[0].Quantity);
            Assert.Equal(6.00000200, result.Asks[1].Price);
            Assert.Equal(18.00000000, result.Asks[1].Quantity);
        }

        /// <summary>
        ///     Тест запроса списка недавних сделок
        /// </summary>
        [Fact(DisplayName = "Recent trades list query Test")]
        public async Task GetRecentTradesAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/RECENT_TRADES.json";
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.RECENT_TRADES, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetRecentTradesAsync(null, cancellationToken: CancellationToken.None)).ToList();

            Assert.Single(result);
            Assert.Equal(28457, result[0].Id);
            Assert.Equal(4.00000100, result[0].Price);
            Assert.Equal(12.00000000, result[0].Quantity);
            Assert.Equal(48.000012, result[0].QuoteQty);
            Assert.Equal(1499865549590, result[0].TimeUnix);
            Assert.True(result[0].IsBuyerMaker);
            Assert.True(result[0].IsBestMatch);
        }

        /// <summary>
        ///     Тест запроса списка исторических сделок
        /// </summary>
        [Fact(DisplayName = "The request for a list of historical trades Test")]
        public async Task GetOldTradesAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/OLD_TRADES.json";
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.OLD_TRADES, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetOldTradesAsync(null, cancellationToken: CancellationToken.None)).ToList();

            Assert.Single(result);
            Assert.Equal(28457, result[0].Id);
            Assert.Equal(4.00000100, result[0].Price);
            Assert.Equal(12.00000000, result[0].Quantity);
            Assert.Equal(48.000012, result[0].QuoteQty);
            Assert.Equal(1499865549590, result[0].TimeUnix);
            Assert.True(result[0].IsBuyerMaker);
            Assert.True(result[0].IsBestMatch);
        }

        /// <summary>
        ///     Тест запроса списка свечей по монете
        /// </summary>
        [Theory(DisplayName = "Request for a list of candlesticks by coin Test")]
        [MemberData(nameof(CandleStickData))]
        internal async Task GetCandleStickAsyncTest(string filePath, CandlestickModel[] expected)
        {
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.CANDLESTICK_DATA, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetCandlestickAsync(null, cancellationToken: CancellationToken.None))
                .ToList();

            Assert.Equal(2, result.Count);
            var properties = typeof(CandlestickModel).GetProperties();
            for (var i = 0; i < expected.Length; i++)
            {
                for (var j = 0; j < properties.Length; j++)
                {
                    Assert.Equal(properties[j].GetValue(expected[i]), properties[j].GetValue(result[i]));
                }
            }
        }

        /// <summary>
        ///     Тест запроса текущей средней цены пары
        /// </summary>
        [Fact(DisplayName = "Request for the current average price of a pair Test")]
        public async Task GetAveragePriceAsyncTest()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/AVERAGE_PRICE.json";
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.AVERAGE_PRICE, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = await marketdataSender.GetAveragePriceAsync(null, cancellationToken: CancellationToken.None);

            Assert.Equal(5, result.Mins);
            Assert.Equal(9.35751834, result.AveragePrice);
        }

        /// <summary>
        ///     Тест запроса 24х часового изменения цены пары
        /// </summary>
        [Theory(DisplayName = "Request for a 24-hour change in the price of a pair Test")]
        [MemberData(nameof(DayPriceChangeData))]
        internal async Task GetDayPriceChangeAsyncTest(string symbol, string filePath, List<DayPriceChangeModel> expectedDtos)
        {
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.DAY_PRICE_CHANGE, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetDayPriceChangeAsync(symbol, cancellationToken: CancellationToken.None)).ToList();

            for (var i = 0; i < result.Count; i++)
            {
                var dto = expectedDtos[i];
                var actual = result[i];
                var properties = dto.GetType().GetProperties();
                foreach (var property in properties)
                {
                    Assert.Equal(property.GetValue(dto), property.GetValue(actual));
                }
            }
        }

        /// <summary>
        ///     Тест запроса последней цены пары/пар
        /// </summary>
        [Theory(DisplayName = "Requesting the last price of a pair/pairs Test")]
        [MemberData(nameof(SymbolPriceTickerData))]
        internal async Task GetSymbolPriceTickerAsync(string symbol, string filePath, List<SymbolPriceTickerModel> expectedDtos)
        {
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.SYMBOL_PRICE_TICKER, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetSymbolPriceTickerAsync(symbol, cancellationToken: CancellationToken.None)).ToList();

            for (var i = 0; i < result.Count; i++)
            {
                var dto = expectedDtos[i];
                var actual = result[i];
                var properties = dto.GetType().GetProperties();
                foreach (var property in properties)
                {
                    Assert.Equal(property.GetValue(dto), property.GetValue(actual));
                }
            }
        }

        /// <summary>
        ///     Тест запроса лучшей цены/количества в стакане для символа или символов
        /// </summary>
        [Theory(DisplayName = "Requesting the best price/quantity in the order book for a symbol or symbols Test")]
        [MemberData(nameof(SymbolOrderBookTickerData))]
        internal async Task GetSymbolOrderBookTickerAsyncAsync(string symbol, string filePath, List<SymbolOrderBookTickerModel> expectedDtos)
        {
            using var client = TestHelper.CreateMockHttpClient(BinanceEndpoints.SYMBOL_ORDER_BOOK_TICKER, filePath);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetSymbolOrderBookTickerAsync(symbol, cancellationToken: CancellationToken.None)).ToList();

            for (var i = 0; i < result.Count; i++)
            {
                var dto = expectedDtos[i];
                var actual = result[i];
                var properties = dto.GetType().GetProperties();
                foreach (var property in properties)
                {
                    Assert.Equal(property.GetValue(dto), property.GetValue(actual));
                }
            }
        }

        #endregion
    }
}
