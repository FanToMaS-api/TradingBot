using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TelegramServiceDatabase;
using TelegramServiceDatabase.Repositories;
using TelegramServiceDatabase.Repositories.Impl;

namespace BinanceDatabase
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Строка подключения к бд
        /// </summary>
        private const string TELEGRAM_POSTGRES_CONNECTION_STRING = nameof(TELEGRAM_POSTGRES_CONNECTION_STRING);
        private static readonly ILoggerDecorator Log = LoggerManager.CreateDefaultLogger();

        /// <summary>
        ///     Добавить поддержку БД
        /// </summary>
        public static void AddBinanceDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>(TELEGRAM_POSTGRES_CONNECTION_STRING);
            Log.TraceAsync($"{TELEGRAM_POSTGRES_CONNECTION_STRING}='{connectionString}'");

            services.AddDbContext<TelegramDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddScoped<IUserStateRepository, UserStateRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITelegramDbUnitOfWork, TelegramDbUnitOfWork>();
            services.AddSingleton<ITelegramDatabaseFactory, TelegramDatabaseFactory>();
        }

        /// <summary>
        ///     Применение миграций базы данных
        /// </summary>
        public static void ApplyDatabaseMigration(this IServiceProvider serviceProvider)
        {
            try
            {
                Log.InfoAsync("Applying database migrations...");

                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetService<TelegramDbContext>();

                db.Database.Migrate();

                Log.InfoAsync("Database migrations successfully applied");
            }
            catch (Exception ex)
            {
                throw new Exception("Error applying database migrations", ex);
            }
        }
    }
}
