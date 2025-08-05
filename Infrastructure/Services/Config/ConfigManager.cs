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
            return _configuration[key];
        }

        public T Get<T>(string key)
        {
            var value = _configuration[key];
            return string.IsNullOrEmpty(value) ? default : (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
