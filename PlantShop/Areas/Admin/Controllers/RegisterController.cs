using PlantShop.Models;
using PlantShop.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;
using PlantShop.Utilities;
using System.Linq;

namespace PlantShop.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class RegisterController : Controller
    {
        private readonly PlantShopDbContext _context;

        public RegisterController(PlantShopDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            // nếu dùng Function._Message thì có thể truyền ra view
            ViewBag.Message = Function._Message;
            return View();
        }
        [HttpPost]
        public IActionResult Index(TbAccount account)
        {
            if (account == null)
            {
                return NotFound();
            }

            var check = _context.TbAccounts
                .Where(m => m.Username == account.Username)
                .FirstOrDefault();

            if (check != null)
            {
                Function._Message = "Trùng tài khoản";
                return RedirectToAction("Index", "Register");
            }

            Function._Message = string.Empty;
            account.Password = HashMD5.GetMD5(account.Password != null ? account.Password : "");
            _context.Add(account);
            _context.SaveChanges();
            return RedirectToAction("Index", "Login");
        }

    }
}