using Domain.Interfaces;
using Infrastructure.Services.Proxy;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions.Proxy
{
    public static class ProxyServiceRegistration
    {
        public static IServiceCollection AddProxyManager(this IServiceCollection services)
        {
            services.AddHttpClient<IProxyManager, ProxyManager>();
            services.AddSingleton<IHealthCheckable>(sp => sp.GetRequiredService<IProxyManager>() as ProxyManager);
            return services;
        }
    }

}
