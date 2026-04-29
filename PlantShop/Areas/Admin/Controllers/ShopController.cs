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
                ViewData["CarCategoryId"] = new SelectList(_context.TbProductCategories, "CarCategoryId", "Title");
                return View();
            }

            // POST: Admin/Cars/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("CarId,Title,Alias,CarCategoryId,Brand,Model,Seats,Transmission,FuelType,Year,LicensePlate,Description,Detail,Image,Price,PriceSale,Quantity,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,IsNew,IsBestSeller,UnitInStock,IsActive,Star")] TbProduct tbCar)
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
                ViewData["CarCategoryId"] = new SelectList(_context.TbProductCategories, "CarCategoryId", "Title", tbCar.CategoryProductId);
                return View(tbCar);
            }

            // POST: Admin/Cars/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("CarId,Title,Alias,CarCategoryId,Brand,Model,Seats,Transmission,FuelType,Year,LicensePlate,Description,Detail,Image,Price,PriceSale,Quantity,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,IsNew,IsBestSeller,UnitInStock,IsActive,Star")] TbProduct tbCar)
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
                var tbCar = await _context.TbProducts.FindAsync(id);
                if (tbCar != null)
                {
                    _context.TbProducts.Remove(tbCar);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool TbCarExists(int id)
            {
                return _context.TbProducts.Any(e => e.ProductId == id);
            }
        }
    
}
