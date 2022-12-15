using Microsoft.AspNetCore.Mvc;

namespace MangaMu.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
