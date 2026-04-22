using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;

namespace PlantShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly PlantShopDbContext _context;
        public ShopController(PlantShopDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.TbProducts.ToList();
            return View(products);
            

        }


        

        public IActionResult Details()
        {
            return View();
        }
        [Route("/product/{alias}-{id}.html")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbProductCategories == null)
            {
                return NotFound();
            }

            var product = await _context.TbProductCategories.FirstOrDefaultAsync(m => m.CategoryProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.produicts = _context.TbProductCategories
                .Where(i => i.CategoryProductId == id)
                .ToList();

            return View(product);
        }
    }
}
