using Microsoft.AspNetCore.Mvc;
using PlantShop.Models;

namespace PlantShop.ViewComponents
{
    public class MenuTopViewComponent : ViewComponent
    {
        private readonly PlantShopDbContext _context;
        public MenuTopViewComponent(PlantShopDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = _context.TbMenus.Where(m => (bool)m.IsActive).
                OrderBy(m => m.Position).ToList();
            return await Task.FromResult<IViewComponentResult>(View(items));
        }
    } 
}
