using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ExtensionsLibrary
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Регистрирует настройки сервиса
        /// </summary>
        public static TOptions LoadOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class
        {
            var options = (TOptions)Activator.CreateInstance(typeof(TOptions));

            if (options == null)
            {
                throw new NullReferenceException($"Failed create instance options '{typeof(TOptions)}'");
            }

            var name = (string)typeof(TOptions).GetProperty("Name").GetValue(options)
                ?? throw new Exception("Service options must have property 'Name'");
            configuration.GetSection(name).Bind(options);
            services.AddSingleton(options);

            return options;
        }
    }
}
