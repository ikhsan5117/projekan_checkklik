using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using AMRVI.Services; 
using OfficeOpenXml;

namespace AMRVI.Controllers
{
    public class ChecklistDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly PlantService _plantService;

        public ChecklistDataController(ApplicationDbContext context, IWebHostEnvironment environment, PlantService plantService)
        {
            _context = context;
            _environment = environment;
            _plantService = plantService;
        }

        public async Task<IActionResult> Index()
        {
            var plant = _plantService.GetPlantName();
            object? machines = null;

            switch(plant)
            {
                case "BTR":
                    machines = await _context.Machines_BTR.Where(m => m.IsActive).OrderBy(m => m.Name)
                        .Select(m => new Machine { Id = m.Id, Name = m.Name }).ToListAsync();
                    break;
                case "HOSE":
                     machines = await _context.Machines_HOSE.Where(m => m.IsActive).OrderBy(m => m.Name)
                        .Select(m => new Machine { Id = m.Id, Name = m.Name }).ToListAsync();
                    break;
                case "MOLDED":
                     machines = await _context.Machines_MOLDED.Where(m => m.IsActive).OrderBy(m => m.Name)
                        .Select(m => new Machine { Id = m.Id, Name = m.Name }).ToListAsync();
                    break;
                case "MIXING":
                     machines = await _context.Machines_MIXING.Where(m => m.IsActive).OrderBy(m => m.Name)
                        .Select(m => new Machine { Id = m.Id, Name = m.Name }).ToListAsync();
                    break;
                default:
                     machines = await _context.Machines.Where(m => m.IsActive).OrderBy(m => m.Name).ToListAsync();
                    break;
            }

            return View(machines);
        }

        public IActionResult ImportVelasto()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetChecklistItems(int? machineId)
        {
            var plant = _plantService.GetPlantName();

            switch(plant)
            {
                case "BTR":
                    var qBTR = _context.ChecklistItems_BTR.Include(ci => ci.Machine).AsQueryable();
                    if (machineId.HasValue && machineId > 0) qBTR = qBTR.Where(ci => ci.MachineId == machineId);
                    var itemsBTR = await qBTR.Where(ci => ci.IsActive).OrderBy(ci => ci.Machine.Name).ThenBy(ci => ci.OrderNumber)
                        .Select(ci => new { ci.Id, ci.DetailName, ci.StandardDescription, ci.OrderNumber, ci.ImagePath, MachineName = ci.Machine.Name }).ToListAsync();
                    return Json(new { success = true, data = itemsBTR });

                case "HOSE":
                    var qHOSE = _context.ChecklistItems_HOSE.Include(ci => ci.Machine).AsQueryable();
                    if (machineId.HasValue && machineId > 0) qHOSE = qHOSE.Where(ci => ci.MachineId == machineId);
                    var itemsHOSE = await qHOSE.Where(ci => ci.IsActive).OrderBy(ci => ci.Machine.Name).ThenBy(ci => ci.OrderNumber)
                        .Select(ci => new { ci.Id, ci.DetailName, ci.StandardDescription, ci.OrderNumber, ci.ImagePath, MachineName = ci.Machine.Name }).ToListAsync();
                    return Json(new { success = true, data = itemsHOSE });

                case "MOLDED":
                    var qMOLDED = _context.ChecklistItems_MOLDED.Include(ci => ci.Machine).AsQueryable();
                    if (machineId.HasValue && machineId > 0) qMOLDED = qMOLDED.Where(ci => ci.MachineId == machineId);
                    var itemsMOLDED = await qMOLDED.Where(ci => ci.IsActive).OrderBy(ci => ci.Machine.Name).ThenBy(ci => ci.OrderNumber)
                        .Select(ci => new { ci.Id, ci.DetailName, ci.StandardDescription, ci.OrderNumber, ci.ImagePath, MachineName = ci.Machine.Name }).ToListAsync();
                    return Json(new { success = true, data = itemsMOLDED });

                case "MIXING":
                    var qMIXING = _context.ChecklistItems_MIXING.Include(ci => ci.Machine).AsQueryable();
                    if (machineId.HasValue && machineId > 0) qMIXING = qMIXING.Where(ci => ci.MachineId == machineId);
                    var itemsMIXING = await qMIXING.Where(ci => ci.IsActive).OrderBy(ci => ci.Machine.Name).ThenBy(ci => ci.OrderNumber)
                        .Select(ci => new { ci.Id, ci.DetailName, ci.StandardDescription, ci.OrderNumber, ci.ImagePath, MachineName = ci.Machine.Name }).ToListAsync();
                    return Json(new { success = true, data = itemsMIXING });

                default:
                    var qRVI = _context.ChecklistItems.Include(ci => ci.Machine).AsQueryable();
                    if (machineId.HasValue && machineId > 0) qRVI = qRVI.Where(ci => ci.MachineId == machineId);
                    var itemsRVI = await qRVI.Where(ci => ci.IsActive).OrderBy(ci => ci.Machine.Name).ThenBy(ci => ci.OrderNumber)
                        .Select(ci => new { ci.Id, ci.DetailName, ci.StandardDescription, ci.OrderNumber, ci.ImagePath, MachineName = ci.Machine.Name }).ToListAsync();
                    return Json(new { success = true, data = itemsRVI });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddChecklistItem(int machineId, string detailName, string standardDescription, int orderNumber, IFormFile? imageFile)
        {
            try
            {
                var plant = _plantService.GetPlantName();
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

                switch (plant)
                {
                     case "BTR":
                        _context.ChecklistItems_BTR.Add(new ChecklistItem_BTR { MachineId = machineId, DetailName = detailName, StandardDescription = standardDescription, OrderNumber = orderNumber, ImagePath = imagePath, IsActive = true, CreatedAt = DateTime.Now });
                        break;
                    case "HOSE":
                         _context.ChecklistItems_HOSE.Add(new ChecklistItem_HOSE { MachineId = machineId, DetailName = detailName, StandardDescription = standardDescription, OrderNumber = orderNumber, ImagePath = imagePath, IsActive = true, CreatedAt = DateTime.Now });
                        break;
                    case "MOLDED":
                        _context.ChecklistItems_MOLDED.Add(new ChecklistItem_MOLDED { MachineId = machineId, DetailName = detailName, StandardDescription = standardDescription, OrderNumber = orderNumber, ImagePath = imagePath, IsActive = true, CreatedAt = DateTime.Now });
                        break;
                    case "MIXING":
                        _context.ChecklistItems_MIXING.Add(new ChecklistItem_MIXING { MachineId = machineId, DetailName = detailName, StandardDescription = standardDescription, OrderNumber = orderNumber, ImagePath = imagePath, IsActive = true, CreatedAt = DateTime.Now });
                        break;
                    default:
                        _context.ChecklistItems.Add(new ChecklistItem { MachineId = machineId, DetailName = detailName, StandardDescription = standardDescription, OrderNumber = orderNumber, ImagePath = imagePath, IsActive = true, CreatedAt = DateTime.Now });
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
        public async Task<IActionResult> DeleteChecklistItem(int id)
        {
            var plant = _plantService.GetPlantName();
            bool found = false;

            switch (plant)
            {
                case "BTR":
                    var iBTR = await _context.ChecklistItems_BTR.FindAsync(id);
                    if(iBTR != null) { _context.ChecklistItems_BTR.Remove(iBTR); found = true; }
                    break;
                case "HOSE":
                    var iHOSE = await _context.ChecklistItems_HOSE.FindAsync(id);
                    if(iHOSE != null) { _context.ChecklistItems_HOSE.Remove(iHOSE); found = true; }
                    break;
                case "MOLDED":
                    var iMOLDED = await _context.ChecklistItems_MOLDED.FindAsync(id);
                    if(iMOLDED != null) { _context.ChecklistItems_MOLDED.Remove(iMOLDED); found = true; }
                    break;
                case "MIXING":
                     var iMIXING = await _context.ChecklistItems_MIXING.FindAsync(id);
                    if(iMIXING != null) { _context.ChecklistItems_MIXING.Remove(iMIXING); found = true; }
                    break;
                default:
                    var iRVI = await _context.ChecklistItems.FindAsync(id);
                    if(iRVI != null) { _context.ChecklistItems.Remove(iRVI); found = true; }
                    break;
            }

            if (!found) return Json(new { success = false, message = "Not found" });

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetChecklistItem(int id)
        {
             var plant = _plantService.GetPlantName();
             object? item = null;
             
             switch(plant)
             {
                 case "BTR":
                    item = await _context.ChecklistItems_BTR.Where(c=>c.Id==id)
                        .Select(c => new { c.Id, c.MachineId, c.DetailName, c.StandardDescription, c.OrderNumber, c.ImagePath }).FirstOrDefaultAsync();
                    break;
                 case "HOSE":
                    item = await _context.ChecklistItems_HOSE.Where(c=>c.Id==id)
                        .Select(c => new { c.Id, c.MachineId, c.DetailName, c.StandardDescription, c.OrderNumber, c.ImagePath }).FirstOrDefaultAsync();
                    break;
                 case "MOLDED":
                    item = await _context.ChecklistItems_MOLDED.Where(c=>c.Id==id)
                        .Select(c => new { c.Id, c.MachineId, c.DetailName, c.StandardDescription, c.OrderNumber, c.ImagePath }).FirstOrDefaultAsync();
                    break;
                 case "MIXING":
                    item = await _context.ChecklistItems_MIXING.Where(c=>c.Id==id)
                        .Select(c => new { c.Id, c.MachineId, c.DetailName, c.StandardDescription, c.OrderNumber, c.ImagePath }).FirstOrDefaultAsync();
                    break;
                 default:
                    item = await _context.ChecklistItems.Where(c=>c.Id==id)
                        .Select(c => new { c.Id, c.MachineId, c.DetailName, c.StandardDescription, c.OrderNumber, c.ImagePath }).FirstOrDefaultAsync();
                    break;
             }

            if (item == null) return Json(new { success = false, message = "Not found" });
            return Json(new { success = true, data = item });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateChecklistItem(int id, int machineId, string detailName, string standardDescription, int orderNumber, IFormFile? imageFile, bool keepExistingImage = true)
        {
            try
            {
                var plant = _plantService.GetPlantName();
                bool found = false;

                // Helper to handle image logic
                async Task<string?> HandleImage(string? existingPath) 
                {
                     if (imageFile != null && imageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingPath))
                        {
                            var oldPath = Path.Combine(_environment.WebRootPath, existingPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "checklist");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                        var uName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                        var fPath = Path.Combine(uploadsFolder, uName);
                        using (var stream = new FileStream(fPath, FileMode.Create)) await imageFile.CopyToAsync(stream);
                        return $"/uploads/checklist/{uName}";
                    }
                    return existingPath;
                }

                switch (plant)
                {
                    case "BTR":
                        var iBTR = await _context.ChecklistItems_BTR.FindAsync(id);
                        if(iBTR != null) {
                            iBTR.MachineId = machineId; iBTR.DetailName = detailName; iBTR.StandardDescription = standardDescription; iBTR.OrderNumber = orderNumber;
                            iBTR.ImagePath = await HandleImage(iBTR.ImagePath);
                            found = true;
                        }
                        break;
                    case "HOSE":
                         var iHOSE = await _context.ChecklistItems_HOSE.FindAsync(id);
                        if(iHOSE != null) {
                            iHOSE.MachineId = machineId; iHOSE.DetailName = detailName; iHOSE.StandardDescription = standardDescription; iHOSE.OrderNumber = orderNumber;
                            iHOSE.ImagePath = await HandleImage(iHOSE.ImagePath);
                            found = true;
                        }
                        break;
                     case "MOLDED":
                        var iMOLDED = await _context.ChecklistItems_MOLDED.FindAsync(id);
                        if(iMOLDED != null) {
                            iMOLDED.MachineId = machineId; iMOLDED.DetailName = detailName; iMOLDED.StandardDescription = standardDescription; iMOLDED.OrderNumber = orderNumber;
                            iMOLDED.ImagePath = await HandleImage(iMOLDED.ImagePath);
                            found = true;
                        }
                        break;
                     case "MIXING":
                        var iMIXING = await _context.ChecklistItems_MIXING.FindAsync(id);
                        if(iMIXING != null) {
                            iMIXING.MachineId = machineId; iMIXING.DetailName = detailName; iMIXING.StandardDescription = standardDescription; iMIXING.OrderNumber = orderNumber;
                            iMIXING.ImagePath = await HandleImage(iMIXING.ImagePath);
                            found = true;
                        }
                        break;
                    default:
                        var iRVI = await _context.ChecklistItems.FindAsync(id);
                        if(iRVI != null) {
                            iRVI.MachineId = machineId; iRVI.DetailName = detailName; iRVI.StandardDescription = standardDescription; iRVI.OrderNumber = orderNumber;
                            iRVI.ImagePath = await HandleImage(iRVI.ImagePath);
                            found = true;
                        }
                        break;
                }

                if (!found) return Json(new { success = false, message = "Item not found" });

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
             var plant = _plantService.GetPlantName();
             ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
             
             List<ChecklistItemExcelDto> data = new List<ChecklistItemExcelDto>();
             
             switch(plant)
             {
                 case "BTR":
                    var qBTR = _context.ChecklistItems_BTR.Include(c=>c.Machine).Where(c=>c.IsActive).AsQueryable();
                    if(machineId.HasValue && machineId>0) qBTR = qBTR.Where(c=>c.MachineId == machineId);
                    data = await qBTR.OrderBy(c=>c.Machine.Name).ThenBy(c=>c.OrderNumber)
                        .Select(c => new ChecklistItemExcelDto { Machine = c.Machine.Name, Order = c.OrderNumber, Detail = c.DetailName, Std = c.StandardDescription, Img = c.ImagePath }).ToListAsync();
                    break;
                 case "HOSE":
                     var qHOSE = _context.ChecklistItems_HOSE.Include(c=>c.Machine).Where(c=>c.IsActive).AsQueryable();
                    if(machineId.HasValue && machineId>0) qHOSE = qHOSE.Where(c=>c.MachineId == machineId);
                    data = await qHOSE.OrderBy(c=>c.Machine.Name).ThenBy(c=>c.OrderNumber)
                        .Select(c => new ChecklistItemExcelDto { Machine = c.Machine.Name, Order = c.OrderNumber, Detail = c.DetailName, Std = c.StandardDescription, Img = c.ImagePath }).ToListAsync();
                     break;
                 case "MOLDED":
                     var qMOLDED = _context.ChecklistItems_MOLDED.Include(c=>c.Machine).Where(c=>c.IsActive).AsQueryable();
                    if(machineId.HasValue && machineId>0) qMOLDED = qMOLDED.Where(c=>c.MachineId == machineId);
                    data = await qMOLDED.OrderBy(c=>c.Machine.Name).ThenBy(c=>c.OrderNumber)
                        .Select(c => new ChecklistItemExcelDto { Machine = c.Machine.Name, Order = c.OrderNumber, Detail = c.DetailName, Std = c.StandardDescription, Img = c.ImagePath }).ToListAsync();
                     break;
                 case "MIXING":
                     var qMIXING = _context.ChecklistItems_MIXING.Include(c=>c.Machine).Where(c=>c.IsActive).AsQueryable();
                    if(machineId.HasValue && machineId>0) qMIXING = qMIXING.Where(c=>c.MachineId == machineId);
                    data = await qMIXING.OrderBy(c=>c.Machine.Name).ThenBy(c=>c.OrderNumber)
                        .Select(c => new ChecklistItemExcelDto { Machine = c.Machine.Name, Order = c.OrderNumber, Detail = c.DetailName, Std = c.StandardDescription, Img = c.ImagePath }).ToListAsync();
                     break;
                 default:
                    var qRVI = _context.ChecklistItems.Include(c=>c.Machine).Where(c=>c.IsActive).AsQueryable();
                    if(machineId.HasValue && machineId>0) qRVI = qRVI.Where(c=>c.MachineId == machineId);
                    data = await qRVI.OrderBy(c=>c.Machine.Name).ThenBy(c=>c.OrderNumber)
                        .Select(c => new ChecklistItemExcelDto { Machine = c.Machine.Name, Order = c.OrderNumber, Detail = c.DetailName, Std = c.StandardDescription, Img = c.ImagePath }).ToListAsync();
                    break;
             }

             using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"Checklist {plant}");
                worksheet.Cells[1, 1].Value = "No";
                worksheet.Cells[1, 2].Value = "Mesin";
                worksheet.Cells[1, 3].Value = "Urutan";
                worksheet.Cells[1, 4].Value = "Bagian (Detail)";
                worksheet.Cells[1, 5].Value = "Standar";
                worksheet.Cells[1, 6].Value = "Path Gambar";
                
                using (var range = worksheet.Cells[1, 1, 1, 6]) { range.Style.Font.Bold = true; range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(16, 185, 129)); range.Style.Font.Color.SetColor(System.Drawing.Color.White); }

                int row = 2;
                foreach (var x in data)
                {
                    worksheet.Cells[row, 1].Value = row - 1;
                    worksheet.Cells[row, 2].Value = x.Machine;
                    worksheet.Cells[row, 3].Value = x.Order;
                    worksheet.Cells[row, 4].Value = x.Detail;
                    worksheet.Cells[row, 5].Value = x.Std;
                    worksheet.Cells[row, 6].Value = x.Img;
                    row++;
                }
                worksheet.Cells.AutoFitColumns();
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Checklist_{plant}.xlsx");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0) return Json(new { success = false, message = "File tidak valid" });
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var plant = _plantService.GetPlantName();

            try {
                using (var stream = new MemoryStream()) {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream)) {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;
                        int imported = 0;
                        List<string> errors = new List<string>();

                        Dictionary<string, int> machines = new Dictionary<string, int>();
                        switch(plant) {
                            case "BTR": machines = await _context.Machines_BTR.ToDictionaryAsync(m => m.Name, m => m.Id); break;
                            case "HOSE": machines = await _context.Machines_HOSE.ToDictionaryAsync(m => m.Name, m => m.Id); break;
                            case "MOLDED": machines = await _context.Machines_MOLDED.ToDictionaryAsync(m => m.Name, m => m.Id); break;
                            case "MIXING": machines = await _context.Machines_MIXING.ToDictionaryAsync(m => m.Name, m => m.Id); break;
                            default: machines = await _context.Machines.ToDictionaryAsync(m => m.Name, m => m.Id); break;
                        }

                        for (int row = 2; row <= rowCount; row++) {
                             try {
                                var machineName = worksheet.Cells[row, 2].Value?.ToString();
                                var orderNumberStr = worksheet.Cells[row, 3].Value?.ToString();
                                var detailName = worksheet.Cells[row, 4].Value?.ToString();
                                var standardDesc = worksheet.Cells[row, 5].Value?.ToString();
                                
                                if (string.IsNullOrWhiteSpace(machineName) || string.IsNullOrWhiteSpace(detailName)) continue;
                                if (!machines.ContainsKey(machineName)) { errors.Add($"Baris {row}: Mesin '{machineName}' tidak ditemukan di plant {plant}"); continue; }
                                if (!int.TryParse(orderNumberStr, out int orderNumber)) orderNumber = 0;

                                switch(plant) {
                                    case "BTR": _context.ChecklistItems_BTR.Add(new ChecklistItem_BTR { MachineId = machines[machineName], OrderNumber = orderNumber, DetailName = detailName, StandardDescription = standardDesc??"", IsActive = true, CreatedAt = DateTime.Now }); break;
                                    case "HOSE": _context.ChecklistItems_HOSE.Add(new ChecklistItem_HOSE { MachineId = machines[machineName], OrderNumber = orderNumber, DetailName = detailName, StandardDescription = standardDesc??"", IsActive = true, CreatedAt = DateTime.Now }); break;
                                    case "MOLDED": _context.ChecklistItems_MOLDED.Add(new ChecklistItem_MOLDED { MachineId = machines[machineName], OrderNumber = orderNumber, DetailName = detailName, StandardDescription = standardDesc??"", IsActive = true, CreatedAt = DateTime.Now }); break;
                                    case "MIXING": _context.ChecklistItems_MIXING.Add(new ChecklistItem_MIXING { MachineId = machines[machineName], OrderNumber = orderNumber, DetailName = detailName, StandardDescription = standardDesc??"", IsActive = true, CreatedAt = DateTime.Now }); break;
                                    default: _context.ChecklistItems.Add(new ChecklistItem { MachineId = machines[machineName], OrderNumber = orderNumber, DetailName = detailName, StandardDescription = standardDesc??"", IsActive = true, CreatedAt = DateTime.Now }); break;
                                }
                                imported++;
                            } catch(Exception ex) { errors.Add($"Baris {row}: {ex.Message}"); }
                        }
                        await _context.SaveChangesAsync();
                        var message = $"Berhasil import {imported} item.";
                        if (errors.Any()) message += $" {errors.Count} error: " + string.Join("; ", errors.Take(5));
                        return Json(new { success = true, message = message, imported = imported, errors = errors.Count });
                    }
                }
            } catch (Exception ex) { return Json(new { success = false, message = "Error: " + ex.Message }); }
        }

        [HttpPost]
        public async Task<IActionResult> SmartImportVelasto(IFormFile file)
        {
            if (file == null || file.Length == 0) return Json(new { success = false, message = "File tidak valid" });
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var plant = _plantService.GetPlantName();

            try {
                using (var stream = new MemoryStream()) {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream)) {
                        int totalSheets = package.Workbook.Worksheets.Count;
                        int machinesCreated = 0;
                        int unitsCreated = 0;
                        int itemsImported = 0;

                        foreach (var worksheet in package.Workbook.Worksheets) {
                            string unitNumberStr = "";
                            string machineName = "";

                            // 1. AGGRESSIVE SEARCH: Scan Rows 1-10, Columns 1-30
                            for (int r = 1; r <= 15; r++) {
                                for (int col = 1; col <= 30; col++) {
                                    var cellValue = worksheet.Cells[r, col].Value?.ToString()?.Trim() ?? "";
                                    if (string.IsNullOrEmpty(cellValue)) continue;

                                    // Search for Unit Number (PV pattern)
                                    if (cellValue.Contains("No. Mesin", StringComparison.OrdinalIgnoreCase) || 
                                        cellValue.Contains("No Mesin", StringComparison.OrdinalIgnoreCase) ||
                                        cellValue.Equals("No.", StringComparison.OrdinalIgnoreCase)) {
                                        
                                        // Check if value is in same cell (e.g., "No. Mesin: PV 014")
                                        if (cellValue.Contains(":")) {
                                            var split = cellValue.Split(':');
                                            if (split.Length > 1 && split[1].Trim().Length >= 3) {
                                                unitNumberStr = split[1].Trim();
                                            }
                                        }
                                        
                                        // If still empty, check neighbors (merged cells handle)
                                        if (string.IsNullOrEmpty(unitNumberStr)) {
                                            for(int next = 1; next <= 5; next++) {
                                                var val = worksheet.Cells[r, col + next].Value?.ToString()?.Trim();
                                                if (!string.IsNullOrEmpty(val) && val.Length >= 3) { unitNumberStr = val; break; }
                                            }
                                        }
                                    }

                                    // Search for Machine Name
                                    if (cellValue.Contains("Nama Mesin", StringComparison.OrdinalIgnoreCase)) {
                                        if (cellValue.Contains(":")) {
                                            var split = cellValue.Split(':');
                                            if (split.Length > 1 && split[1].Trim().Length >= 3) {
                                                machineName = split[1].Trim();
                                            }
                                        }
                                        if (string.IsNullOrEmpty(machineName)) {
                                            for(int next = 1; next <= 8; next++) {
                                                var val = worksheet.Cells[r, col + next].Value?.ToString()?.Trim();
                                                if (!string.IsNullOrEmpty(val) && val.Length >= 3) { machineName = val; break; }
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(unitNumberStr) && !string.IsNullOrEmpty(machineName)) break;
                            }

                            // 2. BACKUP PLAN: If still empty, use Worksheet Name for Unit Number
                            if (string.IsNullOrEmpty(unitNumberStr)) {
                                var wsName = worksheet.Name;
                                // Regex to find PV followed by numbers (e.g., PV 013 or PV013)
                                var match = System.Text.RegularExpressions.Regex.Match(wsName, @"PV\s?\d+", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                if (match.Success) unitNumberStr = match.Value.ToUpper();
                            }

                            // 4. CLEANING & REFINING
                            if (!string.IsNullOrEmpty(machineName)) {
                                if (machineName.Contains("/")) machineName = machineName.Split('/').First().Trim();
                                if (machineName.Contains(":")) machineName = machineName.Split(':').Last().Trim();
                                machineName = machineName.Replace("Nama Mesin", "", StringComparison.OrdinalIgnoreCase).Trim();
                            }

                            // VALIDATION: Reject numeric-only machine names or very short ones
                            if (string.IsNullOrEmpty(machineName) || int.TryParse(machineName, out _) || machineName.Length < 3) continue;
                            
                            if (!string.IsNullOrEmpty(unitNumberStr)) {
                                unitNumberStr = unitNumberStr.Replace("No. Mesin", "", StringComparison.OrdinalIgnoreCase)
                                                             .Replace("No Mesin", "", StringComparison.OrdinalIgnoreCase)
                                                             .Replace(":", "").Trim();
                            }

                            // If we STILL don't have metadata, we can't process this sheet
                            if (string.IsNullOrEmpty(machineName) || string.IsNullOrEmpty(unitNumberStr)) continue;

                            // 4. Ensure Machine Exists (Case Insensitive)
                            int machineId = 0;
                            var now = DateTime.Now;
                            string p = (plant ?? "MOLDED").ToUpper();

                            if (p == "BTR") {
                                var m = await _context.Machines_BTR.FirstOrDefaultAsync(m => m.Name.ToLower() == machineName.ToLower());
                                if (m == null) { m = new Machine_BTR { Name = machineName, IsActive = true, CreatedAt = now }; _context.Machines_BTR.Add(m); await _context.SaveChangesAsync(); machinesCreated++; }
                                machineId = m.Id;
                            } else if (p == "HOSE") {
                                var m = await _context.Machines_HOSE.FirstOrDefaultAsync(m => m.Name.ToLower() == machineName.ToLower());
                                if (m == null) { m = new Machine_HOSE { Name = machineName, IsActive = true, CreatedAt = now }; _context.Machines_HOSE.Add(m); await _context.SaveChangesAsync(); machinesCreated++; }
                                machineId = m.Id;
                            } else if (p == "MOLDED") {
                                var m = await _context.Machines_MOLDED.FirstOrDefaultAsync(m => m.Name.ToLower() == machineName.ToLower());
                                if (m == null) { m = new Machine_MOLDED { Name = machineName, IsActive = true, CreatedAt = now }; _context.Machines_MOLDED.Add(m); await _context.SaveChangesAsync(); machinesCreated++; }
                                machineId = m.Id;
                            } else if (p == "MIXING") {
                                var m = await _context.Machines_MIXING.FirstOrDefaultAsync(m => m.Name.ToLower() == machineName.ToLower());
                                if (m == null) { m = new Machine_MIXING { Name = machineName, IsActive = true, CreatedAt = now }; _context.Machines_MIXING.Add(m); await _context.SaveChangesAsync(); machinesCreated++; }
                                machineId = m.Id;
                            } else {
                                var m = await _context.Machines.FirstOrDefaultAsync(m => m.Name.ToLower() == machineName.ToLower());
                                if (m == null) { m = new Machine { Name = machineName, IsActive = true, CreatedAt = now }; _context.Machines.Add(m); await _context.SaveChangesAsync(); machinesCreated++; }
                                machineId = m.Id;
                            }

                            // 5. Ensure Unit Number Exists
                            bool unitExists = false;
                            if (p == "BTR") unitExists = await _context.MachineNumbers_BTR.AnyAsync(n => n.Number.ToLower() == unitNumberStr.ToLower() && n.MachineId == machineId);
                            else if (p == "HOSE") unitExists = await _context.MachineNumbers_HOSE.AnyAsync(n => n.Number.ToLower() == unitNumberStr.ToLower() && n.MachineId == machineId);
                            else if (p == "MOLDED") unitExists = await _context.MachineNumbers_MOLDED.AnyAsync(n => n.Number.ToLower() == unitNumberStr.ToLower() && n.MachineId == machineId);
                            else if (p == "MIXING") unitExists = await _context.MachineNumbers_MIXING.AnyAsync(n => n.Number.ToLower() == unitNumberStr.ToLower() && n.MachineId == machineId);
                            else unitExists = await _context.MachineNumbers.AnyAsync(n => n.Number.ToLower() == unitNumberStr.ToLower() && n.MachineId == machineId);

                            if (!unitExists) {
                                if (p == "BTR") _context.MachineNumbers_BTR.Add(new MachineNumber_BTR { MachineId = machineId, Number = unitNumberStr, Location = "Imported", IsActive = true, CreatedAt = now });
                                else if (p == "HOSE") _context.MachineNumbers_HOSE.Add(new MachineNumber_HOSE { MachineId = machineId, Number = unitNumberStr, Location = "Imported", IsActive = true, CreatedAt = now });
                                else if (p == "MOLDED") _context.MachineNumbers_MOLDED.Add(new MachineNumber_MOLDED { MachineId = machineId, Number = unitNumberStr, Location = "Imported", IsActive = true, CreatedAt = now });
                                else if (p == "MIXING") _context.MachineNumbers_MIXING.Add(new MachineNumber_MIXING { MachineId = machineId, Number = unitNumberStr, Location = "Imported", IsActive = true, CreatedAt = now });
                                else _context.MachineNumbers.Add(new MachineNumber { MachineId = machineId, Number = unitNumberStr, Location = "Imported", IsActive = true, CreatedAt = now });
                                unitsCreated++;
                            }
                            await _context.SaveChangesAsync();

                            // 4. Parse Checklist Items (Table starting from Row 7-8)
                            // Col B: Komponen (DetailName)
                            // Col C: Standar (StandardDescription)
                            string lastDetailName = string.Empty;
                            int rowCount = worksheet.Dimension?.Rows ?? 0;
                            
                            for (int row = 7; row <= rowCount; row++) {
                                var currentNo = worksheet.Cells[row, 1].Text?.Trim();
                                var detailName = worksheet.Cells[row, 2].Text?.Trim();
                                var stdDesc = worksheet.Cells[row, 3].Text?.Trim();

                                // If detail name is empty, it usually inherits from the row above (like Hydraulic sub-items)
                                if (!string.IsNullOrEmpty(detailName)) lastDetailName = detailName;
                                if (string.IsNullOrEmpty(lastDetailName) || string.IsNullOrEmpty(stdDesc)) continue;

                                // Check if already exists to avoid duplication in one machine
                                bool exists = false;
                                switch(plant) {
                                    case "BTR": exists = await _context.ChecklistItems_BTR.AnyAsync(c => c.MachineId == machineId && c.DetailName == lastDetailName && c.StandardDescription == stdDesc); break;
                                    case "HOSE": exists = await _context.ChecklistItems_HOSE.AnyAsync(c => c.MachineId == machineId && c.DetailName == lastDetailName && c.StandardDescription == stdDesc); break;
                                    case "MOLDED": exists = await _context.ChecklistItems_MOLDED.AnyAsync(c => c.MachineId == machineId && c.DetailName == lastDetailName && c.StandardDescription == stdDesc); break;
                                    case "MIXING": exists = await _context.ChecklistItems_MIXING.AnyAsync(c => c.MachineId == machineId && c.DetailName == lastDetailName && c.StandardDescription == stdDesc); break;
                                    default: exists = await _context.ChecklistItems.AnyAsync(c => c.MachineId == machineId && c.DetailName == lastDetailName && c.StandardDescription == stdDesc); break;
                                }

                                if (!exists) {
                                    int order = 0; int.TryParse(currentNo, out order);
                                    if(order == 0) order = itemsImported + 1;

                                    switch(plant) {
                                        case "BTR": _context.ChecklistItems_BTR.Add(new ChecklistItem_BTR { MachineId = machineId, OrderNumber = order, DetailName = lastDetailName, StandardDescription = stdDesc, CreatedAt = now, IsActive = true }); break;
                                        case "HOSE": _context.ChecklistItems_HOSE.Add(new ChecklistItem_HOSE { MachineId = machineId, OrderNumber = order, DetailName = lastDetailName, StandardDescription = stdDesc, CreatedAt = now, IsActive = true }); break;
                                        case "MOLDED": _context.ChecklistItems_MOLDED.Add(new ChecklistItem_MOLDED { MachineId = machineId, OrderNumber = order, DetailName = lastDetailName, StandardDescription = stdDesc, CreatedAt = now, IsActive = true }); break;
                                        case "MIXING": _context.ChecklistItems_MIXING.Add(new ChecklistItem_MIXING { MachineId = machineId, OrderNumber = order, DetailName = lastDetailName, StandardDescription = stdDesc, CreatedAt = now, IsActive = true }); break;
                                        default: _context.ChecklistItems.Add(new ChecklistItem { MachineId = machineId, OrderNumber = order, DetailName = lastDetailName, StandardDescription = stdDesc, CreatedAt = now, IsActive = true }); break;
                                    }
                                    itemsImported++;
                                }
                            }
                            await _context.SaveChangesAsync();
                        }

                        return Json(new { success = true, message = $"Beres pak! Berhasil memproses {totalSheets} sheet. Menambahkan {machinesCreated} tipe mesin, {unitsCreated} nomor unit, dan {itemsImported} item checklist." });
                    }
                }
            } catch (Exception ex) { return Json(new { success = false, message = "Error: " + ex.Message }); }
        }

        [HttpGet]
        public IActionResult DownloadTemplate()
        {
              ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
             using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Template Checklist");
                worksheet.Cells[1, 1].Value = "No";
                worksheet.Cells[1, 2].Value = "Mesin";
                worksheet.Cells[1, 3].Value = "Urutan";
                worksheet.Cells[1, 4].Value = "Bagian (Detail)";
                worksheet.Cells[1, 5].Value = "Standar";
                using (var range = worksheet.Cells[1, 1, 1, 5]) { range.Style.Font.Bold = true; }
                 worksheet.Cells[2, 1].Value = 1;
                worksheet.Cells[2, 2].Value = "NAMA MESIN DISINI";
                worksheet.Cells[2, 3].Value = 1;
                worksheet.Cells[2, 4].Value = "Cek Bagian A";
                worksheet.Cells[2, 5].Value = "Standar OK";
                 var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template_Checklist.xlsx");
            }
        }
    }

    public class ChecklistItemExcelDto
    {
        public string? Machine { get; set; }
        public int Order { get; set; }
        public string? Detail { get; set; }
        public string? Std { get; set; }
        public string? Img { get; set; }
    }
}
