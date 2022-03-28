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
            services.AddRecurringJobScheduler();
            services.AddRedis(Configuration);
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseMiddleware<InitializationMiddleware>();

            app.UseRouting();
            var redisDatabase = serviceProvider.GetRequiredService<IRedisDatabase>();
            var scheduler = serviceProvider.GetRequiredService<IRecurringJobScheduler>();
            var task = Task.Run(async () => await new Service(_settings, redisDatabase, scheduler).RunAsync());

            Task.WaitAll(task);
        }
    }
}
