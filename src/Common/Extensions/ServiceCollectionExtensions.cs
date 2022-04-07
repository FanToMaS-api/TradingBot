using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.Extensions
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
            where TOptions : OptionsBase
        {
            var options = (TOptions)Activator.CreateInstance(typeof(TOptions));

            if (options == null)
            {
                throw new NullReferenceException($"Failed create instance options '{typeof(TOptions)}'");
            }

            configuration.GetSection(options.Name).Bind(options);
            services.AddSingleton(options);

            return options;
        }
    }
}
