using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using AMRVI.Services;
using System.Linq;
using OfficeOpenXml;
using System.Drawing;
using Microsoft.AspNetCore.SignalR;

namespace AMRVI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class HenkatenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PlantService _plantService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<AMRVI.Hubs.NotificationHub> _hubContext;

        public HenkatenController(
            ApplicationDbContext context,
            PlantService plantService,
            IWebHostEnvironment environment,
            IHubContext<AMRVI.Hubs.NotificationHub> hubContext)
        {
            _context = context;
            _plantService = plantService;
            _environment = environment;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Henkaten Problem Management";
            ViewBag.CurrentPlant = _plantService.GetPlantName();
            return View();
        }

        [HttpGet("Analytics")]
        public IActionResult Analytics()
        {
            ViewData["Title"] = "Analytics Henkaten";
            ViewBag.CurrentPlant = _plantService.GetPlantName();
            return View();
        }

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData()
        {
            try
            {
                var plantId = _plantService.GetPlantId();
                var problems = await _context.HenkatenProblems
                    .Where(h => h.PlantId == plantId)
                    .OrderByDescending(h => h.TanggalUpdate)
                    .Select(h => new
                    {
                        h.Id,
                        TanggalUpdate = h.TanggalUpdate != null ? h.TanggalUpdate.ToString("dd/MM/yyyy") : "",
                        h.Shift,
                        h.PicLeader,
                        h.Department,
                        h.NamaAreaLine,
                        h.NamaOperator,
                        h.Jenis4M,
                        h.Standard4M,
                        h.Actual4M,
                        h.KeteranganProblem,
                        h.TemporaryAction,
                        h.RencanaPerbaikan,
                        TanggalRencanaPerbaikan = h.TanggalRencanaPerbaikan != null 
                            ? h.TanggalRencanaPerbaikan.ToString("dd/MM/yyyy") 
                            : "",
                        h.FotoTemuan,
                        h.AktualPerbaikan,
                        TanggalAktualPerbaikan = h.TanggalAktualPerbaikan.HasValue 
                            ? h.TanggalAktualPerbaikan.Value.ToString("dd/MM/yyyy") 
                            : null,
                        h.FotoAktual,
                        h.Status
                    })
                    .ToListAsync();

                return Ok(problems);
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"Error in GetData: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] HenkatenProblem model, IFormFile? fotoTemuan, IFormFile? fotoAktual)
        {
            try
            {
                // Validate model is not null
                if (model == null)
                {
                    Console.WriteLine("Model is null");
                    return BadRequest(new { error = "Data tidak valid - model null" });
                }

                // Set Id to 0 for create operation (new record)
                model.Id = 0;
                ModelState.Remove("Id"); // Remove from ModelState to avoid validation errors

                // Remove Plant navigation property from ModelState (we only need PlantId)
                ModelState.Remove("Plant");

                // Log received data for debugging
                Console.WriteLine($"Received model - Id: {model.Id}, TanggalUpdate: {model.TanggalUpdate}, Shift: {model.Shift}, PicLeader: {model.PicLeader}");
                Console.WriteLine($"NamaAreaLine: {model.NamaAreaLine}, NamaOperator: {model.NamaOperator}, Jenis4M: {model.Jenis4M}");
                Console.WriteLine($"KeteranganProblem: {model.KeteranganProblem}, RencanaPerbaikan: {model.RencanaPerbaikan}");
                Console.WriteLine($"TanggalRencanaPerbaikan: {model.TanggalRencanaPerbaikan}");

                // Log ModelState for debugging
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState Keys:");
                    foreach (var key in ModelState.Keys)
                    {
                        Console.WriteLine($"  {key}: {string.Join(", ", ModelState[key].Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                // Validate ModelState (after removing Id and Plant)
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                        .ToList();
                    
                    Console.WriteLine("ModelState errors:");
                    foreach (var err in errors)
                    {
                        Console.WriteLine($"  {err.Field}: {string.Join(", ", err.Errors)}");
                    }
                    
                    return BadRequest(new { error = "Data tidak valid", details = errors });
                }

                // Check required fields manually
                if (model.TanggalUpdate == default(DateTime))
                {
                    return BadRequest(new { error = "Tanggal Update harus diisi" });
                }
                if (string.IsNullOrWhiteSpace(model.Shift))
                {
                    return BadRequest(new { error = "Shift harus diisi" });
                }
                if (string.IsNullOrWhiteSpace(model.PicLeader))
                {
                    return BadRequest(new { error = "PIC Leader harus diisi" });
                }
                if (string.IsNullOrWhiteSpace(model.NamaAreaLine))
                {
                    return BadRequest(new { error = "Nama Area/Line harus diisi" });
                }
                if (string.IsNullOrWhiteSpace(model.NamaOperator))
                {
                    return BadRequest(new { error = "Nama Operator harus diisi" });
                }
                if (string.IsNullOrWhiteSpace(model.Jenis4M))
                {
                    return BadRequest(new { error = "Jenis 4M harus diisi" });
                }
                if (string.IsNullOrWhiteSpace(model.KeteranganProblem))
                {
                    return BadRequest(new { error = "Keterangan Problem harus diisi" });
                }
                if (string.IsNullOrWhiteSpace(model.RencanaPerbaikan))
                {
                    return BadRequest(new { error = "Rencana Perbaikan harus diisi" });
                }
                if (model.TanggalRencanaPerbaikan == default(DateTime))
                {
                    return BadRequest(new { error = "Tanggal Rencana Perbaikan harus diisi" });
                }

                var plantId = _plantService.GetPlantId();
                model.PlantId = plantId;
                model.CreatedAt = DateTime.Now;
                model.Department = User.FindFirst("Department")?.Value ?? "Produksi"; // Default ke Produksi jika null

                // Handle Foto Temuan upload
                if (fotoTemuan != null && fotoTemuan.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "henkaten");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{fotoTemuan.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fotoTemuan.CopyToAsync(fileStream);
                    }

                    model.FotoTemuan = $"/uploads/henkaten/{uniqueFileName}";
                }

                // Handle Foto Aktual upload
                if (fotoAktual != null && fotoAktual.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "henkaten");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{fotoAktual.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fotoAktual.CopyToAsync(fileStream);
                    }

                    model.FotoAktual = $"/uploads/henkaten/{uniqueFileName}";
                }

                // Update status based on whether aktual perbaikan is filled
                if (!string.IsNullOrEmpty(model.AktualPerbaikan) && model.TanggalAktualPerbaikan.HasValue)
                {
                    model.Status = "Selesai";
                }
                else
                {
                    model.Status = "Pending";
                }

                _context.HenkatenProblems.Add(model);
                await _context.SaveChangesAsync();

                // Broadcast SignalR update
                var plantName = _plantService.GetPlantName();
                await _hubContext.Clients.All.SendAsync("HenkatenDataUpdated", plantName);

                return Ok(new { success = true, message = "Data berhasil disimpan" });
            }
            catch (DbUpdateException dbEx)
            {
                // Log database errors
                Console.WriteLine($"Database error in Create: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {dbEx.InnerException.Message}");
                }
                return StatusCode(500, new { error = "Gagal menyimpan ke database. Pastikan semua field terisi dengan benar.", details = dbEx.Message });
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"Error in Create: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { error = $"Terjadi kesalahan: {ex.Message}", details = ex.StackTrace });
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var problem = await _context.HenkatenProblems.FindAsync(id);
                if (problem == null)
                    return NotFound(new { error = "Data tidak ditemukan" });

                return Ok(problem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit([FromForm] HenkatenProblem model, IFormFile? fotoTemuan, IFormFile? fotoAktual)
        {
            try
            {
                var existing = await _context.HenkatenProblems.FindAsync(model.Id);
                if (existing == null)
                    return NotFound(new { error = "Data tidak ditemukan" });

                // Update fields
                existing.TanggalUpdate = model.TanggalUpdate;
                existing.Shift = model.Shift;
                existing.PicLeader = model.PicLeader;
                existing.NamaAreaLine = model.NamaAreaLine;
                existing.NamaOperator = model.NamaOperator;
                existing.Jenis4M = model.Jenis4M;
                existing.Standard4M = model.Standard4M;
                existing.Actual4M = model.Actual4M;
                existing.KeteranganProblem = model.KeteranganProblem;
                existing.TemporaryAction = model.TemporaryAction;
                existing.RencanaPerbaikan = model.RencanaPerbaikan;
                existing.TanggalRencanaPerbaikan = model.TanggalRencanaPerbaikan;
                existing.AktualPerbaikan = model.AktualPerbaikan;
                existing.TanggalAktualPerbaikan = model.TanggalAktualPerbaikan;
                existing.UpdatedAt = DateTime.Now;

                // Handle Foto Temuan upload
                if (fotoTemuan != null && fotoTemuan.Length > 0)
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(existing.FotoTemuan))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, existing.FotoTemuan.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "henkaten");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{fotoTemuan.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fotoTemuan.CopyToAsync(fileStream);
                    }

                    existing.FotoTemuan = $"/uploads/henkaten/{uniqueFileName}";
                }

                // Handle Foto Aktual upload
                if (fotoAktual != null && fotoAktual.Length > 0)
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(existing.FotoAktual))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, existing.FotoAktual.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "henkaten");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{fotoAktual.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fotoAktual.CopyToAsync(fileStream);
                    }

                    existing.FotoAktual = $"/uploads/henkaten/{uniqueFileName}";
                }

                // Update status based on whether aktual perbaikan is filled
                if (!string.IsNullOrEmpty(existing.AktualPerbaikan) && existing.TanggalAktualPerbaikan.HasValue)
                {
                    existing.Status = "Selesai";
                }
                else
                {
                    existing.Status = "Pending";
                }

                await _context.SaveChangesAsync();

                // Broadcast SignalR update
                var plantName = _plantService.GetPlantName();
                await _hubContext.Clients.All.SendAsync("HenkatenDataUpdated", plantName);

                return Ok(new { success = true, message = "Data berhasil diupdate" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var problem = await _context.HenkatenProblems.FindAsync(id);
                if (problem == null)
                    return NotFound(new { error = "Data tidak ditemukan" });

                // Delete associated files
                if (!string.IsNullOrEmpty(problem.FotoTemuan))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, problem.FotoTemuan.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                if (!string.IsNullOrEmpty(problem.FotoAktual))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, problem.FotoAktual.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _context.HenkatenProblems.Remove(problem);
                await _context.SaveChangesAsync();

                // Broadcast SignalR update
                var plantName = _plantService.GetPlantName();
                await _hubContext.Clients.All.SendAsync("HenkatenDataUpdated", plantName);

                return Ok(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var plantId = _plantService.GetPlantId();
                var plantName = _plantService.GetPlantName();
                
                var problems = await _context.HenkatenProblems
                    .Where(h => h.PlantId == plantId)
                    .OrderByDescending(h => h.TanggalUpdate)
                    .ToListAsync();

                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Data Henkaten");
                    
                    // Header Row - Set each header individually
                    sheet.Cells[1, 1].Value = "No";
                    sheet.Cells[1, 2].Value = "Tanggal Update";
                    sheet.Cells[1, 3].Value = "Shift";
                    sheet.Cells[1, 4].Value = "Department";
                    sheet.Cells[1, 5].Value = "PIC Leader";
                    sheet.Cells[1, 6].Value = "Area/Line";
                    sheet.Cells[1, 7].Value = "Operator";
                    sheet.Cells[1, 8].Value = "Jenis 4M";
                    sheet.Cells[1, 9].Value = "4M Standard";
                    sheet.Cells[1, 10].Value = "4M Actual";
                    sheet.Cells[1, 11].Value = "Keterangan Problem";
                    sheet.Cells[1, 12].Value = "Temporary Action";
                    sheet.Cells[1, 13].Value = "Rencana Perbaikan";
                    sheet.Cells[1, 14].Value = "Tanggal Rencana Perbaikan";
                    sheet.Cells[1, 15].Value = "Aktual Perbaikan";
                    sheet.Cells[1, 16].Value = "Tanggal Aktual Perbaikan";
                    sheet.Cells[1, 17].Value = "Status";

                    // Style header row
                    using (var range = sheet.Cells[1, 1, 1, 17])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Font.Size = 11;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 158, 11)); // Orange
                        range.Style.Font.Color.SetColor(Color.White);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    
                    // Set row height for header
                    sheet.Row(1).Height = 25;

                    // Data Rows
                    for (int i = 0; i < problems.Count; i++)
                    {
                        var problem = problems[i];
                        int row = i + 2;
                        
                        sheet.Cells[row, 1].Value = i + 1;
                        sheet.Cells[row, 2].Value = problem.TanggalUpdate.ToString("dd/MM/yyyy");
                        sheet.Cells[row, 3].Value = problem.Shift;
                        sheet.Cells[row, 4].Value = problem.Department;
                        sheet.Cells[row, 5].Value = problem.PicLeader;
                        sheet.Cells[row, 6].Value = problem.NamaAreaLine;
                        sheet.Cells[row, 7].Value = problem.NamaOperator;
                        sheet.Cells[row, 8].Value = problem.Jenis4M;
                        sheet.Cells[row, 9].Value = problem.Standard4M ?? "";
                        sheet.Cells[row, 10].Value = problem.Actual4M ?? "";
                        sheet.Cells[row, 11].Value = problem.KeteranganProblem;
                        sheet.Cells[row, 12].Value = problem.TemporaryAction ?? "";
                        sheet.Cells[row, 13].Value = problem.RencanaPerbaikan;
                        sheet.Cells[row, 14].Value = problem.TanggalRencanaPerbaikan.ToString("dd/MM/yyyy");
                        sheet.Cells[row, 15].Value = problem.AktualPerbaikan ?? "";
                        sheet.Cells[row, 16].Value = problem.TanggalAktualPerbaikan.HasValue 
                            ? problem.TanggalAktualPerbaikan.Value.ToString("dd/MM/yyyy") 
                            : "";
                        sheet.Cells[row, 17].Value = problem.Status;

                        // Color coding for status
                        if (problem.Status?.ToLower() == "selesai")
                        {
                            sheet.Cells[row, 17].Style.Font.Color.SetColor(Color.FromArgb(16, 185, 129)); // Green
                            sheet.Cells[row, 17].Style.Font.Bold = true;
                        }
                        else if (problem.Status?.ToLower() == "delay")
                        {
                            sheet.Cells[row, 17].Style.Font.Color.SetColor(Color.FromArgb(239, 68, 68)); // Red
                            sheet.Cells[row, 17].Style.Font.Bold = true;
                        }
                        else
                        {
                            sheet.Cells[row, 17].Style.Font.Color.SetColor(Color.FromArgb(245, 158, 11)); // Orange
                        }
                    }

                    // Auto fit columns
                    sheet.Cells.AutoFitColumns();
                    
                    // Set column widths for better readability
                    sheet.Column(1).Width = 5; // No
                    sheet.Column(2).Width = 15; // Tanggal Update
                    sheet.Column(3).Width = 10; // Shift
                    sheet.Column(4).Width = 15; // Department
                    sheet.Column(5).Width = 15; // PIC Leader
                    sheet.Column(6).Width = 15; // Area/Line
                    sheet.Column(7).Width = 15; // Operator
                    sheet.Column(8).Width = 12; // Jenis 4M
                    sheet.Column(9).Width = 20; // 4M Standard
                    sheet.Column(10).Width = 20; // 4M Actual
                    sheet.Column(11).Width = 30; // Keterangan Problem
                    sheet.Column(12).Width = 25; // Temporary Action
                    sheet.Column(13).Width = 25; // Rencana Perbaikan
                    sheet.Column(14).Width = 20; // Tanggal Rencana Perbaikan
                    sheet.Column(15).Width = 25; // Aktual Perbaikan
                    sheet.Column(16).Width = 20; // Tanggal Aktual Perbaikan
                    sheet.Column(17).Width = 12; // Status

                    // Freeze first row
                    sheet.View.FreezePanes(2, 1);

                    var fileName = $"Henkaten_{plantName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(package.GetAsByteArray(), 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                        fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExportToExcel: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = $"Gagal mengekspor data: {ex.Message}" });
            }
        }
    }
}
