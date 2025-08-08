using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Config
{
    public class ConfigManager : IConfigManager
    {
        private readonly IConfiguration _configuration;

        public ConfigManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Get(string key)
        {
            return _configuration[key] ?? string.Empty;
        }

        public T Get<T>(string key)
        {
            return _configuration.GetValue<T>(key);
        }

        public string? GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }
    }
}
