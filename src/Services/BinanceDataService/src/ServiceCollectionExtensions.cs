using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace BinanceDataService
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Добавить фабрику для создания обработчиков данных
        /// </summary>
        public static void AddDataServiceFactory(this IServiceCollection services)
            => services.AddSingleton<IBinanceDataServiceFactory, BinanceDataServiceFactory>();
    }
}
