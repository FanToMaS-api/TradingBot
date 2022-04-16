using Common.Extensions;
using Logger.Configuration;
using Logger.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Telegram.Client;

namespace Logger
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Добавить логгер с расширением - отпрвкой сообщений в телеграм
        /// </summary>
        public static void AddTelegramLogger(this IServiceCollection services, IConfiguration configuration)
        {
            var options = services.LoadOptions<TelegramLoggerConfiguration>(configuration);
            services.AddSingleton<BaseLoggerDecorator>();
            services.AddSingleton<ILoggerDecorator, TelegramLoggerDecorator>(_ =>
            {
                var baseLogger = _.GetRequiredService<BaseLoggerDecorator>();
                var client = _.GetRequiredService<ITelegramClient>();

                return new TelegramLoggerDecorator(baseLogger, client, LogLevel.Error, options);
            });
        }
    }
}
