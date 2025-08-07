using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Services.Database
{
    public class MongoDatabaseManager : IDatabaseManager
    {
        private readonly IMongoDatabase _database;

        public MongoDatabaseManager(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("CleanArchitecture");
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            return await collection.Find(FilterDefinition<T>.Empty).ToListAsync();
        }

        public async Task<int> ExecuteAsync(string query, object? parameters = null)
        {
            // Not implemented
            return await Task.FromResult(0);
        }
    }
}
