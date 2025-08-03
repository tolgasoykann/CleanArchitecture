using Microsoft.Extensions.Configuration;

namespace Infrastructure.Interfaces
{
    public interface IConfigManager
    {
        string Get(string key);
        T Get<T>(string key);
    }
}
