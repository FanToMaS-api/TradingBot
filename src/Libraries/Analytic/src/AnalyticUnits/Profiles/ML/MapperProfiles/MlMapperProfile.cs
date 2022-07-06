using Analytic.AnalyticUnits.Profiles.ML.Models.Impl;
using Analytic.Filters.Enums;
using AutoMapper;
using BinanceDatabase.Entities;

namespace Analytic.AnalyticUnits.Profiles.ML.MapperProfiles
{
    /// <summary>
    ///     Профиль маппинга для сущностей
    /// </summary>
    public class MlMapperProfile : Profile
    {
        #region .ctor

        /// <inheritdoc cref="MlMapperProfile" />
        public MlMapperProfile()
        {
            CreateMap<HotMiniTickerEntity, HotTradeObjectModel>()
                .ForMember(_ => _.ClosePrice, _ => _.MapFrom(_ => (float)_.Price))
                .ForMember(_ => _.ClosePriceDouble, _ => _.MapFrom(_ => _.Price))
                .ForMember(_ => _.EventTime, _ => _.MapFrom(_ => _.ReceivedTime))
                .ForMember(_ => _.AggregateDataInterval, _ => _.MapFrom(_ => AggregateDataIntervalType.Default));

            CreateMap<MiniTickerEntity, TradeObjectModel>()
                .ForMember(_ => _.MinPrice, _ => _.MapFrom(_ => (float)_.MinPrice))
                .ForMember(_ => _.MaxPrice, _ => _.MapFrom(_ => (float)_.MaxPrice))
                .ForMember(_ => _.OpenPrice, _ => _.MapFrom(_ => (float)_.OpenPrice))
                .ForMember(_ => _.ClosePrice, _ => _.MapFrom(_ => (float)_.ClosePrice))
                .ForMember(_ => _.ClosePriceDouble, _ => _.MapFrom(_ => _.ClosePrice))
                .ForMember(_ => _.BasePurchaseVolume, _ => _.MapFrom(_ => (float)_.BasePurchaseVolume))
                .ForMember(_ => _.QuotePurchaseVolume, _ => _.MapFrom(_ => (float)_.QuotePurchaseVolume))
                .ForMember(_ => _.PriceDeviationPercent, _ => _.MapFrom(_ => (float)_.PriceDeviationPercent))
                .ForMember(_ => _.AggregateDataInterval, _ => _.MapFrom(_ => (int)_.AggregateDataInterval));
        }

        #endregion
    }
}
