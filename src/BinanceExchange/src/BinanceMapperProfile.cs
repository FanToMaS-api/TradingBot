using AutoMapper;
using BinanceExchange.Enums.Helper;
using BinanceExchange.Models;
using Common.Models;

namespace BinanceExchange
{
    /// <summary>
    ///     Содержит профили маппинга для сущностей бинанса
    /// </summary>
    public class BinanceMapperProfile : Profile
    {
        public BinanceMapperProfile()
        {
            CreateMap<SymbolInfoModel, TradeObjectRuleTradingModel>()
                .ForMember(_ => _.Status, _ => _.MapFrom(_ => _.Status))
                .ForMember(_ => _.Name, _ => _.MapFrom(_ => _.Symbol));
            CreateMap<SymbolPriceTickerModel, TradeObjectNamePriceModel>()
                .ForMember(_ => _.Name, _ => _.MapFrom(_ => _.Symbol));
            CreateMap<PriceQtyPair, OrderModel>();

            CreateMap<Models.TradeFeeModel, Common.Models.TradeFeeModel>()
                .ForMember(_ => _.ShortName, _ => _.MapFrom(_ => _.Coin));
            CreateMap<CoinModel, TradeObject>()
                .ForMember(_ => _.ShortName, _ => _.MapFrom(_ => _.Coin));
            CreateMap<Models.DayPriceChangeModel, Common.Models.DayPriceChangeModel>();
            CreateMap<Models.CandlestickModel, Common.Models.CandlestickModel>();
            CreateMap<Models.BalanceModel, Common.Models.BalanceModel>();
            CreateMap<Models.AccountInformationModel, Common.Models.AccountInformationModel>();

            CreateMap<Models.OrderBookModel, Common.Models.OrderBookModel>();
            CreateMap<Models.TradeModel, Common.Models.TradeModel>();
            CreateMap<Models.MiniTickerStreamModel, Common.Models.MiniTickerStreamModel>();
            CreateMap<Models.TickerStreamModel, Common.Models.TradeObjectStreamModel>();

            CreateMap<Models.BookTickerStreamModel, Common.Models.BookTickerStreamModel>();
            CreateMap<Models.AggregateSymbolTradeStreamModel, Common.Models.AggregateTradeStreamModel>();
            CreateMap<Models.SymbolTradeStreamModel, Common.Models.TradeStreamModel>();
     
            CreateMap<Models.CancelOrderResponseModel, Common.Models.CancelOrderResponseModel>();
            CreateMap<Models.CheckOrderResponseModel, Common.Models.CheckOrderResponseModel>();

            CreateMap<Models.FillModel, Common.Models.FillModel>();
            CreateMap<Models.FullOrderResponseModel, Common.Models.FullOrderResponseModel>();

            CreateMap<Models.CandlestickStreamModel, Common.Models.CandlestickStreamModel>()
                .ForMember(_ => _.KineStartTimeUnix, _ => _.MapFrom(_ => _.Kline.KineStartTimeUnix))
                .ForMember(_ => _.KineStopTimeUnix, _ => _.MapFrom(_ => _.Kline.KineStopTimeUnix))
                .ForMember(_ => _.Interval, _ => _.MapFrom(_ => _.Kline.Interval.ToUrl()))
                .ForMember(_ => _.OpenPrice, _ => _.MapFrom(_ => _.Kline.OpenPrice))
                .ForMember(_ => _.MinPrice, _ => _.MapFrom(_ => _.Kline.MinPrice))
                .ForMember(_ => _.MaxPrice, _ => _.MapFrom(_ => _.Kline.MaxPrice))
                .ForMember(_ => _.ShortName, _ => _.MapFrom(_ => _.Symbol))
                .ForMember(_ => _.Volume, _ => _.MapFrom(_ => _.Kline.Volume))
                .ForMember(_ => _.ClosePrice, _ => _.MapFrom(_ => _.Kline.ClosePrice))
                .ForMember(_ => _.IsKlineClosed, _ => _.MapFrom(_ => _.Kline.IsKlineClosed))
                .ForMember(_ => _.QuoteAssetVolume, _ => _.MapFrom(_ => _.Kline.QuoteAssetVolume))
                .ForMember(_ => _.TradesNumber, _ => _.MapFrom(_ => _.Kline.TradesNumber))
                .ForMember(_ => _.BasePurchaseVolume, _ => _.MapFrom(_ => _.Kline.BasePurchaseVolume))
                .ForMember(_ => _.QuotePurchaseVolume, _ => _.MapFrom(_ => _.Kline.QuotePurchaseVolume));

            CreateMap<SymbolOrderBookTickerModel, BestSymbolOrderModel>()
                .ForMember(_ => _.BidQuantity, _ => _.MapFrom(_ => _.BidQty))
                .ForMember(_ => _.AskQuantity, _ => _.MapFrom(_ => _.AskQty));
            CreateMap<AccountTradingStatusModel, TradingAccountInfoModel>()
                .ForMember(_ => _.UpdateTimeUnix, _ => _.MapFrom(_ => _.Data.UpdateTimeUnix))
                .ForMember(_ => _.IsLocked, _ => _.MapFrom(_ => _.Data.IsLocked))
                .ForMember(_ => _.PlannedRecoverTimeUnix, _ => _.MapFrom(_ => _.Data.PlannedRecoverTimeUnix));
        }
    }
}
