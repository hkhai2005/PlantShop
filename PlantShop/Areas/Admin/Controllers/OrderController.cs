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
    public class OrderController : Controller
    {
        private readonly PlantShopDbContext _context;

        public OrderController(PlantShopDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index(string searchTerm, int? orderStatusId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.TbOrders
                .Include(o => o.OrderStatus)
                .OrderBy(o => o.CreatedDate)
                .AsQueryable();

            // Lọc theo từ khóa (Mã đơn, tên KH, SĐT)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o => o.Code.Contains(searchTerm) ||
                                         o.CustomerName.Contains(searchTerm) ||
                                         o.Phone.Contains(searchTerm));
            }

            // Lọc theo trạng thái
            if (orderStatusId.HasValue && orderStatusId > 0)
            {
                query = query.Where(o => o.OrderStatusId == orderStatusId);
            }

            // Lọc theo ngày
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedDate >= fromDate);
            }
            if (toDate.HasValue)
            {
                var endDate = toDate.Value.AddDays(1);
                query = query.Where(o => o.CreatedDate <= endDate);
            }

            ViewBag.OrderStatusList = new SelectList(await _context.TbOrderStatuses.ToListAsync(), "OrderStatusId", "Name");
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedStatus = orderStatusId;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            var orders = await query.ToListAsync();
            return View(orders);
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.TbOrders
                .Include(o => o.OrderStatus)
                .Include(o => o.TbOrderDetails)          // Sửa: OrderDetails, không phải OrderDetailsId
                    .ThenInclude(od => od.Product)      // ThenInclude sau OrderDetails
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.TbOrders
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.OrderId == id);  // Sửa: dùng FirstOrDefaultAsync thay vì FindAsync để Include hoạt động

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.OrderStatusList = new SelectList(await _context.TbOrderStatuses.ToListAsync(), "OrderStatusId", "Name", order.OrderStatusId);
            return View(order);
        }

        // POST: Admin/Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TbOrder order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingOrder = await _context.TbOrders.FindAsync(id);
                    if (existingOrder == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường được phép sửa
                    existingOrder.OrderStatusId = order.OrderStatusId;
                    existingOrder.Note = order.Note;
                    existingOrder.ModifiedDate = DateTime.Now;
                    existingOrder.ModifiedBy = User.Identity?.Name ?? "Admin";

                    // Nếu chuyển sang trạng thái hủy
                    var canceledStatus = await _context.TbOrderStatuses
                        .FirstOrDefaultAsync(s => s.Name.Contains("Hủy") || s.Name.Contains("Cancel"));

                    if (canceledStatus != null && order.OrderStatusId == canceledStatus.OrderStatusId)
                    {
                        if (string.IsNullOrEmpty(order.CancelReason))
                        {
                            ModelState.AddModelError("CancelReason", "Vui lòng nhập lý do hủy đơn hàng");
                            ViewBag.OrderStatusList = new SelectList(await _context.TbOrderStatuses.ToListAsync(), "OrderStatusId", "Name", order.OrderStatusId);
                            return View(order);
                        }
                        existingOrder.CancelReason = order.CancelReason;
                        existingOrder.CanceledDate = DateTime.Now;
                    }

                    _context.Update(existingOrder);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật đơn hàng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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

            ViewBag.OrderStatusList = new SelectList(await _context.TbOrderStatuses.ToListAsync(), "OrderStatusId", "Name", order.OrderStatusId);
            return View(order);
        }

        //GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.TbOrders
                .Include(o => o.OrderStatus)
                .Include(o => o.TbOrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.TbOrders
                .Include(o => o.TbOrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order != null)
            {
                // Xóa chi tiết đơn hàng trước
                _context.TbOrderDetails.RemoveRange(order.TbOrderDetails);
                // Xóa đơn hàng
                _context.TbOrders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa đơn hàng thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.TbOrders.Any(e => e.OrderId == id);
        }
    }
}