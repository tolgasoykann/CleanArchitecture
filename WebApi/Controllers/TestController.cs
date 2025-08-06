using CleanArchitecture.Domain.Interfaces;
using Domain.Interfaces;
using Infrastructure.Services.Proxy;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IConfigManager _configManager;
        private readonly ILogManager _logManager;
        private readonly IProxyManager _proxyManager;
        private readonly ISessionManager _sessionManager;

        public TestController(
            IConfigManager configManager,
            ILogManager logManager,
            IProxyManager proxyManager,
            ISessionManager sessionManager)
        {
            _configManager = configManager;
            _logManager = logManager;
            _proxyManager = proxyManager;
            _sessionManager = sessionManager;
        }

        [HttpGet("config/{key}")]
        public IActionResult GetConfigValue(string key)
        {
            var value = _configManager.Get(key);
            return Ok(value);
        }

        [HttpPost("log")]
        public IActionResult WriteLog([FromBody] string message)
        {
            _logManager.Info(message);
            return Ok("Log written.");
        }

      
        [HttpPost("session/{key}")]
        public IActionResult SetSession(string key, [FromBody] string value)
        {
            _sessionManager.Set(key, value);  // Redis'e set
            return Ok($"Key {key} set with value '{value}'");
        }



    }
}
