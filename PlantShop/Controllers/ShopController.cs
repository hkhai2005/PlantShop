using Microsoft.AspNetCore.Mvc;

namespace PlantShop.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
