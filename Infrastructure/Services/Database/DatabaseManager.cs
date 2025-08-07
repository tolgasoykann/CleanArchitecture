using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureToggleService _toggleService;

        public DatabaseManager(IServiceProvider serviceProvider, IFeatureToggleService toggleService)
        {
            _serviceProvider = serviceProvider;
            _toggleService = toggleService;
        }

        private IDatabaseManager GetExecutor()
        {
            var provider = _toggleService.GetDatabaseProvider();

            return provider switch
            {
                "MongoDB" => _serviceProvider.GetRequiredService<Infrastructure.Services.Database.MongoDbExecutor>(),
                "Redis" => _serviceProvider.GetRequiredService<Infrastructure.Services.Database.RedisExecutor>(),
                "Dapper" => _serviceProvider.GetRequiredService<Infrastructure.Services.Database.DapperExecutor>(),
                _ => _serviceProvider.GetRequiredService<Infrastructure.Services.Database.SqlClientExecutor>()
            };
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
            => GetExecutor().QueryAsync<T>(query, parameters);

        public Task<int> ExecuteAsync(string query, object? parameters = null)
            => GetExecutor().ExecuteAsync(query, parameters);
    }
}


