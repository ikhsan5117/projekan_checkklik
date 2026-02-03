using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using OfficeOpenXml;

namespace AMRVI.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(string? role, bool? isActive)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            var users = await query
                .OrderBy(u => u.Username)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.Department,
                    u.IsActive,
                    u.CreatedAt,
                    u.LastLogin
                })
                .ToListAsync();

            return Json(new { success = true, data = users });
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.Department,
                    u.IsActive
                })
                .FirstOrDefaultAsync();

            if (user == null) return Json(new { success = false, message = "User not found" });
            return Json(new { success = true, data = user });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string username, string fullName, string email, string password, string role, string? department)
        {
            try
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    return Json(new { success = false, message = "Username sudah digunakan" });
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == email))
                {
                    return Json(new { success = false, message = "Email sudah digunakan" });
                }

                var user = new User
                {
                    Username = username,
                    FullName = fullName,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = role,
                    Department = department,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(int id, string username, string fullName, string email, string? password, string role, string? department, bool isActive)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return Json(new { success = false, message = "User not found" });

                // Check username uniqueness (exclude current user)
                if (await _context.Users.AnyAsync(u => u.Username == username && u.Id != id))
                {
                    return Json(new { success = false, message = "Username sudah digunakan" });
                }

                // Check email uniqueness (exclude current user)
                if (await _context.Users.AnyAsync(u => u.Email == email && u.Id != id))
                {
                    return Json(new { success = false, message = "Email sudah digunakan" });
                }

                user.Username = username;
                user.FullName = fullName;
                user.Email = email;
                user.Role = role;
                user.Department = department;
                user.IsActive = isActive;

                // Only update password if provided
                if (!string.IsNullOrWhiteSpace(password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return Json(new { success = false, message = "User not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string? role, bool? isActive)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            var users = await query.OrderBy(u => u.Username).ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("User Data");

                // Header
                worksheet.Cells[1, 1].Value = "No";
                worksheet.Cells[1, 2].Value = "Username";
                worksheet.Cells[1, 3].Value = "Full Name";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Role";
                worksheet.Cells[1, 6].Value = "Department";
                worksheet.Cells[1, 7].Value = "Status";
                worksheet.Cells[1, 8].Value = "Last Login";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(99, 102, 241));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Data
                int row = 2;
                foreach (var user in users)
                {
                    worksheet.Cells[row, 1].Value = row - 1;
                    worksheet.Cells[row, 2].Value = user.Username;
                    worksheet.Cells[row, 3].Value = user.FullName;
                    worksheet.Cells[row, 4].Value = user.Email;
                    worksheet.Cells[row, 5].Value = user.Role;
                    worksheet.Cells[row, 6].Value = user.Department;
                    worksheet.Cells[row, 7].Value = user.IsActive ? "Active" : "Inactive";
                    worksheet.Cells[row, 8].Value = user.LastLogin?.ToString("yyyy-MM-dd HH:mm");
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "File tidak valid" });

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;
                        int imported = 0;
                        var errors = new List<string>();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var username = worksheet.Cells[row, 2].Value?.ToString();
                                var fullName = worksheet.Cells[row, 3].Value?.ToString();
                                var email = worksheet.Cells[row, 4].Value?.ToString();
                                var password = worksheet.Cells[row, 5].Value?.ToString();
                                var role = worksheet.Cells[row, 6].Value?.ToString();
                                var department = worksheet.Cells[row, 7].Value?.ToString();

                                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                                {
                                    errors.Add($"Baris {row}: Data tidak lengkap");
                                    continue;
                                }

                                // Check duplicates
                                if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
                                {
                                    errors.Add($"Baris {row}: Username atau email sudah ada");
                                    continue;
                                }

                                var user = new User
                                {
                                    Username = username,
                                    FullName = fullName ?? username,
                                    Email = email,
                                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                                    Role = role ?? "User",
                                    Department = department,
                                    IsActive = true,
                                    CreatedAt = DateTime.Now
                                };

                                _context.Users.Add(user);
                                imported++;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Baris {row}: {ex.Message}");
                            }
                        }

                        await _context.SaveChangesAsync();

                        var message = $"Berhasil import {imported} user.";
                        if (errors.Any())
                        {
                            message += $" {errors.Count} error: " + string.Join("; ", errors.Take(5));
                        }

                        return Json(new { success = true, message = message, imported = imported, errors = errors.Count });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult DownloadTemplate()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Template Users");

                // Header
                worksheet.Cells[1, 1].Value = "No";
                worksheet.Cells[1, 2].Value = "Username";
                worksheet.Cells[1, 3].Value = "Full Name";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Password";
                worksheet.Cells[1, 6].Value = "Role";
                worksheet.Cells[1, 7].Value = "Department";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(99, 102, 241));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Sample data
                worksheet.Cells[2, 1].Value = 1;
                worksheet.Cells[2, 2].Value = "johndoe";
                worksheet.Cells[2, 3].Value = "John Doe";
                worksheet.Cells[2, 4].Value = "john@example.com";
                worksheet.Cells[2, 5].Value = "password123";
                worksheet.Cells[2, 6].Value = "User";
                worksheet.Cells[2, 7].Value = "Production";

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template_Users.xlsx");
            }
        }
    }
}
