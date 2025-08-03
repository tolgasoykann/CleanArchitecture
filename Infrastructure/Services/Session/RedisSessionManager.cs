
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration; // <-- GetValue için gerekli
using StackExchange.Redis;
using System; // <-- TimeSpan için gerekli
using System.Text.Json;
using System.Linq;
using CleanArchitecture.Infrastructure.Interfaces;

namespace Infrastructure.Services.Session
{
    public class RedisSessionManager : ISessionManager
    {
        private readonly IConnectionMultiplexer _redisConnection; // <-- Clear() metodu için bunu kullanacağız
        private readonly IDatabase _redisDatabase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TimeSpan _sessionTimeout;

        public RedisSessionManager(IConnectionMultiplexer redisConnection, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _redisConnection = redisConnection;
            _redisDatabase = redisConnection.GetDatabase();
            _httpContextAccessor = httpContextAccessor;
            // appsettings.json'dan oturum zaman aşımı süresini oku, bulamazsan varsayılan 20 dakika kullan.
            _sessionTimeout = TimeSpan.FromMinutes(configuration.GetValue("SessionSettings:TimeoutMinutes", 20));
        }

        // SessionId'yi tarayıcıdan gelen cookie'den alıyoruz.
        // Eğer cookie yoksa, yeni bir tane oluşturup yanıta ekliyoruz.
        public string SessionId
        {
            get
            {
                var cookieName = "RedisSessionId";
                var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[cookieName];
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                    _httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieName, sessionId, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.Add(_sessionTimeout)
                    });
                }
                return sessionId;
            }
        }

        private string GetRedisKey(string key) => $"session:{SessionId}:{key}";

        public T? Get<T>(string key)
        {
            var redisKey = GetRedisKey(key);
            var redisValue = _redisDatabase.StringGet(redisKey);

            if (redisValue.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(redisValue.ToString());
        }

        public void Set<T>(string key, T value)
        {
            var redisKey = GetRedisKey(key);
            var jsonValue = JsonSerializer.Serialize(value);
            _redisDatabase.StringSet(redisKey, jsonValue, _sessionTimeout);
        }

        public void Remove(string key)
        {
            var redisKey = GetRedisKey(key);
            _redisDatabase.KeyDelete(redisKey);
        }

        public void Clear()
        {
            // Bağlı olduğumuz Redis sunucusunu alıyoruz.
            var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());

            // Mevcut oturum kimliğine ait tüm anahtarları bulmak için bir desen (pattern) oluşturuyoruz.
            // Örn: "session:abc-123-def-456:*"
            var pattern = $"session:{SessionId}:*";

            // Sunucudaki anahtarları, sunucuyu kilitlemeden (SCAN komutu ile) tarıyoruz
            // ve desenimize uyanları bir diziye atıyoruz.
            var sessionKeys = server.Keys(database: _redisDatabase.Database, pattern: pattern).ToArray();

            // Eğer silinecek anahtar bulunduysa, hepsini tek seferde siliyoruz.
            if (sessionKeys.Length > 0)
            {
                _redisDatabase.KeyDelete(sessionKeys);
            }

        }
    }
}
