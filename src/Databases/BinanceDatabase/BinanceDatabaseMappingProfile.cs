using AutoMapper;
using BinanceDatabase.Entities;
using Common.Extensions;
using Common.Models;

namespace BinanceDatabase
{
    /// <summary>
    ///     Профиль маппинга для сущностей базы данных
    /// </summary>
    public class BinanceDatabaseMappingProfile : Profile
    {
        /// <inheritdoc cref="BinanceDatabaseMappingProfile"/>
        public BinanceDatabaseMappingProfile()
        {
            CreateMap<MiniTradeObjectStreamModel, HotMiniTickerEntity>()
                .ForMember(_ => _.Price, _ => _.MapFrom(_ => _.ClosePrice))
                .ForMember(_ => _.Pair, _ => _.MapFrom(_ => _.ShortName))
                .ForMember(_ => _.ReceivedTime, _ => _.MapFrom(_ => _.EventTimeUnix.FromUnixToDateTime()));

            CreateMap<MiniTradeObjectStreamModel, MiniTickerEntity>()
                .ForMember(_ => _.EventTime, _ => _.MapFrom(_ => _.EventTimeUnix.FromUnixToDateTime()));
        }
    }
}
