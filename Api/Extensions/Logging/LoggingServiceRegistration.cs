using Domain.Interfaces;
using Infrastructure.Services.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public static class LoggingServiceRegistration
{
    public static IServiceCollection AddLoggingManager(this IServiceCollection services, string loggerType)
    {
      
        services.AddHttpContextAccessor();

        if (loggerType == "console")
        {
            services.AddSingleton<ILogManager, ConsoleLogManager>();
        }
        else if (loggerType == "file")
        {
            services.AddSingleton<ILogManager, FileLogManager>();
        }
        else if (loggerType == "composite")
        {
            // Önce bağımsız olarak ekle
            services.AddSingleton<ConsoleLogManager>();
            services.AddSingleton<FileLogManager>();

            // Sonra composite yap
            services.AddSingleton<ILogManager>(sp =>
            {
                var loggers = new List<ILogManager>
                {
                    sp.GetRequiredService<ConsoleLogManager>(),
                    sp.GetRequiredService<FileLogManager>()
                };
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();


                return new CompositeLogManager(loggers,httpContextAccessor);
            });
        }

        return services;
    }
}
