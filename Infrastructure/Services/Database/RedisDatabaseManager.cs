using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services.Database
{
    public class RedisDatabaseManager : IDatabaseManager
    {
        private readonly IDatabase _redisDb;
        private readonly ICustomJsonSerializer _customJsonSerializer;

        public RedisDatabaseManager(IConfiguration configuration, ICustomJsonSerializer customJsonSerializer)
        {
            var redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
            _redisDb = redis.GetDatabase();
            _customJsonSerializer = customJsonSerializer;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string key, object? parameters = null)
        {
            var value = await _redisDb.StringGetAsync(key);
            if (value.IsNullOrEmpty) return Enumerable.Empty<T>();
            return new[] { _customJsonSerializer.Deserialize<T>(value!)! };
        }

        public async Task<int> ExecuteAsync(string key, object? value = null)
        {
            var json = _customJsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(key, json);
            return 1;
        }
    }
}
