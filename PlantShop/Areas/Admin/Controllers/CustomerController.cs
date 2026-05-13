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
    public class CustomerController : Controller
    {
        private readonly  PlantShopDbContext _context;

        public CustomerController(PlantShopDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Customer
        public async Task<IActionResult> Index()
        {
            var customers = await _context.TbCustomers.OrderByDescending(c => c.CustomerId).ToListAsync();
            return View(customers);
        }

        // GET: Admin/Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.TbCustomers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,FullName,Birthday,Avatar,Phone,Email,Address,IsActive,Gender,Ward,District,Province,VerifyEmail,VerifyPhone")] TbCustomer customer)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra username đã tồn tại chưa
                if (_context.TbCustomers.Any(c => c.Username == customer.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại!");
                    return View(customer);
                }

                customer.LastLogin = null;
                customer.ResetPasswordToken = null;
                customer.TokenExpiry = null;

                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.TbCustomers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Username,Password,FullName,Birthday,Avatar,Phone,Email,Address,LastLogin,IsActive,Gender,Ward,District,Province,VerifyEmail,VerifyPhone,ResetPasswordToken,TokenExpiry")] TbCustomer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu password để trống thì giữ nguyên password cũ
                    if (string.IsNullOrEmpty(customer.Password))
                    {
                        var existingCustomer = await _context.TbCustomers.AsNoTracking()
                            .FirstOrDefaultAsync(c => c.CustomerId == id);
                        if (existingCustomer != null)
                        {
                            customer.Password = existingCustomer.Password;
                        }
                    }

                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Admin/Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.TbCustomers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.TbCustomers.FindAsync(id);
            if (customer != null)
            {
                _context.TbCustomers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa khách hàng thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.TbCustomers.Any(e => e.CustomerId == id);
        }
    }
}