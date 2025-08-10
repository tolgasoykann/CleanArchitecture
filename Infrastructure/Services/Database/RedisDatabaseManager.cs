using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;


namespace Infrastructure.Services.Database
{
    public class RedisDatabaseManager : IDatabaseManager , IHealthCheckable
    {
        private readonly IDatabase _redisDb;
        private readonly ICustomJsonSerializer _customJsonSerializer;
        private readonly ILogManager _logManager;
        private readonly IConnectionMultiplexer _redis;


        public RedisDatabaseManager(IConfiguration configuration, ICustomJsonSerializer customJsonSerializer, ILogManager logManager, IConnectionMultiplexer connectionMultiplexer)
        {
            // DI ile gelen ConnectionMultiplexer'ı kullanıyoruz.
            _redis = connectionMultiplexer;
            _redisDb = _redis.GetDatabase();

            // Diğer bağımlılıklar
            _customJsonSerializer = customJsonSerializer;
            _logManager = logManager;
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

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                var db = _redis.GetDatabase();
                var pong = await db.PingAsync();
                _logManager.Info($"Redis ping: {pong.TotalMilliseconds}ms");
                return true;
            }
            catch (Exception ex)
            {
                _logManager.Error("RedisSessionManager health check failed", ex);
                return false;
            }
        }

    }
}
