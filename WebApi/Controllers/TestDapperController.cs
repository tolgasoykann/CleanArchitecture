using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDapperController : ControllerBase
    {
        private readonly IDatabaseManager _databaseManager;

        public TestDapperController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _databaseManager.QueryAsync<UserDto>(
                "EXEC dbo.TestGetUsers", null);

            return Ok(users);
        }
    }

    // Response için basit DTO
    public class DapperUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
