using CleanArchitecture.Domain.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Linq;

namespace Infrastructure.Services.Session
{
    public class RedisSessionManager : ISessionManager, IHealthCheckable
    {
        private readonly IDatabase _redisDatabase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TimeSpan _sessionTimeout;
        private readonly ICustomJsonSerializer _jsonSerializer;
        private readonly ILogManager _logManager;
        private readonly ISessionContextAccessor _sessionContextAccessor;
        private readonly IConnectionMultiplexer _redisConnection;

        public RedisSessionManager(
            IConnectionMultiplexer redisConnection,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ICustomJsonSerializer customJsonSerializer,
            ILogManager logManager,
            ISessionContextAccessor sessionContextAccessor)
        {
            _redisConnection = redisConnection;
            _redisDatabase = redisConnection.GetDatabase();
            _httpContextAccessor = httpContextAccessor;
            _sessionTimeout = TimeSpan.FromMinutes(configuration.GetValue("SessionSettings:TimeoutMinutes", 20));
            _jsonSerializer = customJsonSerializer;
            _logManager = logManager;
            _sessionContextAccessor = sessionContextAccessor;
        }

        public T? Get<T>(string key)
        {
            var redisKey = GetRedisKey(key);
            _logManager.Info($"[TraceId: {_sessionContextAccessor.TraceId}] RedisSessionManager -> Get called with key: {redisKey}");

            var redisValue = _redisDatabase.StringGet(redisKey);

            if (redisValue.IsNullOrEmpty)
            {
                _logManager.Warning($"[TraceId: {_sessionContextAccessor.TraceId}] Key not found in Redis: {redisKey}");
                return default;
            }

            var result = _jsonSerializer.Deserialize<T>(redisValue.ToString());
            _logManager.Info($"[TraceId: {_sessionContextAccessor.TraceId}] Deserialized value for key {redisKey}");

            return result;
        }

        public void Set<T>(string key, T value)
        {
            var redisKey = GetRedisKey(key);
            var jsonValue = _jsonSerializer.Serialize(value);

            _redisDatabase.StringSet(redisKey, jsonValue, _sessionTimeout);
            _logManager.Info($"[TraceId: {_sessionContextAccessor.TraceId}] RedisSessionManager -> Set key: {redisKey}, timeout: {_sessionTimeout.TotalMinutes} minutes");
        }

        public void Remove(string key)
        {
            var redisKey = GetRedisKey(key);
            _redisDatabase.KeyDelete(redisKey);
            _logManager.Info($"[TraceId: {_sessionContextAccessor.TraceId}] RedisSessionManager -> Removed key: {redisKey}");
        }

        public void Clear()
        {
            var pattern = $"session:{SessionId}:*";
            var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
            var sessionKeys = server.Keys(_redisDatabase.Database, pattern: pattern).ToArray();

            if (sessionKeys.Length > 0)
            {
                _redisDatabase.KeyDelete(sessionKeys);
                _logManager.Info($"[TraceId: {_sessionContextAccessor.TraceId}] RedisSessionManager -> Cleared {sessionKeys.Length} keys for session: {SessionId}");
            }
            else
            {
                _logManager.Info($"[TraceId: {_sessionContextAccessor.TraceId}] RedisSessionManager -> No keys to clear for session: {SessionId}");
            }
        }

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

                    _logManager.Info($"[TraceId: {_sessionContextAccessor.TraceId}] RedisSessionManager -> New sessionId generated: {sessionId}");
                }

                return sessionId;
            }

        }
        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                // Redis'e bir ping atarak bağlantının durumunu kontrol ediyoruz.
                await _redisConnection.GetDatabase().PingAsync();

                // ConnectionMultiplexer'ın bağlantı durumunu da kontrol edebiliriz.
                bool isConnected = _redisConnection.IsConnected;
                if (!isConnected)
                {
                    _logManager.Error("Redis connection is not connected.");
                    return false;
                }

                _logManager.Info("Redis health check: OK");
                return true;
            }
            catch (Exception ex)
            {
                _logManager.Error("RedisSessionManager health check failed", ex);
                return false;
            }
        }

        private string GetRedisKey(string key) => $"session:{SessionId}:{key}";
    }
}
