using AutoMapper;
using Common.Models;
using ExchangeLibrary.Binance.Models;

namespace ExchangeLibrary.Binance
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
        }
    }
}
