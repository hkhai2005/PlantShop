using Microsoft.AspNetCore.Mvc;

namespace PlantShop.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
