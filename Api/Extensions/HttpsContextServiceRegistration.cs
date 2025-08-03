
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using CleanArchitecture.Infrastructure.Interfaces;
using CleanArchitecture.Infrastructure.Services.RedisSessionManager;

namespace CleanArchitecture.Api.Extensions.HttpContextServiceRegistration
{
    public static class HttpsContextServiceRegistration
    {
        public static IServiceCollection AddRedisSessionManager(this IServiceCollection services, IConfiguration configuration)
        {
            // Redis bağlantısını IConnectionMultiplexer olarak singleton kaydetmek kritiktir.
            // Bu nesne pahalıdır ve uygulama boyunca tek bir tane olması gerekir.
            var redisConnectionString = configuration.GetConnectionString("Redis");
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

            // Artık ISessionManager istendiğinde Redis implementasyonunu veriyoruz.
            // IHttpContextAccessor zaten diğer projede kayıtlı olduğu için tekrar eklemeye gerek yok.
            // Eğer diğer projeyi silecekseniz buraya AddHttpContextAccessor eklenmelidir.
            services.AddScoped<ISessionManager, RedisSessionManager>();

            return services;
        }
    }
}
