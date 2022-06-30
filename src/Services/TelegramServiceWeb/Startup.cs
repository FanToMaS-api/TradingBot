using BinanceDatabase;
using Common.Initialization;
using ExtensionsLibrary;
using Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TelegramServiceDatabase;
using TelegramServiceWeb.Configuration;

namespace TelegramServiceWeb
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
                await Task.Factory.StartNew(
                    async () => await telegramService.StartAsync(),
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                    TaskScheduler.Default);
            });

            // постгрес 6 имеет критически важные изменения пр обработке DateTime
            // https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
            // https://github.com/nhibernate/nhibernate-core/issues/2994
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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
