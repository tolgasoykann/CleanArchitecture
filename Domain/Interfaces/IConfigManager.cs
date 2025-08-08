
namespace Domain.Interfaces
{
    public interface IConfigManager
    {
        string Get(string key);
        T Get<T>(string key);
        string? GetConnectionString(string v);
    }
}
