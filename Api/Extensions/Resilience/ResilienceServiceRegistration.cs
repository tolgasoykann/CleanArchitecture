using Domain.Interfaces;
using Infrastructure.Services.Resiliency;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions.Resilience
{
    public static class ResilienceServiceRegistration
    {
        public static IServiceCollection AddResilienceServiceRegistration(this IServiceCollection services)
        {
            // Resiliency servisleri (RateLimit + CircuitBreaker) için bağımlılık kaydı
            services.AddScoped<IResiliencyService, ResiliencyService>();

            return services;
        }
    }
}
