using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LoginDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LoginDemo.Models; // Add this if your User class is in Models namespace

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _hasher;

     public AccountController(AppDbContext context, IPasswordHasher<User> hasher)
    {
        _context = context;
        _hasher = hasher;
     }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

   
    
    [HttpPost]
public async Task<IActionResult> Login(string email, string password)
{
    var user = _context.Users.FirstOrDefault(u => u.Email == email);
    if (user == null || string.IsNullOrEmpty(user.PasswordHash))
    {
        // ❌ Redirect to Result page (failed)
        return RedirectToAction("Result", new { success = false });
    }

    var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
    if (result == PasswordVerificationResult.Success)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email ?? string.Empty)
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // ✅ Redirect to Result page (success)
        return RedirectToAction("Result", new { success = true });
    }

    // ❌ Redirect to Result page (failed)
    return RedirectToAction("Result", new { success = false });
}

    // ✅ New action to show Login Success / Failed
    public IActionResult Result(bool success)
    {
        ViewBag.Message = success ? "✅ Login Successful!" : "❌ Login Failed!";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}
