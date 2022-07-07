using Analytic;
using Analytic.AnalyticUnits.Profiles.ML.MapperProfiles;
using AutoMapper;
using BinanceDatabase;
using BinanceExchange;
using Common.Initialization;
using ExtensionsLibrary;
using Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis;
using Scheduler;
using SignalsSender.Configuration;
using System;
using System.Threading.Tasks;
using Telegram;

namespace SignalsSender
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMappingProfiles(
                new BinanceMapperProfile(),
                new MlMapperProfile());

            services.AddTelegramClient(Configuration);
            services.AddTelegramLogger(Configuration);
            services.AddBinanceDatabase(Configuration);
            services.AddRedis(Configuration);
            services.AddRecurringJobScheduler();

            services.AddBinanceExchange(Configuration);
            services.LoadOptions<SignalSenderConfig>(Configuration);
            services.AddBinanceAnalyticService();

            services.AddSingleton<IService, Service>();

            services.AddRazorPages();
            services.ConfigureForInitialization<IService>(async _ => await _.RunAsync());

            // постгрес 6 имеет критически важные изменения пр обработке DateTime
            // https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
            // https://github.com/nhibernate/nhibernate-core/issues/2994
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseRouting();

            serviceProvider.ApplyBinanceDatabaseMigration();
            var task = Task.Run(async () => await AppInitializer.InitializeAsync(serviceProvider));
            Task.WaitAll(task);
        }
    }
}
