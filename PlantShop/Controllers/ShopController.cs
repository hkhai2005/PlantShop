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
        public ActionResult Index(
    string keyword,
    int? sort,
    int? price)
        {
            var products = _context.TbProducts.AsQueryable();

            // SEARCH
            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(p =>
                    p.Title.Contains(keyword));
            }

            // PRICE FILTER
            switch (price)
            {
                case 1:
                    products = products.Where(p =>
                        p.Price < 100000);
                    break;

                case 2:
                    products = products.Where(p =>
                        p.Price >= 100000 &&
                        p.Price <= 300000);
                    break;

                case 3:
                    products = products.Where(p =>
                        p.Price > 300000);
                    break;
            }

            // SORT
            switch (sort)
            {
                case 1:
                    products = products.OrderByDescending(p => p.ProductId);
                    break;

                case 2:
                    products = products.OrderBy(p => p.Price);
                    break;

                case 3:
                    products = products.OrderByDescending(p => p.Price);
                    break;
            }

            ViewBag.Keyword = keyword;
            ViewBag.Sort = sort;
            ViewBag.Price = price;

            return View(products.ToList());
        }


        public IActionResult Details()
        {
            return View();
        }
        [Route("/product/{alias}-{id}.html")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbProducts == null)
            {
                return NotFound();
            }

            var product = await _context.TbProducts.FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.produicts = _context.TbProducts
                .Where(i => i.ProductId == id)
                .ToList();

            return View(product);
        }
    }
}
