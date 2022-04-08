using AutoMapper;
using BinanceDatabase;
using BinanceExchange;
using BinanceDataService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis;
using Scheduler;
using SignalsSender.Configuration;
using System;
using System.Threading.Tasks;
using Common.Initialization;
using Common.Extensions;
using Analytic;
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
            var mapperConfig = new MapperConfiguration(
            mc =>
            {
                mc.AddProfile(new BinanceDatabaseMappingProfile());
                mc.AddProfile(new BinanceMapperProfile());
            }
        );

            services.AddSingleton(mapperConfig.CreateMapper());
            services.AddBinanceDatabase(Configuration);
            services.AddRedis(Configuration);
            services.AddRecurringJobScheduler();

            services.AddBinanceExchange(Configuration);
            services.LoadOptions<SignalSenderConfig>(Configuration);
            services.AddDataServiceFactory();
            services.AddBinanceAnalyticService();
            services.AddTelegramClient(Configuration);

            services.AddSingleton<IService, Service>();

            services.AddRazorPages();
            services.ConfigureForInitialization<IService>(async _ => await _.RunAsync());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            // app.UseMiddleware<InitializationMiddleware>();
            app.UseRouting();

            serviceProvider.ApplyDatabaseMigration();
            var task = Task.Run(async () => await AppInitializer.InitializeAsync(serviceProvider));
            Task.WaitAll(task);
        }
    }
}
