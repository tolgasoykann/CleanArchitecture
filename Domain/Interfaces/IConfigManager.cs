
namespace Domain.Interfaces
{
    public interface IConfigManager
    {
        string Get(string key);
        T Get<T>(string key);
    }
}
