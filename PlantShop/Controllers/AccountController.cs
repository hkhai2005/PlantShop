using PlantShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Do_An_Thue_Xe.Controllers
{
    public class AccountController : Controller
    {
        private readonly PlantShopDbContext _context;

        public AccountController(PlantShopDbContext context)
        {
            _context = context;
        }

        // =======================
        // 1. ĐĂNG KÝ (GET)
        // =======================
        public IActionResult Register()
        {
            return View();
        }

        // =======================
        // 2. XỬ LÝ ĐĂNG KÝ (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Register(TbAccount account)
        {

            account.RoleId = 4;                // Customer
            account.IsActive = true;

            ModelState.Remove("Username"); // ❗ xóa lỗi validation

            account.Username = account.Email;
            if (ModelState.IsValid)
            {
                //Kiểm tra email trùng
                var checkEmail = await _context.TbAccounts
                    .FirstOrDefaultAsync(a => a.Email == account.Email);

                if (checkEmail != null)
                {
                    ViewBag.Error = "Email này đã được sử dụng!";
                    return View(account);
                }

                //// Mặc định là Customer
                //account.RoleId = 2;

                _context.TbAccounts.Add(account);
                await _context.SaveChangesAsync();

                //TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(account);
        }

        // =======================
        // 3. ĐĂNG NHẬP (GET)
        // =======================
        public IActionResult Login()
        {
            return View();
        }

        // =======================
        // 4. XỬ LÝ ĐĂNG NHẬP (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var account = await _context.TbAccounts
                .FirstOrDefaultAsync(a => a.Email == email && a.Password == password);

            if (account != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.FullName),
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim("AccountId", account.AccountId.ToString()),
                    new Claim("RoleId", account.RoleId.ToString())
                };

                var identity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                // Phân quyền
                if (account.RoleId == 1)
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            return View();
        }

        // =======================
        // 5. ĐĂNG XUẤT
        // =======================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
