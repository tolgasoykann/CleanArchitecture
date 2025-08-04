using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using Infrastructure.Services.Proxy;


namespace WebApi.Controllers
{

namespace WebApi.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class TestProxyController : ControllerBase
        {
            private readonly IProxyManager _proxyManager;

            public TestProxyController(IProxyManager proxyManager)
            {
                _proxyManager = proxyManager;
            }

            [HttpGet("post/{id}")]
            public async Task<IActionResult> GetPost(int id)
            {
                string url = $"https://jsonplaceholder.typicode.com/posts/{id}";
                var result = await _proxyManager.GetAsync<Post>(url);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
        }

        // Basit model örneği (aynı namespace'de ya da ayrı bir yerde)
        public class Post
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }
    }

}
