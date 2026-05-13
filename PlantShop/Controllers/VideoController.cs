using PlantShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PlantShop.Controllers
{
    public class VideoController : Controller
    {
        private readonly PlantShopDbContext _context;

        public VideoController(PlantShopDbContext context)
        {
            _context = context;
        }

        // GET: /Video
        public async Task<IActionResult> Index(string category, string search)
        {
            var query = _context.Videos
                .Where(v => v.IsActive == true)
                .OrderByDescending(v => v.SortOrder)
                .ThenByDescending(v => v.CreatedDate)
                .AsQueryable();

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(v => v.Category == category);
            }

            // Tìm kiếm theo tiêu đề
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.Title.Contains(search));
            }

            var videos = await query.ToListAsync();

            // Lấy danh sách danh mục cho bộ lọc
            ViewBag.Categories = await _context.Videos
                .Where(v => v.IsActive == true && v.Category != null)
                .Select(v => v.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.CurrentCategory = category;
            ViewBag.SearchKeyword = search;

            return View(videos);
        }

        // GET: /Video/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var video = await _context.Videos
                .FirstOrDefaultAsync(v => v.VideoId == id && v.IsActive == true);

            if (video == null)
            {
                return NotFound();
            }

            // Tăng lượt xem
            video.Views = (video.Views ?? 0) + 1;
            await _context.SaveChangesAsync();

            // Lấy video liên quan (cùng danh mục)
            var relatedVideos = await _context.Videos
                .Where(v => v.IsActive == true && v.VideoId != id)
                .Where(v => v.Category == video.Category)
                .OrderByDescending(v => v.Views)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedVideos = relatedVideos;

            return View(video);
        }
    }
}