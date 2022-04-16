using Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Redis
{
    /// <summary>
    /// Расширение для подключения Redis'а
    /// </summary>
    public static class ServiceCollectionExtension
    {
        private static readonly ILoggerDecorator Log = LoggerManager.CreateDefaultLogger();

        /// <summary>
        /// Добавление <see cref="IRedisDatabase"/>
        /// </summary>
        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("REDIS_CONNECTION_STRING");
            Log.TraceAsync($"REDIS_CONNECTION_STRING='{connectionString}'");
            services.AddSingleton<IRedisDatabase, RedisDatabase>();
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        }
    }
}
