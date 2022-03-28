using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis;
using Scheduler;
using SignalsSender.Configuration;
using System;
using System.Threading.Tasks;

namespace SignalsSender
{
    public class Startup
    {
        private readonly SignalSenderConfig _settings = new();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.GetSection(SignalSenderConfig.Name).Bind(_settings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IService, Service>((sp) =>
            {
                var redisDatabase = sp.GetRequiredService<IRedisDatabase>();
                var scheduler = sp.GetRequiredService<IRecurringJobScheduler>();
                return new Service(_settings, redisDatabase, scheduler);
            });
            services.AddRecurringJobScheduler();
            services.AddRedis(Configuration);
            services.AddRazorPages();
            services.ConfigureForInitialization<IService>( async _=> await _.RunAsync());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            // app.UseMiddleware<InitializationMiddleware>();
            app.UseRouting();

            var task = Task.Run(async () => await AppInitializer.InitializeAsync(serviceProvider));
            Task.WaitAll(task);
        }
    }
}
