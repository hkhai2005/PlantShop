using PlantShop.Models;
using PlantShop.Services.VnPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;


namespace PlantShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IVnPayService _vnPayService;

        private readonly PlantShopDbContext _context;
        public CheckoutController(IVnPayService vnPayService, PlantShopDbContext context)
        {
            _vnPayService = vnPayService;

            _context = context;
        }
        public IActionResult Index()
        {
            // 1. Lấy giỏ hàng từ Session
            //var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var cartJson =
    HttpContext.Session.GetString("GioHang");

            var cart =
                string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            // 2. Nếu giỏ hàng rỗng thì đá về trang chủ hoặc trang giỏ hàng
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // 3. Truyền View kèm dữ liệu giỏ hàng để hiển thị
            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(item => item.Total);

            return View(); // Trả về View để khách điền thông tin
        }
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {          
                var response = _vnPayService.PaymentExecute(Request.Query);
                return View(response);
        }
    }
}
