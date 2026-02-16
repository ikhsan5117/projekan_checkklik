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
    public class ManPowerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PlantService _plantService;

        public ManPowerController(
            ApplicationDbContext context,
            PlantService plantService)
        {
            _context = context;
            _plantService = plantService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Master Data ManPower";
            ViewBag.CurrentPlant = _plantService.GetPlantName();
            return View();
        }

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData()
        {
            try
            {
                var plantId = _plantService.GetPlantId();
                var manPowers = await _context.ManPowers
                    .Where(m => m.PlantId == plantId)
                    .OrderBy(m => m.NamaLengkap)
                    .Select(m => new
                    {
                        m.Id,
                        m.NIK,
                        m.NamaLengkap,
                        m.Jabatan,
                        m.Department,
                        m.Shift,
                        m.AreaLine,
                        m.NoTelepon,
                        m.Email,
                        m.IsActive
                    })
                    .ToListAsync();

                return Ok(manPowers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ManPower model)
        {
            try
            {
                var plantId = _plantService.GetPlantId();
                model.PlantId = plantId;
                model.CreatedAt = DateTime.Now;

                // Check if NIK already exists
                var exists = await _context.ManPowers
                    .AnyAsync(m => m.NIK == model.NIK && m.PlantId == plantId);

                if (exists)
                {
                    return BadRequest(new { error = "NIK sudah terdaftar" });
                }

                _context.ManPowers.Add(model);
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
                var manPower = await _context.ManPowers.FindAsync(id);
                if (manPower == null)
                    return NotFound(new { error = "Data tidak ditemukan" });

                return Ok(manPower);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit([FromBody] ManPower model)
        {
            try
            {
                var existing = await _context.ManPowers.FindAsync(model.Id);
                if (existing == null)
                    return NotFound(new { error = "Data tidak ditemukan" });

                // Check if NIK already exists (excluding current record)
                var nikExists = await _context.ManPowers
                    .AnyAsync(m => m.NIK == model.NIK && m.Id != model.Id && m.PlantId == existing.PlantId);

                if (nikExists)
                {
                    return BadRequest(new { error = "NIK sudah terdaftar" });
                }

                // Update fields
                existing.NIK = model.NIK;
                existing.NamaLengkap = model.NamaLengkap;
                existing.Jabatan = model.Jabatan;
                existing.Department = model.Department;
                existing.Shift = model.Shift;
                existing.AreaLine = model.AreaLine;
                existing.NoTelepon = model.NoTelepon;
                existing.Email = model.Email;
                existing.IsActive = model.IsActive;
                existing.UpdatedAt = DateTime.Now;

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
                var manPower = await _context.ManPowers.FindAsync(id);
                if (manPower == null)
                    return NotFound(new { error = "Data tidak ditemukan" });

                _context.ManPowers.Remove(manPower);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var manPower = await _context.ManPowers.FindAsync(id);
                if (manPower == null)
                    return NotFound(new { error = "Data tidak ditemukan" });

                manPower.IsActive = !manPower.IsActive;
                manPower.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Status berhasil diupdate", isActive = manPower.IsActive });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
