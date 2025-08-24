using LoginDemo.Data;
using LoginDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoginDemo.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _hasher;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<User>();
        }

        // GET: Register
        public IActionResult Register() => View();

        // POST: Register
        [HttpPost]
        public IActionResult Register(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Message = "Email already registered";
                return View();
            }

            var user = new User
            {
                Email = email
            };
            user.PasswordHash = _hasher.HashPassword(user, password);

            _context.Users.Add(user);
            _context.SaveChanges();

            ViewBag.Message = "Registration successful. Please login.";
            return View();
        }

        // GET: Login
        public IActionResult Login() => View();

        // POST: Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "Invalid login.";
                return View();
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                ViewBag.Message = "Invalid login.";
                return View();
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                // Login successful
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Invalid login.";
                return View();
            }
        }
    }
}
