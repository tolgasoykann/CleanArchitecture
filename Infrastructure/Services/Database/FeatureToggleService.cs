using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

public class FeatureToggleService : IFeatureToggleService
{
    private readonly IConfiguration _configuration;

    public FeatureToggleService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetDatabaseProvider()
    {
        return _configuration["DatabaseProvider"] ?? "SqlClient";
    }

    public string GetConnectionString(string provider)
    {
        return _configuration.GetConnectionString(provider) ?? string.Empty;
    }

    public string GetMongoDatabase() =>
        _configuration["MongoDbSettings:Database"] ?? "CleanArchitectureDb";

    public string GetMongoCollection() =>
        _configuration["MongoDbSettings:UserCollection"] ?? "Users";

    public string GetRedisUserKeyPrefix() =>
        _configuration["RedisSettings:UserKeyPrefix"] ?? "user:";
}
