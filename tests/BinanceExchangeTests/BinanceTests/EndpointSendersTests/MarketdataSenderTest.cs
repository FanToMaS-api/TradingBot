using BinanceExchange;
using BinanceExchange.Client;
using BinanceExchange.Client.Impl;
using BinanceExchange.EndpointSenders;
using BinanceExchange.EndpointSenders.Impl;
using BinanceExchange.Enums;
using BinanceExchange.Models;
using SharedForTest;
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
                    new List<DayPriceChangeModel>
                    {
                        TestHelper.GetBinanceDayPriceChangeModels(),
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    null,
                    "../../../BinanceTests/Jsons/Marketdata/DAY_PRICE_CHANGE_SYMBOL_IS_NULL.json",
                    new List<DayPriceChangeModel>
                    {
                        TestHelper.GetBinanceDayPriceChangeModels(),
                        TestHelper.GetBinanceDayPriceChangeModels(),
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/DAY_PRICE_CHANGE_SYMBOL_IS_NULL.json",
                    new List<DayPriceChangeModel>
                    {
                        TestHelper.GetBinanceDayPriceChangeModels(),
                        TestHelper.GetBinanceDayPriceChangeModels(),
                    }
                },
            };

        /// <summary>
        ///     Пути к файлам и объекты для проверки для запроса списка свечей по монете
        /// </summary>
        public static IEnumerable<object[]> CandlestickData =>
            new List<object[]>
            {
                new object[]
                {
                    "../../../BinanceTests/Jsons/Marketdata/CANDLESTICK_DATA.json",
                    TestHelper.GetBinanceCandlestickModels()
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
                        TestHelper.CreatetBinanceSymbolPriceTickerModel("LTCBTC", 4.00000200)
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    null,
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_PRICE_TICKERS.json",
                    new List<SymbolPriceTickerModel>
                    {
                        TestHelper.CreatetBinanceSymbolPriceTickerModel("LTCBTC", 4.00000200),
                        TestHelper.CreatetBinanceSymbolPriceTickerModel("ETHBTC", 0.07946600)
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_PRICE_TICKERS.json",
                    new List<SymbolPriceTickerModel>
                    {
                        TestHelper.CreatetBinanceSymbolPriceTickerModel("LTCBTC", 4.00000200),
                        TestHelper.CreatetBinanceSymbolPriceTickerModel("ETHBTC", 0.07946600)
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
                        TestHelper.CreateBinanceSymbolOrderBookTickerModel("LTCBTC", 4.00000000, 431.00000000, 4.00000200, 9.00000000)
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    null,
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_ORDER_BOOK_TICKERS.json",
                    new List<SymbolOrderBookTickerModel>
                    {
                        TestHelper.CreateBinanceSymbolOrderBookTickerModel("LTCBTC", 4.00000000, 431.00000000, 4.00000200, 9.00000000),
                        TestHelper.CreateBinanceSymbolOrderBookTickerModel("ETHBTC", 0.07946700, 9.00000000, 100000.00000000, 1000.00000000)
                    }
                },

                /// тест при запросе информации о всех парах
                new object[]
                {
                    "",
                    "../../../BinanceTests/Jsons/Marketdata/SYMBOL_ORDER_BOOK_TICKERS.json",
                    new List<SymbolOrderBookTickerModel>
                    {
                        TestHelper.CreateBinanceSymbolOrderBookTickerModel("LTCBTC", 4.00000000, 431.00000000, 4.00000200, 9.00000000),
                        TestHelper.CreateBinanceSymbolOrderBookTickerModel("ETHBTC", 0.07946700, 9.00000000, 100000.00000000, 1000.00000000)
                    }
                },
            };

        #endregion

        #region Tests

        /// <summary>
        ///     Тест запроса текущих правил биржевой торговли и информации о символах
        /// </summary>
        [Fact(DisplayName = "Requesting current exchange trading rules and symbol information Test")]
        internal async Task<ExchangeInfoModel> GetExchangeInfoAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/EXCHANGE_INFO.json";

            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.EXCHANGE_INFO, filePath);
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = await marketdataSender.GetExchangeInfoAsync(CancellationToken.None);

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

            return result;
        }

        /// <summary>
        ///     Тест запроса списка ордеров для конкретной монеты
        /// </summary>
        [Fact(DisplayName = "Requesting a list of orders for a specific coin Test")]
        internal async Task<OrderBookModel> GetOrderBookAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/ORDER_BOOK.json";

            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.ORDER_BOOK, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
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

            return result;
        }

        /// <summary>
        ///     Тест запроса списка недавних сделок
        /// </summary>
        [Fact(DisplayName = "Recent trades list query Test")]
        internal async Task<IEnumerable<TradeModel>> GetRecentTradesAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/RECENT_TRADES.json";
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.RECENT_TRADES, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
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

            return result;
        }

        /// <summary>
        ///     Тест запроса списка исторических сделок
        /// </summary>
        [Fact(DisplayName = "The request for a list of historical trades Test")]
        internal async Task<IEnumerable<TradeModel>> GetOldTradesAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/OLD_TRADES.json";
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.OLD_TRADES, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
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

            return result;
        }

        /// <summary>
        ///     Тест запроса списка свечей по монете
        /// </summary>
        [Theory(DisplayName = "Request for a list of candlesticks by coin Test")]
        [MemberData(nameof(CandlestickData))]
        internal async Task<IEnumerable<CandlestickModel>> GetCandlestickAsync_Test(string filePath, CandlestickModel[] expected)
        {
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.CANDLESTICK_DATA, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetCandlestickAsync(null, cancellationToken: CancellationToken.None))
                .ToList();

            Assert.Equal(2, result.Count);
            var properties = typeof(CandlestickModel).GetProperties();
            for (var i = 0; i < expected.Length; i++)
            {
                TestExtensions.CheckingAssertions(expected[i], result[i]);
            }

            return result;
        }

        /// <summary>
        ///     Тест запроса текущей средней цены пары
        /// </summary>
        [Fact(DisplayName = "Request for the current average price of a pair Test")]
        internal async Task<AveragePriceModel> GetAveragePriceAsync_Test()
        {
            var filePath = "../../../BinanceTests/Jsons/Marketdata/AVERAGE_PRICE.json";
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.AVERAGE_PRICE, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = await marketdataSender.GetAveragePriceAsync(null, cancellationToken: CancellationToken.None);

            Assert.Equal(5, result.Mins);
            Assert.Equal(9.35751834, result.AveragePrice);

            return result;
        }

        /// <summary>
        ///     Тест запроса 24х часового изменения цены пары
        /// </summary>
        [Theory(DisplayName = "Request for a 24-hour change in the price of a pair Test")]
        [MemberData(nameof(DayPriceChangeData))]
        internal async Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync_Test(string symbol, string filePath, List<DayPriceChangeModel> expectedDtos)
        {
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.DAY_PRICE_CHANGE, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetDayPriceChangeAsync(symbol, cancellationToken: CancellationToken.None)).ToList();

            for (var i = 0; i < result.Count; i++)
            {
                var expected = expectedDtos[i];
                var actual = result[i];
                TestExtensions.CheckingAssertions(expected, actual);
            }

            return result;
        }

        /// <summary>
        ///     Тест запроса последней цены пары/пар
        /// </summary>
        [Theory(DisplayName = "Requesting the last price of a pair/pairs Test")]
        [MemberData(nameof(SymbolPriceTickerData))]
        internal async Task<IEnumerable<SymbolPriceTickerModel>> GetSymbolPriceTickerAsync_Test(string symbol, string filePath, List<SymbolPriceTickerModel> expectedDtos)
        {
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.SYMBOL_PRICE_TICKER, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetSymbolPriceTickerAsync(symbol, cancellationToken: CancellationToken.None)).ToList();

            for (var i = 0; i < result.Count; i++)
            {
                var expected = expectedDtos[i];
                var actual = result[i];
                TestExtensions.CheckingAssertions(expected, actual);
            }

            return result;
        }

        /// <summary>
        ///     Тест запроса лучшей цены/количества в стакане для символа или символов
        /// </summary>
        [Theory(DisplayName = "Requesting the best price/quantity in the order book for a symbol or symbols Test")]
        [MemberData(nameof(SymbolOrderBookTickerData))]
        internal async Task<IEnumerable<SymbolOrderBookTickerModel>> GetSymbolOrderBookTickerAsync_Test(
            string symbol,
            string filePath,
            List<SymbolOrderBookTickerModel> expectedDtos)
        {
            var clientFactory = TestHelper.CreateMockIHttpClientFactory(BinanceEndpoints.SYMBOL_ORDER_BOOK_TICKER, filePath);
            var options = TestHelper.CreateBinanceExchangeOptions("", "");
            IBinanceClient binanceClient = new BinanceClient(clientFactory, options);
            IMarketdataSender marketdataSender = new MarketdataSender(binanceClient);

            // Act
            var result = (await marketdataSender.GetSymbolOrderBookTickerAsync(symbol, cancellationToken: CancellationToken.None)).ToList();

            for (var i = 0; i < result.Count; i++)
            {
                var expected = expectedDtos[i];
                var actual = result[i];
                TestExtensions.CheckingAssertions(expected, actual);
            }

            return result;
        }

        #endregion
    }
}
