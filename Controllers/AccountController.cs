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
            Console.WriteLine($"[LOGIN DEBUG] Request received for Plant: {model.Plant}, IsAdminLogin: {model.IsAdminLogin}");

            // Validasi manual: Plant wajib diisi
            if (string.IsNullOrWhiteSpace(model.Plant))
            {
                ModelState.AddModelError("Plant", "Plant harus dipilih.");
                return View(model);
            }

            // ============================================
            // LOGIN SEBAGAI ADMIN: perlu Username & Password
            // ============================================
            if (model.IsAdminLogin)
            {
                if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Username dan Password harus diisi untuk login Admin.");
                    return View(model);
                }

                Models.Interfaces.IUser? user = model.Plant switch
                {
                    "BTR"    => await _context.Users_BTR.FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower() && u.IsActive),
                    "HOSE"   => await _context.Users_HOSE.FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower() && u.IsActive),
                    "MOLDED" => await _context.Users_MOLDED.FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower() && u.IsActive),
                    "MIXING" => await _context.Users_MIXING.FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower() && u.IsActive),
                    _        => await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower() && u.IsActive)
                };

                Console.WriteLine($"[LOGIN DEBUG] Admin user found? {(user != null ? "YES" : "NO")}");

                if (user == null || !string.Equals(user.Password, model.Password, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "Username atau Password tidak valid.");
                    return View(model);
                }

                // Pastikan yang login admin/supervisor
                if (user.Role != "Administrator" && user.Role != "Admin" && user.Role != "Supervisor")
                {
                    ModelState.AddModelError(string.Empty, "Akun ini tidak memiliki hak Admin.");
                    return View(model);
                }

                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FullName", user.FullName),
                    new Claim("Department", ""), // Admin: department diisi nanti
                    new Claim(ClaimTypes.Email, user.Email ?? "No Email"),
                    new Claim("NIK", user.Id.ToString()),
                    new Claim("Plant", model.Plant)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    new AuthenticationProperties { IsPersistent = model.RememberMe });

                // Update LastLogin
                await UpdateLastLogin(model.Plant, user.Id);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            // ============================================
            // LOGIN SEBAGAI OPERATOR: cukup pilih Plant
            // ============================================
            // Buat session sebagai Operator generik dari plant tersebut
            var (plantLabel, plantId) = model.Plant switch
            {
                "BTR"    => ("BTR", "2"),
                "HOSE"   => ("HOSE", "3"),
                "MOLDED" => ("MOLDED", "4"),
                "MIXING" => ("MIXING", "5"),
                _        => ("RVI", "1")
            };

            var operatorClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"Auto_{model.Plant}"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("FullName", $"Viewer {plantLabel}"),
                new Claim("Department", "Production"),
                new Claim(ClaimTypes.Email, ""),
                new Claim("NIK", plantId), // ID mengikuti ID Plant master
                new Claim("Plant", model.Plant)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(operatorClaims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Langsung masuk ke Dashboard Utama
            return RedirectToAction("Index", "Home");
        }

        private async Task UpdateLastLogin(string plant, int userId)
        {
            if (plant == "BTR")
            {
                var u = await _context.Users_BTR.FindAsync(userId);
                if (u != null) u.LastLogin = DateTime.Now;
            }
            else if (plant == "HOSE")
            {
                var u = await _context.Users_HOSE.FindAsync(userId);
                if (u != null) u.LastLogin = DateTime.Now;
            }
            else if (plant == "MOLDED")
            {
                var u = await _context.Users_MOLDED.FindAsync(userId);
                if (u != null) u.LastLogin = DateTime.Now;
            }
            else if (plant == "MIXING")
            {
                var u = await _context.Users_MIXING.FindAsync(userId);
                if (u != null) u.LastLogin = DateTime.Now;
            }
            else
            {
                var u = await _context.Users.FindAsync(userId);
                if (u != null) u.LastLogin = DateTime.Now;
            }
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
