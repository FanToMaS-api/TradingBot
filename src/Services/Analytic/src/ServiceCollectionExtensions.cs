using Analytic.Binance;
using Microsoft.Extensions.DependencyInjection;

namespace Analytic
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Добавить сервис аналитики для бинанс биржи
        /// </summary>
        public static void AddBinanceAnalyticService(this IServiceCollection services)
            => services.AddSingleton<IAnalyticService, BinanceAnalyticService>();
    }
}
