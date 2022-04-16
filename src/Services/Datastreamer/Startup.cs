using AutoMapper;
using BinanceDatabase;
using BinanceDataService;
using BinanceExchange;
using Common.Initialization;
using DataServiceLibrary;
using Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis;
using Scheduler;
using System;
using System.Threading.Tasks;

namespace Datastreamer
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
                });

            services.AddTelegramLogger(Configuration);
            services.AddSingleton(mapperConfig.CreateMapper());
            services.AddBinanceDatabase(Configuration);
            services.AddRecurringJobScheduler();
            services.AddRedis(Configuration);
            services.AddBinanceExchange(Configuration);
            services.AddDataServiceFactory();

            services.AddRazorPages();
            services.ConfigureForInitialization<IDataService>(async dataService =>
            {
                await dataService.StartAsync();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseRouting();

            serviceProvider.ApplyDatabaseMigration();
            var task = Task.Run(async () => await AppInitializer.InitializeAsync(serviceProvider));
            Task.WaitAll(task);
        }
    }
}
