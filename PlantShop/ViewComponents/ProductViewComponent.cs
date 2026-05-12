using PlantShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PlantShop.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly PlantShopDbContext _context;
        public ProductViewComponent(PlantShopDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _context.TbProducts
              .Include(c => c.CategoryProduct)
              .Where(c => c.IsActive == true && c.IsNew == true)
              .OrderByDescending(c => c.ProductId)
              .ToListAsync();
            return View(items);
        }
    }
}
