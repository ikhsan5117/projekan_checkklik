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
                // Cari user di tabel yang sesuai dengan Plant yang dipilih
                Models.Interfaces.IUser? user = model.Plant switch
                {
                    "BTR" => await _context.Users_BTR.FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive),
                    "HOSE" => await _context.Users_HOSE.FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive),
                    "MOLDED" => await _context.Users_MOLDED.FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive),
                    "MIXING" => await _context.Users_MIXING.FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive),
                    _ => await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive) // RVI default
                };

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
                        new Claim("NIK", user.Id.ToString()),
                        new Claim("Plant", model.Plant) // PENTING: Simpan Plant di Claim
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

                    // Update LastLogin (harus cast ke concrete type untuk update)
                    if (model.Plant == "BTR")
                    {
                        var btrUser = await _context.Users_BTR.FindAsync(user.Id);
                        if (btrUser != null) { btrUser.LastLogin = DateTime.Now; }
                    }
                    else if (model.Plant == "HOSE")
                    {
                        var hoseUser = await _context.Users_HOSE.FindAsync(user.Id);
                        if (hoseUser != null) { hoseUser.LastLogin = DateTime.Now; }
                    }
                    else if (model.Plant == "MOLDED")
                    {
                        var moldedUser = await _context.Users_MOLDED.FindAsync(user.Id);
                        if (moldedUser != null) { moldedUser.LastLogin = DateTime.Now; }
                    }
                    else if (model.Plant == "MIXING")
                    {
                        var mixingUser = await _context.Users_MIXING.FindAsync(user.Id);
                        if (mixingUser != null) { mixingUser.LastLogin = DateTime.Now; }
                    }
                    else // RVI
                    {
                        var rviUser = await _context.Users.FindAsync(user.Id);
                        if (rviUser != null) { rviUser.LastLogin = DateTime.Now; }
                    }
                    
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
