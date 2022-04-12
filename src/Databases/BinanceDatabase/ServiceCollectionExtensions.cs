using BinanceDatabase.Repositories;
using BinanceDatabase.Repositories.ColdRepositories;
using BinanceDatabase.Repositories.ColdRepositories.Impl;
using BinanceDatabase.Repositories.HotRepositories;
using BinanceDatabase.Repositories.HotRepositories.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;

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
        private const string POSTGRES_CONNECTION_STRING = nameof(POSTGRES_CONNECTION_STRING);
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Добавить поддержку БД
        /// </summary>
        public static void AddBinanceDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>(POSTGRES_CONNECTION_STRING);
            Log.Trace($"{POSTGRES_CONNECTION_STRING}='{connectionString}'");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddScoped<IMiniTickerRepository, MiniTickerRepository>();
            services.AddScoped<IHotMiniTickerRepository, HotMiniTickerRepository>();
            services.AddScoped<IPredictionRepository, PredictionRepository>();
            services.AddScoped<IColdUnitOfWork, ColdUnitOfWork>();
            services.AddScoped<IHotUnitOfWork, HotUnitOfWork>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IBinanceDbContextFactory, BinanceDbContextFactory>();
        }

        /// <summary>
        ///     Применение миграций базы данных
        /// </summary>
        public static void ApplyDatabaseMigration(this IServiceProvider serviceProvider)
        {
            try
            {
                Log.Info("Applying database migrations...");

                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetService<AppDbContext>();

                db.Database.Migrate();

                Log.Info("Database migrations successfully applied");
            }
            catch (Exception ex)
            {
                throw new Exception("Error applying database migrations", ex);
            }
        }
    }
}
