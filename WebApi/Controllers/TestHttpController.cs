using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class TestHttpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
