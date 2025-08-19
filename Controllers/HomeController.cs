using Microsoft.AspNetCore.Mvc;

namespace TaskManagementPortal.Controllers
{
    public class HomeController : Controller
    {
        // GET: /
        public IActionResult Index()
        {
            // This message will be shown on the home page
            ViewBag.TitleMessage = "Professional Task Management Portal";
            ViewBag.Subtitle = "A demonstration of ASP.NET Core, .NET MAUI integration, and modern web development practices.";
            return View();
        }
    }
}