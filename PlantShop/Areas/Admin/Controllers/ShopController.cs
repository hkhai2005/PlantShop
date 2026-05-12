using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;

namespace PlantShop.Areas.Admin.Controllers
{

        [Area("Admin")]
        public class ShopController : Controller
        {
            private readonly PlantShopDbContext _context;

            public ShopController(PlantShopDbContext context)
            {
                _context = context;
            }

            // GET: Admin/Shop
            public async Task<IActionResult> Index()
            {
                var carRentalContext = _context.TbProducts.Include(t => t.CategoryProduct);
                return View(await carRentalContext.ToListAsync());
            }

            // GET: Admin/Cars/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var tbCar = await _context.TbProducts
                    .Include(t => t.CategoryProduct)
                    .FirstOrDefaultAsync(m => m.ProductId== id);
                if (tbCar == null)
                {
                    return NotFound();
                }

                return View(tbCar);
            }

            // GET: Admin/Cars/Create
            public IActionResult Create()
            {
                ViewData["CategoryProductId"] = new SelectList(_context.TbProductCategories, "CategoryProductId", "Title");
                return View();
        }

            // POST: Admin/Cars/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("ProductId,Title,Alias,CategoryProductId,Description,Detail,Image,Price,PriceSale,Quantity,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,IsNew,IsBestSeller,IsFeatured,UnitInStock,IsActive,Star,SKU,Weight,Dimensions,Views,SoldCount,Tags")] TbProduct tbCar)
            {
                if (ModelState.IsValid)
                {
                    tbCar.Alias = PlantShop.Utilities.Function.TitleSlugGenerationAlias(tbCar.Title);
                    _context.Add(tbCar);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                //ViewData["CarCategoryId"] = new SelectList(_context.TbCarCategories, "CarCategoryId", "Title", tbCar.CarCategoryId);
                return View(tbCar);
            }

            // GET: Admin/Cars/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
            {
                return NotFound();
            }

            var tbCar = await _context.TbProducts.FindAsync(id);
            if (tbCar == null)
            {
                return NotFound();
            }
            ViewData["CategoryProductId"] = new SelectList(_context.TbProductCategories, "CategoryProductId", "Title", tbCar.CategoryProductId);
            return View(tbCar);
        }

            // POST: Admin/Cars/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Alias,CategoryProductId,Description,Detail,Image,Price,PriceSale,Quantity,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,IsNew,IsBestSeller,IsFeatured,UnitInStock,IsActive,Star,SKU,Weight,Dimensions,Views,SoldCount,Tags")] TbProduct tbCar)
            {
                if (id != tbCar.ProductId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(tbCar);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TbCarExists(tbCar.ProductId))
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
                //ViewData["CarCategoryId"] = new SelectList(_context.TbCarCategories, "CarCategoryId", "Title", tbCar.CarCategoryId);
                return View(tbCar);
            }

            // GET: Admin/Cars/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var tbCar = await _context.TbProducts
                    .Include(t => t.CategoryProduct)
                    .FirstOrDefaultAsync(m => m.ProductId == id);
                if (tbCar == null)
                {
                    return NotFound();
                }

                return View(tbCar);
            }

            // POST: Admin/Cars/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.TbProducts.FindAsync(id);

            if (product != null)
            {
                // 1. Xóa ảnh sản phẩm
                var images = _context.TbProductImages
                    .Where(x => x.ProductId == id);
                _context.TbProductImages.RemoveRange(images);

                // 2. Xóa thuộc tính sản phẩm
                var attributes = _context.TbProductAttributes
                    .Where(x => x.ProductId == id);
                _context.TbProductAttributes.RemoveRange(attributes);

                // 3. Xóa chi tiết đơn hàng
                var orderDetails = _context.TbOrderDetails
                    .Where(x => x.ProductId == id);
                _context.TbOrderDetails.RemoveRange(orderDetails);

                // 4. Xóa wishlist
                var wishlists = _context.TbWishlists
                    .Where(x => x.ProductId == id);
                _context.TbWishlists.RemoveRange(wishlists);

                // 5. Xóa đánh giá
                var reviews = _context.TbProductReviews
                    .Where(x => x.ProductId == id);
                _context.TbProductReviews.RemoveRange(reviews);

                // 6. Xóa giỏ hàng
                var carts = _context.TbCartDetails
                    .Where(x => x.ProductId == id);
                _context.TbCartDetails.RemoveRange(carts);

                // Cuối cùng xóa Product
                _context.TbProducts.Remove(product);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TbCarExists(int id)
            {
                return _context.TbProducts.Any(e => e.ProductId == id);
            }
        }
    
}
