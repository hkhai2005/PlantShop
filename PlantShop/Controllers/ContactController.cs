using Microsoft.AspNetCore.Mvc;

namespace PlantShop.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
