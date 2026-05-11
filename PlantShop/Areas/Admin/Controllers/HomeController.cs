using PlantShop.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace PlantShop.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            if (!Function.IsLogin())
                return RedirectToAction("Index", "Login");
            return View();
        }
    }
}
