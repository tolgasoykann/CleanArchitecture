using Microsoft.AspNetCore.Http;
using System.Text.Json;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;

namespace CleanArchitecture.Infrastructure.Services.HttpContextSessionManager;

public class HttpContextSessionManager : ISessionManager // Interface'i doğru implemente ediyor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    // Session nesnesine kolay erişim için yardımcı özellik
    private ISession? Session => _httpContextAccessor.HttpContext?.Session;

    // CONSTRUCTOR DÜZELTİLDİ: Sadece IHttpContextAccessor alıyor
    public HttpContextSessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string SessionId => Session?.Id ?? string.Empty;

    public T? Get<T>(string key)
    {
        // METOT DÜZELTİLDİ: GetString(key) çağrılıyor
        var sessionData = Session?.GetString(key);

        if (string.IsNullOrEmpty(sessionData))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(sessionData);
    }

    public void Set<T>(string key, T value)
    {
        var jsonString = JsonSerializer.Serialize(value);

        // METOT DÜZELTİLDİ: Gerçek session nesnesinin SetString metodu çağrılıyor
        Session?.SetString(key, jsonString);
    }

    public void Remove(string key) => Session?.Remove(key);

    public void Clear() => Session?.Clear();
}