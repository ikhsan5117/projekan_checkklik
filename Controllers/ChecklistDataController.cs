using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using OfficeOpenXml;

namespace AMRVI.Controllers
{
    public class ChecklistDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ChecklistDataController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var machines = await _context.Machines
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
            return View(machines);
        }

        [HttpGet]
        public async Task<IActionResult> GetChecklistItems(int? machineId)
        {
            var query = _context.ChecklistItems.Include(ci => ci.Machine).AsQueryable();

            if (machineId.HasValue && machineId.Value > 0)
            {
                query = query.Where(ci => ci.MachineId == machineId);
            }

            var items = await query
                .Where(ci => ci.IsActive)
                .OrderBy(ci => ci.Machine.Name)
                .ThenBy(ci => ci.OrderNumber)
                .Select(ci => new
                {
                    ci.Id,
                    ci.DetailName,
                    ci.StandardDescription,
                    ci.OrderNumber,
                    ci.ImagePath,
                    MachineName = ci.Machine.Name
                })
                .ToListAsync();
            return Json(new { success = true, data = items });
        }

        [HttpPost]
        public async Task<IActionResult> AddChecklistItem(int machineId, string detailName, string standardDescription, int orderNumber, IFormFile? imageFile)
        {
            try
            {
                string imagePath = string.Empty;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "checklist");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    imagePath = $"/uploads/checklist/{uniqueFileName}";
                }

                var item = new ChecklistItem
                {
                    MachineId = machineId,
                    DetailName = detailName,
                    StandardDescription = standardDescription,
                    OrderNumber = orderNumber,
                    ImagePath = imagePath,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.ChecklistItems.Add(item);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChecklistItem(int id)
        {
            var item = await _context.ChecklistItems.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Not found" });

            _context.ChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetChecklistItem(int id)
        {
            var item = await _context.ChecklistItems
                .Where(ci => ci.Id == id)
                .Select(ci => new
                {
                    ci.Id,
                    ci.MachineId,
                    ci.DetailName,
                    ci.StandardDescription,
                    ci.OrderNumber,
                    ci.ImagePath
                })
                .FirstOrDefaultAsync();

            if (item == null) return Json(new { success = false, message = "Not found" });
            return Json(new { success = true, data = item });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateChecklistItem(int id, int machineId, string detailName, string standardDescription, int orderNumber, IFormFile? imageFile, bool keepExistingImage = true)
        {
            try
            {
                var item = await _context.ChecklistItems.FindAsync(id);
                if (item == null) return Json(new { success = false, message = "Item not found" });

                item.MachineId = machineId;
                item.DetailName = detailName;
                item.StandardDescription = standardDescription;
                item.OrderNumber = orderNumber;

                // Handle image update
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var oldImagePath = Path.Combine(_environment.WebRootPath, item.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Upload new image
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "checklist");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    item.ImagePath = $"/uploads/checklist/{uniqueFileName}";
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(int? machineId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var query = _context.ChecklistItems
                .Include(ci => ci.Machine)
                .Where(ci => ci.IsActive)
                .AsQueryable();

            if (machineId.HasValue && machineId.Value > 0)
            {
                query = query.Where(ci => ci.MachineId == machineId);
            }

            var items = await query
                .OrderBy(ci => ci.Machine.Name)
                .ThenBy(ci => ci.OrderNumber)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Checklist Data");

                // Header
                worksheet.Cells[1, 1].Value = "No";
                worksheet.Cells[1, 2].Value = "Mesin";
                worksheet.Cells[1, 3].Value = "Urutan";
                worksheet.Cells[1, 4].Value = "Bagian (Detail)";
                worksheet.Cells[1, 5].Value = "Standar";
                worksheet.Cells[1, 6].Value = "Path Gambar";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(16, 185, 129));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Data
                int row = 2;
                foreach (var item in items)
                {
                    worksheet.Cells[row, 1].Value = row - 1;
                    worksheet.Cells[row, 2].Value = item.Machine?.Name;
                    worksheet.Cells[row, 3].Value = item.OrderNumber;
                    worksheet.Cells[row, 4].Value = item.DetailName;
                    worksheet.Cells[row, 5].Value = item.StandardDescription;
                    worksheet.Cells[row, 6].Value = item.ImagePath;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = machineId.HasValue 
                    ? $"Checklist_Data_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                    : $"Checklist_All_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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

                        // Get all machines for lookup
                        var machines = await _context.Machines.ToDictionaryAsync(m => m.Name, m => m.Id);

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var machineName = worksheet.Cells[row, 2].Value?.ToString();
                                var orderNumberStr = worksheet.Cells[row, 3].Value?.ToString();
                                var detailName = worksheet.Cells[row, 4].Value?.ToString();
                                var standardDesc = worksheet.Cells[row, 5].Value?.ToString();

                                if (string.IsNullOrWhiteSpace(machineName) || string.IsNullOrWhiteSpace(detailName))
                                {
                                    errors.Add($"Baris {row}: Data tidak lengkap");
                                    continue;
                                }

                                if (!machines.ContainsKey(machineName))
                                {
                                    errors.Add($"Baris {row}: Mesin '{machineName}' tidak ditemukan");
                                    continue;
                                }

                                if (!int.TryParse(orderNumberStr, out int orderNumber))
                                {
                                    errors.Add($"Baris {row}: Nomor urut tidak valid");
                                    continue;
                                }

                                var item = new ChecklistItem
                                {
                                    MachineId = machines[machineName],
                                    OrderNumber = orderNumber,
                                    DetailName = detailName,
                                    StandardDescription = standardDesc ?? "",
                                    ImagePath = "",
                                    IsActive = true,
                                    CreatedAt = DateTime.Now
                                };

                                _context.ChecklistItems.Add(item);
                                imported++;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Baris {row}: {ex.Message}");
                            }
                        }

                        await _context.SaveChangesAsync();

                        var message = $"Berhasil import {imported} item.";
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
                var worksheet = package.Workbook.Worksheets.Add("Template Checklist");

                // Header
                worksheet.Cells[1, 1].Value = "No";
                worksheet.Cells[1, 2].Value = "Mesin";
                worksheet.Cells[1, 3].Value = "Urutan";
                worksheet.Cells[1, 4].Value = "Bagian (Detail)";
                worksheet.Cells[1, 5].Value = "Standar";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(16, 185, 129));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Sample data
                worksheet.Cells[2, 1].Value = 1;
                worksheet.Cells[2, 2].Value = "MESIN INJECTION";
                worksheet.Cells[2, 3].Value = 1;
                worksheet.Cells[2, 4].Value = "Emergency Stop";
                worksheet.Cells[2, 5].Value = "Harus berfungsi dengan baik";

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template_Checklist.xlsx");
            }
        }
    }
}
