using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace ICE.GDocs.Api.Configurations
{
    public static class RedisConfig
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisCfg = configuration.GetSection("Redis").Get<RedisConfiguration>();

            services
                .AddSingleton(_ => redisCfg)
                .AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisCfg);

            return services;
        }
    }
}
