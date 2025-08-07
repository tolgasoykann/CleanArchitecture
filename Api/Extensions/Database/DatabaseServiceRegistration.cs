using Domain.Interfaces;
using Infrastructure.Services.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DatabaseRegistration
{
    public static IServiceCollection AddDatabaseManager(this IServiceCollection services, string type)
    {
        switch (type.ToLower())
        {
            case "sql":
            case "sqlclient":
                services.AddScoped<IDatabaseManager, SqlDatabaseManager>();
                break;
            case "dapper":
                services.AddScoped<IDatabaseManager, DapperDatabaseManager>();
                break;
            case "mongo":
                services.AddScoped<IDatabaseManager, MongoDatabaseManager>();
                break;
            case "redis":
                services.AddScoped<IDatabaseManager, RedisDatabaseManager>();
                break;
            default:
                throw new InvalidOperationException($"Unsupported database type: {type}");
        }

        return services;
    }

}
