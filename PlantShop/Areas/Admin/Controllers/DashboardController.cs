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
    public class DashboardController : Controller
    {
        private readonly PlantShopDbContext _context;

        public DashboardController(PlantShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new DashboardViewModel();

            // ========== 1. TỔNG QUAN ==========
            // Tổng số đơn hàng
            dashboardData.TotalOrders = await _context.TbOrders.CountAsync();

            // Tổng doanh thu (đơn hàng hoàn thành)
            dashboardData.TotalRevenue = await _context.TbOrders
                .Where(o => o.OrderStatus.Name == "Hoàn thành" || o.OrderStatusId == 4)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            // Tổng số khách hàng
            dashboardData.TotalCustomers = await _context.TbCustomers.CountAsync();

            // Tổng số sản phẩm
            dashboardData.TotalProducts = await _context.TbProducts.CountAsync();

            // ========== 2. DOANH THU THEO NGÀY (7 ngày gần nhất) ==========
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.Date.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            dashboardData.RevenueByDay = new List<RevenueByDayDto>();
            foreach (var day in last7Days)
            {
                var revenue = await _context.TbOrders
                    .Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value.Date == day)
                    .Where(o => o.OrderStatus.Name == "Hoàn thành" || o.OrderStatusId == 4)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

                dashboardData.RevenueByDay.Add(new RevenueByDayDto
                {
                    Date = day.ToString("dd/MM"),
                    Revenue = revenue
                });
            }

            // ========== 3. DOANH THU THEO THÁNG (12 tháng gần nhất) ==========
            var last12Months = Enumerable.Range(0, 12)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(m => m)
                .ToList();

            dashboardData.RevenueByMonth = new List<RevenueByMonthDto>();
            foreach (var month in last12Months)
            {
                var revenue = await _context.TbOrders
                    .Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value.Year == month.Year && o.CreatedDate.Value.Month == month.Month)
                    .Where(o => o.OrderStatus.Name == "Hoàn thành" || o.OrderStatusId == 4)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

                dashboardData.RevenueByMonth.Add(new RevenueByMonthDto
                {
                    Month = month.ToString("MM/yyyy"),
                    Revenue = revenue
                });
            }

            // ========== 4. TOP SẢN PHẨM BÁN CHẠY ==========
            dashboardData.TopProducts = await _context.TbOrderDetails
                .Include(od => od.Product)
                .Where(od => od.Product != null)
                .GroupBy(od => new { od.ProductId, od.Product.Title, od.Product.Image })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Title,
                    Image = g.Key.Image,
                    TotalQuantity = g.Sum(od => od.Quantity) ?? 0,
                    TotalRevenue = g.Sum(od => (od.Price * od.Quantity)) ?? 0
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(10)
                .ToListAsync();

            // ========== 5. TRẠNG THÁI ĐƠN HÀNG ==========
            dashboardData.OrderStatusStats = await _context.TbOrders
                .GroupBy(o => o.OrderStatus.Name)
                .Select(g => new OrderStatusStatDto
                {
                    StatusName = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // ========== 6. TOP KHÁCH HÀNG MUA NHIỀU ==========
            dashboardData.TopCustomers = await _context.TbOrders
                .Where(o => o.CustomerName != null)
                .GroupBy(o => new { o.CustomerId, o.CustomerName, o.Phone })
                .Select(g => new TopCustomerDto
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    Phone = g.Key.Phone,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount) ?? 0
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(10)
                .ToListAsync();

            // ========== 7. DOANH THU THEO TUẦN TRONG THÁNG ==========
            var currentMonth = DateTime.Now;
            var weeksInMonth = GetWeeksInMonth(currentMonth.Year, currentMonth.Month);
            dashboardData.RevenueByWeek = new List<RevenueByWeekDto>();

            foreach (var week in weeksInMonth)
            {
                var revenue = await _context.TbOrders
                    .Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value >= week.StartDate && o.CreatedDate.Value <= week.EndDate)
                    .Where(o => o.OrderStatus.Name == "Hoàn thành" || o.OrderStatusId == 4)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

                dashboardData.RevenueByWeek.Add(new RevenueByWeekDto
                {
                    WeekName = week.Name,
                    Revenue = revenue
                });
            }

            return View(dashboardData);
        }

        private List<WeekInfo> GetWeeksInMonth(int year, int month)
        {
            var weeks = new List<WeekInfo>();
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var startDate = firstDayOfMonth;
            int weekNumber = 1;

            while (startDate <= lastDayOfMonth)
            {
                var endDate = startDate.AddDays(6);
                if (endDate > lastDayOfMonth) endDate = lastDayOfMonth;

                weeks.Add(new WeekInfo
                {
                    Name = $"Tuần {weekNumber}",
                    StartDate = startDate,
                    EndDate = endDate
                });

                startDate = startDate.AddDays(7);
                weekNumber++;
            }

            return weeks;
        }
    }

    // ========== VIEW MODEL ==========
    public class DashboardViewModel
    {
        // Tổng quan
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }

        // Doanh thu theo ngày
        public List<RevenueByDayDto> RevenueByDay { get; set; }

        // Doanh thu theo tháng
        public List<RevenueByMonthDto> RevenueByMonth { get; set; }

        // Doanh thu theo tuần
        public List<RevenueByWeekDto> RevenueByWeek { get; set; }

        // Top sản phẩm bán chạy
        public List<TopProductDto> TopProducts { get; set; }

        // Trạng thái đơn hàng
        public List<OrderStatusStatDto> OrderStatusStats { get; set; }

        // Top khách hàng
        public List<TopCustomerDto> TopCustomers { get; set; }
    }

    public class RevenueByDayDto
    {
        public string Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RevenueByMonthDto
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RevenueByWeekDto
    {
        public string WeekName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopProductDto
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class OrderStatusStatDto
    {
        public string StatusName { get; set; }
        public int Count { get; set; }
    }

    public class TopCustomerDto
    {
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class WeekInfo
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}