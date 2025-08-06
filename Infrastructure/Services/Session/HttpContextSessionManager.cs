using Microsoft.AspNetCore.Http;
using Infrastructure.Services.JsonSerializer;
    
using CleanArchitecture.Domain.Interfaces;
using Domain.Interfaces;


namespace Infrastructure.Services.Session;

public class HttpContextSessionManager : ISessionManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICustomJsonSerializer _jsonSerializer;

    // Session nesnesine kolay erişim için yardımcı özellik
    private ISession? Session => _httpContextAccessor.HttpContext?.Session;


    public HttpContextSessionManager(IHttpContextAccessor httpContextAccessor, ICustomJsonSerializer customJsonSerializer)
    {
        _httpContextAccessor = httpContextAccessor;
        _jsonSerializer = customJsonSerializer;
    }

    public string SessionId => _httpContextAccessor.HttpContext?.Session?.Id ?? string.Empty;



    public T? Get<T>(string key) 
    {
        var sessionData = Session?.GetString(key);

        if (string.IsNullOrEmpty(sessionData))
            return default;

        return _jsonSerializer.Deserialize<T>(sessionData);
    }

    public void Set<T>(string key, T value)
    {
        var jsonString = _jsonSerializer.Serialize(value);
        Session?.SetString(key, jsonString);
    }


    public void Remove(string key) => Session?.Remove(key);

    public void Clear() => Session?.Clear();
}