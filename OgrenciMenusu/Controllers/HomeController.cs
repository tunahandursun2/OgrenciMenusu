using Microsoft.AspNetCore.Mvc;

namespace OgrenciMenusu.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
