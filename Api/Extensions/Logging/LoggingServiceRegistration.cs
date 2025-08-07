using Domain.Interfaces;
using Infrastructure.Services.Logging;
using Microsoft.Extensions.DependencyInjection;

public static class LoggingServiceRegistration
{
    public static IServiceCollection AddLoggingManager(this IServiceCollection services, string loggerType)
    {
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
            // 1. Console ve File logger'ları ayrı kaydet
            services.AddSingleton<ConsoleLogManager>();
            services.AddSingleton<FileLogManager>();

            // 2. ILogManager olarak bu ikisini tek tek kaydetme!
            // 3. CompositeLogManager içerisine açıkça gönder

            services.AddSingleton<ILogManager>(sp =>
            {
                var loggers = new List<ILogManager>
                {
                    sp.GetRequiredService<ConsoleLogManager>(),
                    sp.GetRequiredService<FileLogManager>()
                };

                return new CompositeLogManager(loggers);
            });
        }

        return services;
    }
}
