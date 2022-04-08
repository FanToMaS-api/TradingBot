using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BinanceExchange;
using BinanceExchange.Enums.Helper;
using BinanceExchange.Models;
using Common.Models;
using Xunit;

namespace BinanceExchangeTests.BinanceTests.ImplTests
{
    /// <summary>
    ///     Тесты проверяющие правильность маппинга сущностей
    /// </summary>
    public class MarketdataStreamMapperTests
    {
        #region Const

        private const double SomeDouble = 1.5;
        private const string SomeString = "asd";
        private const long SomeLong = 51651;
        private readonly IMapper _mapper;

        #endregion

        #region .ctor

        public MarketdataStreamMapperTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BinanceMapperProfile>();
            });
            _mapper = new Mapper(config);
        }

        #endregion

        #region Tests

        /// <summary>
        ///     Тест проверки маппинга <see cref="AggregateTradeStreamModel"/>
        /// </summary>
        [Fact(DisplayName = "Mapping aggregate trade stream model Test")]
        public void AggregateTradeStreamModelMapping_Test()
        {
            var originalObject = new AggregateSymbolTradeStreamModel()
            {
                AggregateTradeId = SomeLong,
                EventTimeUnix = SomeLong,
                FirstTradeId = SomeLong,
                LastTradeId = SomeLong,
                IsMarketMaker = true,
                Price = SomeDouble,
                Quantity = SomeDouble,
                Symbol = SomeString,
                TradeTimeUnix = SomeLong,
            };

            var result = _mapper.Map<AggregateTradeStreamModel>(originalObject);
            Assert.Equal(originalObject.Price, result.Price);
            Assert.Equal(originalObject.AggregateTradeId, result.AggregateTradeId);
            Assert.Equal(originalObject.EventTimeUnix, result.EventTimeUnix);
            Assert.Equal(originalObject.FirstTradeId, result.FirstTradeId);
            Assert.Equal(originalObject.LastTradeId, result.LastTradeId);
            Assert.Equal(originalObject.IsMarketMaker, result.IsMarketMaker);
            Assert.Equal(originalObject.Price, result.Price);
            Assert.Equal(originalObject.Quantity, result.Quantity);
            Assert.Equal(originalObject.Symbol, result.ShortName);
            Assert.Equal(originalObject.TradeTimeUnix, result.TradeTimeUnix);
        }

        /// <summary>
        ///     Тест проверки маппинга <see cref="BookTickerStreamModel"/>
        /// </summary>
        [Fact(DisplayName = "Mapping book ticker stream model Test")]
        public void BookTickerStreamModelMapping_Test()
        {
            var originalObject = new BinanceExchange.Models.BookTickerStreamModel()
            {
                BestAskPrice = SomeDouble,
                BestBidPrice = SomeDouble,
                Symbol = SomeString,
                BestAskQuantity = SomeDouble,
                BestBidQuantity = SomeDouble,
                OrderBookUpdatedId = SomeLong
            };

            var result = _mapper.Map<Common.Models.BookTickerStreamModel>(originalObject);
            Assert.Equal(originalObject.BestAskPrice, result.BestAskPrice);
            Assert.Equal(originalObject.BestBidPrice, result.BestBidPrice);
            Assert.Equal(originalObject.BestAskQuantity, result.BestAskQuantity);
            Assert.Equal(originalObject.BestBidQuantity, result.BestBidQuantity);
            Assert.Equal(originalObject.OrderBookUpdatedId, result.OrderBookUpdatedId);
            Assert.Equal(originalObject.Symbol, result.ShortName);
        }

        /// <summary>
        ///     Тест проверки маппинга <see cref="TradeObjectStreamModel"/>
        /// </summary>
        [Fact(DisplayName = "Mapping trade object stream model Test")]
        public void TradeObjectStreamModelMapping_Test()
        {
            var originalObject = new TickerStreamModel()
            {
                EventTimeUnix = SomeLong,
                Symbol = SomeString,
                MaxPrice = SomeDouble,
                MinPrice = SomeDouble,
                OpenPrice = SomeDouble,
                AllBaseVolume = SomeDouble,
                AllQuoteVolume = SomeDouble,
                BestAskPrice = SomeDouble,
                BestAskQuantity = SomeDouble,
                BestBidPrice = SomeDouble,
                BestBidQuantity = SomeDouble,
                FirstPrice = SomeDouble,
                LastPrice = SomeDouble,
                FirstTradeId = SomeLong,
                LastTradeId = SomeLong,
                LastQuantity = SomeDouble,
                Price = SomeDouble,
                PricePercentChange = SomeDouble,
                StatisticCloseTimeUnix = SomeLong,
                StatisticOpenTimeUnix = SomeLong,
                TradeNumber = SomeLong,
                WeightedAveragePrice = SomeDouble,
            };

            var result = _mapper.Map<TradeObjectStreamModel>(originalObject);
            Assert.Equal(originalObject.EventTimeUnix, result.EventTimeUnix);
            Assert.Equal(originalObject.Price, result.Price);
            Assert.Equal(originalObject.OpenPrice, result.OpenPrice);
            Assert.Equal(originalObject.MinPrice, result.MinPrice);
            Assert.Equal(originalObject.MaxPrice, result.MaxPrice);
            Assert.Equal(originalObject.AllBaseVolume, result.AllBaseVolume);
            Assert.Equal(originalObject.AllQuoteVolume, result.AllQuoteVolume);
            Assert.Equal(originalObject.Symbol, result.ShortName);
            Assert.Equal(originalObject.BestAskPrice, result.BestAskPrice);
            Assert.Equal(originalObject.BestAskQuantity, result.BestAskQuantity);
            Assert.Equal(originalObject.BestBidPrice, result.BestBidPrice);
            Assert.Equal(originalObject.BestBidQuantity, result.BestBidQuantity);
            Assert.Equal(originalObject.FirstPrice, result.FirstPrice);
            Assert.Equal(originalObject.LastPrice, result.LastPrice);
            Assert.Equal(originalObject.FirstTradeId, result.FirstTradeId);
            Assert.Equal(originalObject.LastTradeId, result.LastTradeId);
            Assert.Equal(originalObject.LastQuantity, result.LastQuantity);
            Assert.Equal(originalObject.PricePercentChange, result.PricePercentChange);
            Assert.Equal(originalObject.StatisticCloseTimeUnix, result.StatisticCloseTimeUnix);
            Assert.Equal(originalObject.StatisticOpenTimeUnix, result.StatisticOpenTimeUnix);
            Assert.Equal(originalObject.TradeNumber, result.TradeNumber);
            Assert.Equal(originalObject.WeightedAveragePrice, result.WeightedAveragePrice);
        }

        /// <summary>
        ///     Тест проверки маппинга <see cref="TradeStreamModel"/>
        /// </summary>
        [Fact(DisplayName = "Mapping trade stream model Test")]
        public void TradeStreamModelMapping_Test()
        {
            var originalObject = new SymbolTradeStreamModel()
            {
                EventTimeUnix = SomeLong,
                Symbol = SomeString,
                BuyerOrderId = SomeLong,
                IsMarketMaker = true,
                Quantity = SomeDouble,
                SellerOrderId = SomeLong,
                TradeId = SomeLong,
                TradeTimeUnix = SomeLong,
                Price = SomeDouble,
            };

            var result = _mapper.Map<TradeStreamModel>(originalObject);
            Assert.Equal(originalObject.EventTimeUnix, result.EventTimeUnix);
            Assert.Equal(originalObject.Symbol, result.ShortName);
            Assert.Equal(originalObject.BuyerOrderId, result.BuyerOrderId);
            Assert.Equal(originalObject.IsMarketMaker, result.IsMarketMaker);
            Assert.Equal(originalObject.Quantity, result.Quantity);
            Assert.Equal(originalObject.SellerOrderId, result.SellerOrderId);
            Assert.Equal(originalObject.TradeId, result.TradeId);
            Assert.Equal(originalObject.TradeTimeUnix, result.TradeTimeUnix);
            Assert.Equal(originalObject.Price, result.Price);
        }

        /// <summary>
        ///     Тест проверки маппинга <see cref="CandlestickStreamModel"/>
        /// </summary>
        [Fact(DisplayName = "Mapping сandlestick stream model Test")]
        public void CandlestickStreamModelMapping_Test()
        {
            var originalObject = new BinanceExchange.Models.CandlestickStreamModel()
            {
                EventTimeUnix = SomeLong,
                Symbol = SomeString,
                Kline = new KlineModel
                {
                    BasePurchaseVolume = SomeDouble,
                    ClosePrice = SomeDouble,
                    FirstTradeId = SomeLong,
                    interval = "1M",
                    IsKlineClosed = true,
                    KineStartTimeUnix = SomeLong,
                    KineStopTimeUnix = SomeLong,
                    LastTradeId = SomeLong,
                    MaxPrice = SomeDouble,
                    MinPrice = SomeDouble,
                    OpenPrice = SomeDouble,
                    QuoteAssetVolume = SomeDouble,
                    QuotePurchaseVolume = SomeDouble,
                    TradesNumber = 123,
                    Symbol = SomeString,
                    Volume = SomeDouble,
                }
            };

            var result = _mapper.Map<Common.Models.CandlestickStreamModel>(originalObject);
            Assert.Equal(originalObject.Symbol, result.ShortName);
            Assert.Equal(originalObject.EventTimeUnix, result.EventTimeUnix);
            Assert.Equal(originalObject.Kline.BasePurchaseVolume, result.BasePurchaseVolume);
            Assert.Equal(originalObject.Kline.ClosePrice, result.ClosePrice);
            Assert.Equal(originalObject.Kline.IsKlineClosed, result.IsKlineClosed);
            Assert.Equal(originalObject.Kline.KineStartTimeUnix, result.KineStartTimeUnix);
            Assert.Equal(originalObject.Kline.KineStopTimeUnix, result.KineStopTimeUnix);
            Assert.Equal(originalObject.Kline.MaxPrice, result.MaxPrice);
            Assert.Equal(originalObject.Kline.MinPrice, result.MinPrice);
            Assert.Equal(originalObject.Kline.OpenPrice, result.OpenPrice);
            Assert.Equal(originalObject.Kline.QuoteAssetVolume, result.QuoteAssetVolume);
            Assert.Equal(originalObject.Kline.QuotePurchaseVolume, result.QuotePurchaseVolume);
            Assert.Equal(originalObject.Kline.Volume, result.Volume);
            Assert.Equal(originalObject.Kline.TradesNumber, result.TradesNumber);
            Assert.Equal(BinanceExchange.Enums.CandlestickIntervalType.OneMonth.ToUrl(), result.Interval);
            Assert.Equal(originalObject.Kline.QuoteAssetVolume, result.QuoteAssetVolume);
        }

        /// <summary>
        ///     Тест проверки маппинга <see cref="MiniTradeObjectStreamModel"/>
        /// </summary>
        [Fact(DisplayName = "Mapping mini trade object stream model Test")]
        public void MiniTradeObjectStreamModelMapping_Test()
        {
            var originalObject = new MiniTickerStreamModel()
            {
                EventTimeUnix = SomeLong,
                Symbol = SomeString,
                BasePurchaseVolume = SomeDouble,
                QuotePurchaseVolume = SomeDouble,
                MaxPrice = SomeDouble,
                MinPrice = SomeDouble,
                OpenPrice = SomeDouble,
                ClosePrice = SomeDouble,
            };

            var result = _mapper.Map<MiniTradeObjectStreamModel>(originalObject);
            Assert.Equal(originalObject.EventTimeUnix, result.EventTimeUnix);
            Assert.Equal(originalObject.ClosePrice, result.ClosePrice);
            Assert.Equal(originalObject.OpenPrice, result.OpenPrice);
            Assert.Equal(originalObject.MinPrice, result.MinPrice);
            Assert.Equal(originalObject.MaxPrice, result.MaxPrice);
            Assert.Equal(originalObject.BasePurchaseVolume, result.BasePurchaseVolume);
            Assert.Equal(originalObject.QuotePurchaseVolume, result.QuotePurchaseVolume);
            Assert.Equal(originalObject.Symbol, result.ShortName);
        }

        #endregion
    }
}
