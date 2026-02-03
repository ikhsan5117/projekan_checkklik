using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using AMRVI.Data;
using AMRVI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AMRVI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive);

                // Plain text password comparison (DEV ONLY - NOT SECURE)
                if (user != null && user.Password == model.Password)
                {
                    // Create claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("FullName", user.FullName),
                        new Claim("Department", user.Department ?? "N/A"),
                        new Claim(ClaimTypes.Email, user.Email ?? "No Email"),
                        new Claim("NIK", user.Id.ToString()) // Using ID as NIK for now
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Update LastLogin
                    user.LastLogin = DateTime.Now;
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        // Safe check: If returnUrl prevents Operator from entering Dashboard
                        if ((returnUrl == "/" || returnUrl.ToLower().Contains("/home")) && 
                            user.Role != "Administrator" && user.Role != "Admin" && user.Role != "Supervisor")
                        {
                             return RedirectToAction("Index", "Inspection");
                        }
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        // Role-based Redirect
                        if (user.Role == "Administrator" || user.Role == "Admin" || user.Role == "Supervisor")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            // Operator / User: Direct to Inspection, Skip Dashboard
                            return RedirectToAction("Index", "Inspection");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
