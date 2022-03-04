using AutoMapper;
using Common.Models;
using BinanceExchange.Models;

namespace BinanceExchange
{
    /// <summary>
    ///     Содержит профили маппинга для сущностей бинанса
    /// </summary>
    public class BinanceMapperProfile : Profile
    {
        public BinanceMapperProfile()
        {
            CreateMap<Models.TradeFeeModel, Common.Models.TradeFeeModel>();
            CreateMap<SymbolInfoModel, SymbolRuleTradingModel>();
            CreateMap<Models.CoinModel, Common.Models.CoinModel>();
            CreateMap<Models.CandlestickModel, Common.Models.CandlestickModel>();
            CreateMap<PriceQtyPair, OrderModel>();
            CreateMap<Models.OrderBookModel, Common.Models.OrderBookModel>();
            CreateMap<Models.TradeModel, Common.Models.TradeModel>();
            CreateMap<AccountTradingStatusModel, TradingAccountInfoModel>()
                .ForMember(_ => _.UpdateTimeUnix, _ => _.MapFrom(_ => _.Data.UpdateTimeUnix))
                .ForMember(_ => _.IsLocked, _ => _.MapFrom(_ => _.Data.IsLocked))
                .ForMember(_ => _.PlannedRecoverTimeUnix, _ => _.MapFrom(_ => _.Data.PlannedRecoverTimeUnix));
        }
    }
}
