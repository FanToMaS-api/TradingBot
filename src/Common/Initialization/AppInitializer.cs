using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Common.Initialization
{
    /// <summary>
    ///     Расширение для запуска сервисов при старте приложения
    /// </summary>
    public static class AppInitializer
    {
        /// <summary>
        ///     Настройка для инициализации
        /// </summary>
        public static void ConfigureForInitialization<T>(
            this IServiceCollection services,
            Func<T, IServiceProvider, Task> action)
        {
            services.ConfigureForInitialization(
                async sp =>
                {
                    var service = sp.GetRequiredService<T>();
                    await action.Invoke(service, sp);
                });
        }

        /// <summary>
        ///     Настройка для инициализации
        /// </summary>
        public static void ConfigureForInitialization<T>(this IServiceCollection services, Func<T, Task> action) =>
            services.ConfigureForInitialization<T>(async (service, _) => await action.Invoke(service));

        /// <summary>
        ///     Настройка для инициализации
        /// </summary>
        public static void ConfigureForInitialization(this IServiceCollection services, Func<IServiceProvider, Task> action) =>
            services.Configure<AppInitializerOptions>(options => { options.InitActions.Add(action); });

        /// <summary>
        ///     Выполняет заданные действия
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider services)
        {
            try
            {
                var options = services.GetRequiredService<IOptions<AppInitializerOptions>>().Value;

                foreach (var action in options.InitActions)
                {
                    await action(services);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
