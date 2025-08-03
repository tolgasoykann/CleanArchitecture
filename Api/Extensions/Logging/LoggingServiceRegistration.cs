using Infrastructure.Interfaces;
using Infrastructure.Services.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions.Logging
{
    public static class LoggingServiceRegistration
    {
        public static IServiceCollection AddLoggingManager(this IServiceCollection services, string loggerType = "file")
        {
            if (loggerType == "console")
            {
                services.AddSingleton<ILogManager, ConsoleLogManager>();
            }
            else
            {
                services.AddSingleton<ILogManager, FileLogManager>();
            }

            return services;
        }
    }
}
