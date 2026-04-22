using Microsoft.AspNetCore.Mvc;

namespace PlantShop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
