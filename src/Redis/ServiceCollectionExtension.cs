using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using StackExchange.Redis;

namespace Redis
{
    /// <summary>
    /// Расширение для подключения Redis'а
    /// </summary>
    public static class ServiceCollectionExtension
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Добавление <see cref="IRedisDatabase"/>
        /// </summary>
        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("REDIS_CONNECTION_STRING");
            Log.Trace($"REDIS_CONNECTION_STRING='{connectionString}'");
            services.AddSingleton<IRedisDatabase, RedisDatabase>();
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        }
    }
}
