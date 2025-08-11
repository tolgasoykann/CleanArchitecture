using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Configuration;
using System.Data.SqlClient;

namespace Infrastructure.Services.Database
{
    public class DatabaseManager : IDatabaseManager , IHealthCheckable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureToggleService _toggleService;
        private readonly ILogManager _logManager;
        private readonly string _connectionString;


        public DatabaseManager(IServiceProvider serviceProvider, IFeatureToggleService toggleService, ILogManager logManager, IConfigManager configManager)
        {
            _serviceProvider = serviceProvider;
            _toggleService = toggleService;
            _logManager = logManager;
            _connectionString = configManager.GetConnectionString(_toggleService.GetDatabaseProvider());

        }

        private IDatabaseManager GetExecutor()
            {
                var provider = _toggleService.GetDatabaseProvider();
                _logManager.Info($"Database provider seçildi: {provider}");
                return provider switch
                {
                    "MongoDB" => _serviceProvider.GetRequiredService<Infrastructure.Services.Database.MongoDbExecutor>(),
                    "Redis" => _serviceProvider.GetRequiredService<Infrastructure.Services.Database.RedisExecutor>(),
                    "Dapper" => _serviceProvider.GetRequiredService<Infrastructure.Services.Database.DapperExecutor>(),
                    _ => _serviceProvider.GetRequiredService<Infrastructure.Services.Database.SqlClientExecutor>()
                };
            }

        public Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
        {
                    _logManager.Info($"Query çalıştırılıyor: {query}");
                   return GetExecutor().QueryAsync<T>(query, parameters);
        }
        public Task<int> ExecuteAsync(string query, object? parameters = null)
        {
                    _logManager.Info($"Execute çalıştırılıyor: {query}");
                    return GetExecutor().ExecuteAsync(query, parameters);
        }
        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                _logManager.Info($"[] DatabaseManager health check passed.");
                return true;
            }
            catch (Exception ex)
            {
                _logManager.Error($"[] DatabaseManager health check failed.", ex);
                return false;
            }
        }
    }
}


