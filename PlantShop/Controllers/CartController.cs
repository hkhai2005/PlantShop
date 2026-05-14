using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;
using PlantShop;
using System.Text.Json;
using PlantShop.Services.VnPay;

namespace PlantShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly PlantShopDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(IVnPayService vnPayService, PlantShopDbContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        // Lưu giỏ hàng vào Session
        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        // Lấy số lượng giỏ hàng (cho Layout)
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(cart.Sum(x => x.Quantity));
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            var viewModel = new CartViewModel
            {
                Items = cart,
                SubTotal = cart.Sum(x => x.Total),
                ShippingFee = cart.Any() ? 30000 : 0,
                TotalQuantity = cart.Sum(x => x.Quantity)
            };
            return View(viewModel);
        }

        // Thêm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.TbProducts
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive == true);

            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(x => x.Product.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    Product = product,
                    Quantity = quantity
                });
            }

            SaveCart(cart);

            return Json(new
            {
                success = true,
                message = "Đã thêm vào giỏ hàng",
                cartCount = cart.Sum(x => x.Quantity)
            });
        }

        // Cập nhật số lượng
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }

            SaveCart(cart);

            return Json(new
            {
                success = true,
                itemTotal = item != null ? item.Total : 0,
                subTotal = cart.Sum(x => x.Total),
                shippingFee = cart.Any() ? 30000 : 0,
                total = cart.Sum(x => x.Total) + (cart.Any() ? 30000 : 0),
                cartCount = cart.Sum(x => x.Quantity)
            });
        }

        // Xóa sản phẩm khỏi giỏ
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProductId == productId);

            if (item != null)
                cart.Remove(item);

            SaveCart(cart);

            return Json(new
            {
                success = true,
                subTotal = cart.Sum(x => x.Total),
                shippingFee = cart.Any() ? 30000 : 0,
                total = cart.Sum(x => x.Total) + (cart.Any() ? 30000 : 0),
                cartCount = cart.Sum(x => x.Quantity)
            });
        }

        // Trang thanh toán
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            var viewModel = new CheckoutViewModel();

            // Nếu đã đăng nhập, lấy thông tin khách hàng
            if (User.Identity?.IsAuthenticated == true)
            {
                // Lấy CustomerId từ session hoặc claim
                // viewModel.CustomerId = ...
            }

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            return View(response);
        }
        // Xử lý thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tạo mã đơn hàng
            string orderCode = "DH" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            // Tính tổng tiền
            var subTotal = cart.Sum(x => x.Total);
            var shippingFee = 30000m;
            var totalAmount = subTotal + shippingFee;

            // Tạo đơn hàng
            var order = new TbOrder
            {
                Code = orderCode,
                CustomerId = model.CustomerId,
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Address = model.Address,
                TotalAmount = totalAmount,
                Quantity = cart.Sum(x => x.Quantity),
                OrderStatusId = 1, // Chờ xác nhận
                CreatedDate = DateTime.Now,
                CreatedBy = model.CustomerName,
                ModifiedDate = DateTime.Now,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = model.PaymentMethod == "COD" ? "Unpaid" : "Pending",
                ShippingFee = shippingFee,
                DiscountAmount = 0,
                Note = model.Note
            };

            _context.TbOrders.Add(order);
            await _context.SaveChangesAsync();

            // Thêm chi tiết đơn hàng
            foreach (var item in cart)
            {
                var orderDetail = new TbOrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.Product.ProductId,
                    Price = item.Price,
                    Quantity = item.Quantity
                };
                _context.TbOrderDetails.Add(orderDetail);

                // Cập nhật tồn kho
                var product = await _context.TbProducts.FindAsync(item.Product.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    product.UnitInStock -= item.Quantity;
                    product.SoldCount = (product.SoldCount ?? 0) + item.Quantity;
                }
            }

            await _context.SaveChangesAsync();

            // Xóa giỏ hàng khỏi Session
            HttpContext.Session.Remove(CartSessionKey);

            TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng: {orderCode}";
            return RedirectToAction("OrderSuccess", new { id = order.OrderId });
        }

        // Trang đặt hàng thành công
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var order = await _context.TbOrders
                .Include(o => o.TbOrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return RedirectToAction("Index");

            return View(order);
        }
        // Lấy tóm tắt đơn hàng cho trang checkout
        public IActionResult GetOrderSummary()
        {
            var cart = GetCart();
            var viewModel = new CartViewModel
            {
                Items = cart,
                SubTotal = cart.Sum(x => x.Total),
                ShippingFee = cart.Any() ? 30000 : 0,
                Discount = 0
            };
            return PartialView("_OrderSummary", viewModel);
        }
    }
}