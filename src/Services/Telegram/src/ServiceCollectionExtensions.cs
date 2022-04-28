using ExtensionsLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Client;
using Telegram.Client.Impl;
using Telegram.Configuration;

namespace Telegram
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Добавить клиент отправки сообщений в телеграм
        /// </summary>
        public static void AddTelegramClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.LoadOptions<TelegramOptions>(configuration);
            services.AddSingleton<ITelegramClient, TelegramClient>();
        }
    }
}
