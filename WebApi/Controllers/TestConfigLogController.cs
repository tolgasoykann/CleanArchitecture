using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestConfigLogController : ControllerBase
    {
        private readonly IConfigManager _configManager;
        private readonly ILogManager _logManager;

        public TestConfigLogController(IConfigManager configManager, ILogManager logManager)
        {
            _configManager = configManager;
            _logManager = logManager;
        }

        [HttpGet("config/{key}")]
        public IActionResult GetConfig(string key)
        {
            var value = _configManager.Get(key);
            _logManager.Info($"Config key '{key}' fetched with value: {value}");

            if (value == null)
                return NotFound($"No config value found for key: {key}");

            return Ok(value);
        }

        [HttpPost("log")]
        public IActionResult WriteLog([FromQuery] string message, [FromQuery] string level = "info")
        {
            switch (level.ToLower())
            {
                case "info":
                    _logManager.Info(message);
                    break;
                case "warn":
                    _logManager.Warning(message);
                    break;
                case "error":
                    _logManager.Error(message);
                    break;
                default:
                    return BadRequest("Invalid log level. Use 'info', 'warn', or 'error'.");
            }

            return Ok($"Log written at level '{level}': {message}");
        }
    }
}
