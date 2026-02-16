using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using AMRVI.Services;

namespace AMRVI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class HenkatenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PlantService _plantService;
        private readonly IWebHostEnvironment _environment;

        public HenkatenController(
            ApplicationDbContext context,
            PlantService plantService,
            IWebHostEnvironment environment)
        {
            _context = context;
            _plantService = plantService;
            _environment = environment;
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
                        TanggalUpdate = h.TanggalUpdate.ToString("dd/MM/yyyy"),
                        h.Shift,
                        h.PicLeader,
                        h.NamaAreaLine,
                        h.NamaOperator,
                        h.Jenis4M,
                        h.KeteranganProblem,
                        h.RencanaPerbaikan,
                        TanggalRencanaPerbaikan = h.TanggalRencanaPerbaikan.ToString("dd/MM/yyyy"),
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
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] HenkatenProblem model, IFormFile? fotoTemuan, IFormFile? fotoAktual)
        {
            try
            {
                var plantId = _plantService.GetPlantId();
                model.PlantId = plantId;
                model.CreatedAt = DateTime.Now;

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

                return Ok(new { success = true, message = "Data berhasil disimpan" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
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
                existing.KeteranganProblem = model.KeteranganProblem;
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

                return Ok(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
