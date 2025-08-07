using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDatabaseController : ControllerBase
    {
        private readonly IDatabaseManager _databaseManager;

        public TestDatabaseController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var query = "SELECT Id, FirstName, Email FROM Users"; // Dummy tablo
            var result = await _databaseManager.QueryAsync<UserDto>(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            var query = "INSERT INTO Users (FirstName, Email) VALUES (@FirstName, @Email)";
            var result = await _databaseManager.ExecuteAsync(query, user);
            return Ok(new { affectedRows = result });
        }
    }

    public class UserDto
    {
        public int? Id { get; set; }  // Insert için gerekmez
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
