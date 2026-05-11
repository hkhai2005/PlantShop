using PlantShop.Models;
using PlantShop.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PlantShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly PlantShopDbContext _context;
        public LoginController(PlantShopDbContext context)
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
            //string password = HashMD5.GetMD5(account.Password);
            var check = _context.TbAccounts.Where(m => m.Username == account.Username && m.Password == account.Password)
                .FirstOrDefault();

            if (check == null)
            {
                Function._Message = "Invalid Username or Password";
                return RedirectToAction("Index", "Register");
            }

            Function._Message = string.Empty;
            Function._AccountId = check.AccountId;
            Function._Username = check.Username;
            Function._Password = check.Password;
            return RedirectToAction("Index", "Home");

        }
    }
}
