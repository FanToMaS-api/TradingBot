using BinanceDatabase;
using Common.Initialization;
using ExtensionsLibrary;
using Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TelegramService.Configuration;
using TelegramServiceDatabase;

namespace TelegramService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTelegramLogger(Configuration);
            services.AddBinanceDatabase(Configuration);
            services.AddTelegramDatabase(Configuration);
            services.AddRazorPages();
            services.LoadOptions<TelegramServiceConfig>(Configuration);
            services.AddSingleton<ITelegramService, TelegramService>();
            services.ConfigureForInitialization<ITelegramService>(async telegramService =>
            {
                await telegramService.StartAsync();
            });
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseRouting();

            serviceProvider.ApplyTelegramDatabaseMigration();
            serviceProvider.ApplyBinanceDatabaseMigration();

            var task = Task.Run(async () => await AppInitializer.InitializeAsync(serviceProvider));
            Task.WaitAll(task);
        }
    }
}
