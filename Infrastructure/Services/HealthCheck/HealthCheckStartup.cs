
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.HealthCheck
{
    public static class HealthCheckStartup
    {
        public static async Task CheckAllManagersHealthAsync(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogManager>();
            var healthCheckables = scope.ServiceProvider
                .GetServices<IHealthCheckable>()
                .ToList();

            foreach (var manager in healthCheckables)
            {
                var result = await manager.CheckHealthAsync();
                logger.Info($"{manager.GetType().Name} health check: {(result ? "OK" : "FAIL")}");
            }
        }
    }

}
