using BinanceDataService.DataHandlers;
using DataServiceLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceDataService
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Добавить фабрику, предоставляющую методы для создания сервиса обработки данных
        /// </summary>
        public static void AddDataServiceFactory(this IServiceCollection services)
        {
            services.AddSingleton<IDataHandler, MarketdataStreamHandler>();
            services.AddSingleton<IDataService, BinanceDataService>();
        }
    }
}
