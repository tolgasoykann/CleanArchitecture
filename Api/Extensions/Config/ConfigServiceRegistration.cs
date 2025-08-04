using Domain.Interfaces;
using Infrastructure.Services.Config;
using Microsoft.Extensions.DependencyInjection;


namespace Api.Extensions.Config
{
    public static class ConfigServiceRegistration
    {
        public static IServiceCollection AddConfigManager(this IServiceCollection services)
        {
            services.AddSingleton<IConfigManager, ConfigManager>();
            return services;
        }
    }
}
