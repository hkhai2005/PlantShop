using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;

namespace PlantShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VideoController : Controller
    {
        private readonly PlantShopDbContext _context;

        public VideoController(PlantShopDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Video
        public async Task<IActionResult> Index(string searchTerm, string category)
        {
            var query = _context.Videos.OrderByDescending(v => v.SortOrder).ThenByDescending(v => v.CreatedDate).AsQueryable();

            // Tìm kiếm theo tiêu đề
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(v => v.Title.Contains(searchTerm));
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(v => v.Category == category);
            }

            var videos = await query.ToListAsync();

            // Lấy danh sách danh mục cho bộ lọc
            ViewBag.Categories = await _context.Videos
                .Where(v => v.Category != null)
                .Select(v => v.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentCategory = category;

            return View(videos);
        }

        // GET: Admin/Video/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Videos
                .FirstOrDefaultAsync(m => m.VideoId == id);

            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        // GET: Admin/Video/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Video/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,YoutubeId,Thumbnail,Duration,Category,IsActive,SortOrder")] Video video)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra YoutubeId đã tồn tại chưa
                if (_context.Videos.Any(v => v.YoutubeId == video.YoutubeId))
                {
                    ModelState.AddModelError("YoutubeId", "ID YouTube đã tồn tại!");
                    return View(video);
                }

                video.CreatedDate = DateTime.Now;
                video.CreatedBy = User.Identity?.Name ?? "Admin";
                video.Views = 0;

                _context.Add(video);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm video thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(video);
        }

        // GET: Admin/Video/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            return View(video);
        }

        // POST: Admin/Video/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VideoId,Title,Description,YoutubeId,Thumbnail,Duration,Category,IsActive,SortOrder,Views,CreatedDate,CreatedBy")] Video video)
        {
            if (id != video.VideoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    video.ModifiedDate = DateTime.Now;
                    video.ModifiedBy = User.Identity?.Name ?? "Admin";

                    _context.Update(video);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật video thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoExists(video.VideoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(video);
        }

        // GET: Admin/Video/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Videos
                .FirstOrDefaultAsync(m => m.VideoId == id);
            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        // POST: Admin/Video/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                _context.Videos.Remove(video);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa video thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VideoExists(int id)
        {
            return _context.Videos.Any(e => e.VideoId == id);
        }
    }
}