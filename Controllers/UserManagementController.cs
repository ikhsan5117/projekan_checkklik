using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using AMRVI.Models.Interfaces; // Use interfaces
using AMRVI.Services; // Use PlantService
using OfficeOpenXml;

using Microsoft.AspNetCore.Authorization;

namespace AMRVI.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PlantService _plantService;

        public UserManagementController(ApplicationDbContext context, PlantService plantService)
        {
            _context = context;
            _plantService = plantService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(string? role, bool? isActive)
        {
            // Use PlantService to get the correct users query
            var query = _plantService.GetUsers();

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            // Must execute query and map to anonymous object
            // Cannot rely on implicit casting for IQueryable<IUser> in complex scenarios
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
            var user = await _plantService.GetUsers()
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
                var plant = _plantService.GetPlantName();
                
                // Check duplicates (using PlantService query)
                if (await _plantService.GetUsers().AnyAsync(u => u.Username == username))
                {
                    return Json(new { success = false, message = "Username sudah digunakan di plant ini" });
                }

                if (await _plantService.GetUsers().AnyAsync(u => u.Email == email))
                {
                    return Json(new { success = false, message = "Email sudah digunakan di plant ini" });
                }

                // Add to specific table based on plant
                switch (plant)
                {
                    case "BTR":
                        _context.Users_BTR.Add(new User_BTR { Username = username, FullName = fullName, Email = email, Password = password, Role = role, Department = department, IsActive = true, CreatedAt = DateTime.Now });
                        break;
                    case "HOSE":
                        _context.Users_HOSE.Add(new User_HOSE { Username = username, FullName = fullName, Email = email, Password = password, Role = role, Department = department, IsActive = true, CreatedAt = DateTime.Now });
                        break;
                    case "MOLDED":
                        _context.Users_MOLDED.Add(new User_MOLDED { Username = username, FullName = fullName, Email = email, Password = password, Role = role, Department = department, IsActive = true, CreatedAt = DateTime.Now });
                        break;
                    case "MIXING":
                        _context.Users_MIXING.Add(new User_MIXING { Username = username, FullName = fullName, Email = email, Password = password, Role = role, Department = department, IsActive = true, CreatedAt = DateTime.Now });
                        break;
                    default: // RVI
                        _context.Users.Add(new User { Username = username, FullName = fullName, Email = email, Password = password, Role = role, Department = department, IsActive = true, CreatedAt = DateTime.Now });
                        break;
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
        public async Task<IActionResult> UpdateUser(int id, string username, string fullName, string email, string? password, string role, string? department, bool isActive)
        {
            try
            {
                var plant = _plantService.GetPlantName();

                // Check duplicates excluding current user
                if (await _plantService.GetUsers().AnyAsync(u => u.Username == username && u.Id != id))
                    return Json(new { success = false, message = "Username sudah digunakan" });
                
                if (await _plantService.GetUsers().AnyAsync(u => u.Email == email && u.Id != id))
                    return Json(new { success = false, message = "Email sudah digunakan" });

                // Update specific entity
                // Note: We cannot easily use generic IUser for update because DbSet.FindAsync returns localized entity
                // We have to repeat switch case
                switch (plant)
                {
                     case "BTR":
                        var uBTR = await _context.Users_BTR.FindAsync(id);
                        if(uBTR == null) return Json(new { success = false, message = "User not found" });
                        uBTR.Username = username; uBTR.FullName = fullName; uBTR.Email = email; uBTR.Role = role; uBTR.Department = department; uBTR.IsActive = isActive;
                        if(!string.IsNullOrWhiteSpace(password)) uBTR.Password = password;
                        break;
                    case "HOSE":
                        var uHOSE = await _context.Users_HOSE.FindAsync(id);
                        if(uHOSE == null) return Json(new { success = false, message = "User not found" });
                        uHOSE.Username = username; uHOSE.FullName = fullName; uHOSE.Email = email; uHOSE.Role = role; uHOSE.Department = department; uHOSE.IsActive = isActive;
                        if(!string.IsNullOrWhiteSpace(password)) uHOSE.Password = password;
                        break;
                    case "MOLDED":
                        var uMOLDED = await _context.Users_MOLDED.FindAsync(id);
                        if(uMOLDED == null) return Json(new { success = false, message = "User not found" });
                        uMOLDED.Username = username; uMOLDED.FullName = fullName; uMOLDED.Email = email; uMOLDED.Role = role; uMOLDED.Department = department; uMOLDED.IsActive = isActive;
                        if(!string.IsNullOrWhiteSpace(password)) uMOLDED.Password = password;
                        break;
                    case "MIXING":
                        var uMIXING = await _context.Users_MIXING.FindAsync(id);
                        if(uMIXING == null) return Json(new { success = false, message = "User not found" });
                        uMIXING.Username = username; uMIXING.FullName = fullName; uMIXING.Email = email; uMIXING.Role = role; uMIXING.Department = department; uMIXING.IsActive = isActive;
                        if(!string.IsNullOrWhiteSpace(password)) uMIXING.Password = password;
                        break;
                    default:
                        var uRVI = await _context.Users.FindAsync(id);
                        if(uRVI == null) return Json(new { success = false, message = "User not found" });
                        uRVI.Username = username; uRVI.FullName = fullName; uRVI.Email = email; uRVI.Role = role; uRVI.Department = department; uRVI.IsActive = isActive;
                        if(!string.IsNullOrWhiteSpace(password)) uRVI.Password = password;
                        break;
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
            var plant = _plantService.GetPlantName();
            
            switch (plant)
            {
                case "BTR":
                    var uBTR = await _context.Users_BTR.FindAsync(id);
                    if(uBTR != null) _context.Users_BTR.Remove(uBTR);
                    break;
                case "HOSE":
                    var uHOSE = await _context.Users_HOSE.FindAsync(id);
                    if(uHOSE != null) _context.Users_HOSE.Remove(uHOSE);
                    break;
                case "MOLDED":
                    var uMOLDED = await _context.Users_MOLDED.FindAsync(id);
                    if(uMOLDED != null) _context.Users_MOLDED.Remove(uMOLDED);
                    break;
                case "MIXING":
                    var uMIXING = await _context.Users_MIXING.FindAsync(id);
                    if(uMIXING != null) _context.Users_MIXING.Remove(uMIXING);
                    break;
                default:
                    var uRVI = await _context.Users.FindAsync(id);
                    if(uRVI != null) _context.Users.Remove(uRVI);
                    break;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string? role, bool? isActive)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var query = _plantService.GetUsers();

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
                var workSheetName = $"User Data {_plantService.GetPlantName()}";
                var worksheet = package.Workbook.Worksheets.Add(workSheetName);

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

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Users_{_plantService.GetPlantName()}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
        }

        // Import Excel logic needs to be aware of the plant too
        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "File tidak valid" });

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var plant = _plantService.GetPlantName();

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

                                // Check duplicates in CURRENT PLANT
                                // using ToList/FirstOrDefault to check duplicates might be expensive inside loop but necessary if PlantService.GetUsers is IQueryable
                                // Better to check against list of existing usernames fetched once, but for now simple check:
                                bool exists = false;
                                switch(plant) {
                                    case "BTR": exists = await _context.Users_BTR.AnyAsync(u => u.Username == username || u.Email == email); break;
                                    case "HOSE": exists = await _context.Users_HOSE.AnyAsync(u => u.Username == username || u.Email == email); break;
                                    case "MOLDED": exists = await _context.Users_MOLDED.AnyAsync(u => u.Username == username || u.Email == email); break;
                                    case "MIXING": exists = await _context.Users_MIXING.AnyAsync(u => u.Username == username || u.Email == email); break;
                                    default: exists = await _context.Users.AnyAsync(u => u.Username == username || u.Email == email); break;
                                }

                                if (exists)
                                {
                                    errors.Add($"Baris {row}: Username atau email sudah ada");
                                    continue;
                                }

                                switch (plant)
                                {
                                    case "BTR":
                                        _context.Users_BTR.Add(new User_BTR { Username = username, FullName = fullName ?? username, Email = email, Password = password, Role = role ?? "User", Department = department, IsActive = true, CreatedAt = DateTime.Now });
                                        break;
                                    case "HOSE":
                                        _context.Users_HOSE.Add(new User_HOSE { Username = username, FullName = fullName ?? username, Email = email, Password = password, Role = role ?? "User", Department = department, IsActive = true, CreatedAt = DateTime.Now });
                                        break;
                                    case "MOLDED":
                                        _context.Users_MOLDED.Add(new User_MOLDED { Username = username, FullName = fullName ?? username, Email = email, Password = password, Role = role ?? "User", Department = department, IsActive = true, CreatedAt = DateTime.Now });
                                        break;
                                    case "MIXING":
                                        _context.Users_MIXING.Add(new User_MIXING { Username = username, FullName = fullName ?? username, Email = email, Password = password, Role = role ?? "User", Department = department, IsActive = true, CreatedAt = DateTime.Now });
                                        break;
                                    default:
                                        _context.Users.Add(new User { Username = username, FullName = fullName ?? username, Email = email, Password = password, Role = role ?? "User", Department = department, IsActive = true, CreatedAt = DateTime.Now });
                                        break;
                                }

                                imported++;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Baris {row}: {ex.Message}");
                            }
                        }

                        await _context.SaveChangesAsync();

                        var message = $"Berhasil import {imported} user ke {plant}.";
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
             // Template logic remains mostly same
             return base.File(System.IO.File.ReadAllBytes("wwwroot/templates/Template_Users.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template_Users.xlsx");
             // Or recreate it
             ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
             using (var package = new ExcelPackage())
             {
                 var worksheet = package.Workbook.Worksheets.Add("Template Users");
                 worksheet.Cells[1, 1].Value = "No";
                 worksheet.Cells[1, 2].Value = "Username";
                 worksheet.Cells[1, 3].Value = "Full Name";
                 worksheet.Cells[1, 4].Value = "Email";
                 worksheet.Cells[1, 5].Value = "Password";
                 worksheet.Cells[1, 6].Value = "Role";
                 worksheet.Cells[1, 7].Value = "Department";
                 
                 using (var range = worksheet.Cells[1, 1, 1, 7]) { range.Style.Font.Bold = true; }
                 
                 worksheet.Cells[2, 1].Value = 1;
                 worksheet.Cells[2, 2].Value = "user1";
                 worksheet.Cells[2, 3].Value = "User One";
                 worksheet.Cells[2, 4].Value = "user1@amrvi.com";
                 worksheet.Cells[2, 5].Value = "123456";
                 worksheet.Cells[2, 6].Value = "User";
                 worksheet.Cells[2, 7].Value = "Production";

                 var stream = new MemoryStream();
                 package.SaveAs(stream);
                 stream.Position = 0;
                 return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template_Users.xlsx");
             }
        }

    }
}
