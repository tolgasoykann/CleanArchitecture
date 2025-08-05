using CleanArchitecture.Domain.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestRedisController : ControllerBase
    {
        private readonly ISessionManager _sessionManager;

        public TestRedisController(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpPost("set")]
        public IActionResult Set([FromQuery] string key, [FromQuery] string value)
        {
            _sessionManager.Set(key, value);
            return Ok("Set completed");
        }

        [HttpGet("get")]
        public IActionResult Get([FromQuery] string key)
        {
            var value = _sessionManager.Get<string>(key);
            return value != null ? Ok(value) : NotFound();
        }

        [HttpDelete("remove")]
        public IActionResult Remove([FromQuery] string key)
        {
            _sessionManager.Remove(key);
            return Ok("Removed");
        }

        [HttpDelete("clear")]
        public IActionResult Clear()
        {
            _sessionManager.Clear();
            return Ok("Redis cleared");
        }
    }
}
