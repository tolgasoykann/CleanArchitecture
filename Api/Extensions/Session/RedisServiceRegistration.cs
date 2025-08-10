using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using CleanArchitecture.Domain.Interfaces;
using Infrastructure.Services.Session;
using Domain.Interfaces;

namespace Api.Extensions.Session
{
    public static class RedisServiceRegistration
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
            // RedisSessionManager'ı IHealthCheckable olarak da kaydedin.
            // Bu, HealthCheckStartup sınıfının onu bulmasını sağlar.
            // Scoped olarak kaydedilen bir servisi, yine Scoped olarak kaydetmelisiniz.
            services.AddScoped<IHealthCheckable, RedisSessionManager>();

            return services;
            return services;
        }
    }
}
