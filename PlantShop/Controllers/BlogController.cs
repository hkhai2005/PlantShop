using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;

namespace PlantShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly PlantShopDbContext _context;
        public BlogController(PlantShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var blogs = _context.TbBlogs.ToList();
            return View(blogs);
        }

        public IActionResult Details()
        {
            return View();
        }
        [Route("/blog/{alias}-{id}.html")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbBlogs == null)
            {
                return NotFound();
            }

            var blog = await _context.TbBlogs.FirstOrDefaultAsync(m => m.BlogId == id);

            if (blog == null)
            {
                return NotFound();
            }
            ViewBag.blogComment = _context.TbBlogComments
                .Where(i => i.BlogId == id)
                .ToList();

            return View(blog);
        }
    }
}
