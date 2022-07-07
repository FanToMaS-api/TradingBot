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
using System.Threading;
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
            services.AddMappingProfiles(
                new BinanceMapperProfile(),
                new BinanceDatabaseMappingProfile());

            services.AddTelegramLogger(Configuration);
            services.AddBinanceDatabase(Configuration);
            services.AddRecurringJobScheduler();
            services.AddRedis(Configuration);
            services.AddBinanceExchange(Configuration);
            services.AddDataServiceFactory(Configuration);

            services.AddRazorPages();
            services.ConfigureForInitialization<IDataService>(async dataService =>
            {
                await Task.Factory.StartNew(
                   async () => await dataService.StartAsync(),
                   CancellationToken.None,
                   TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                   TaskScheduler.Default);
            });

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
