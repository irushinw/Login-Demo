using Microsoft.AspNetCore.Mvc;
using LoginDemo.Data;
using LoginDemo.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace LoginDemo.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<string> _hasher = new PasswordHasher<string>();

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email already exists.";
                return View();
            }

            var user = new User
            {
                Email = email,
                PasswordHash = _hasher.HashPassword(null, password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Invalid login.";
                return View();
            }

            var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                return RedirectToAction("Welcome");
            }

            ViewBag.Error = "Invalid login.";
            return View();
        }

        public IActionResult Welcome() => View();
    }
}
