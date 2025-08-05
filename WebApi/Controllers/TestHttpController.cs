using CleanArchitecture.Domain.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestHttpController : ControllerBase
    {
        private readonly ISessionManager _sessionManager;

        public TestHttpController(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpPost("set")]
        public IActionResult SetSession([FromQuery] string key, [FromQuery] string value)
        {
            _sessionManager.Set(key, value);
            return Ok($"Session value set: {key} = {value}");
        }

        [HttpGet("get")]
        public IActionResult GetSession([FromQuery] string key)
        {
            var value = _sessionManager.Get<string>(key);
            if (value == null)
                return NotFound($"No session value found for key: {key}");

            return Ok($"Session value: {key} = {value}");
        }

        [HttpDelete("remove")]
        public IActionResult RemoveSession([FromQuery] string key)
        {
            _sessionManager.Remove(key);
            return Ok($"Session key removed: {key}");
        }

        [HttpDelete("clear")]
        public IActionResult ClearSession()
        {
            _sessionManager.Clear();
            return Ok("All session values cleared.");
        }
    }
}
