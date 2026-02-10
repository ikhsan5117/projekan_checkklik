using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AMRVI.Services;
using AMRVI.Data;
using AMRVI.Models;
using AMRVI.DTOs;
using AMRVI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AMRVI.Controllers
{
    [Authorize]
    public class AndonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductionDbContext _prodContext;
        private readonly PlantService _plantService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AndonController(
            ApplicationDbContext context,
            ProductionDbContext prodContext,
            PlantService plantService,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _prodContext = prodContext;
            _plantService = plantService;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Andon System Monitoring";
            ViewBag.CurrentPlant = _plantService.GetPlantName();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetData(string? plant = null)
        {
            try
            {
                // Ambil semua data hari ini agar sinkron dengan spreadsheets (termasuk yang sudah RUNNING/Resolved)
                // Filter by date (today) in SQL to improve performance
                var allLogs = await _prodContext.ScwLogs
                    .FromSqlRaw("SELECT * FROM [produksi].[tb_elwp_produksi_scw_logs] WHERE CAST([CreatedAt] AS DATE) = CAST(GETDATE() AS DATE)")
                    .ToListAsync();

                if (allLogs == null || !allLogs.Any())
                {
                    // If no data today, try fetching the last 50 records as fallback to verify connectivity
                    allLogs = await _prodContext.ScwLogs
                        .FromSqlRaw("SELECT TOP 50 * FROM [produksi].[tb_elwp_produksi_scw_logs] ORDER BY [CreatedAt] DESC")
                        .ToListAsync();
                }

                var records = allLogs
                    .Select(a => {
                        // Map PlantId based on ELWP_PRD production database
                        string plantCode = a.PlantId switch {
                            1 => "HOSE",
                            2 => "MOLDED",
                            3 => "RVI",
                            4 => "BTR",
                            _ => "UNK"
                        };

                        return new AndonRecordDto
                        {
                            Id = a.Id,
                            PlantCode = plantCode,
                            MachineCode = a.MesinId?.ToString() ?? "GENERAL",
                            StatusCode = a.Status?.ToUpper() ?? "UNKNOWN",
                            StatusName = a.Status?.ToUpper() ?? "UNKNOWN",
                            FourMCode = a.Jenis4M?.ToUpper() ?? "NONE",
                            FourMName = a.Jenis4M?.ToUpper() ?? "NONE",
                            AreaName = a.AreaId.ToString(),
                            DetailProblem = a.DetailProblem,
                            Remark = a.Keterangan ?? "-",
                            RecordedAt = a.CreatedAt,
                            IsResolved = a.ResolvedAt != null
                        };
                    })
                    .OrderByDescending(a => a.RecordedAt)
                    .ToList();

                // Apply plant filter if specified
                if (!string.IsNullOrEmpty(plant) && plant != "ALL")
                {
                    var filtered = records.Where(r => r.PlantCode.Equals(plant, StringComparison.OrdinalIgnoreCase)).ToList();
                    return Ok(filtered);
                }

                return Ok(records);
            }
            catch (Exception ex)
            {
                // Return detailed error for debugging in console
                return StatusCode(500, new { error = "Database Error", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAndonRecordDto dto)
        {
            try
            {
                // Get Plant
                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.PlantCode == dto.PlantCode);
                if (plant == null)
                    return BadRequest(new { error = "Plant not found" });

                // Get or Create Machine
                var machine = await _context.AndonMachines
                    .FirstOrDefaultAsync(m => m.PlantId == plant.Id && m.MachineCode == dto.MachineCode);
                
                if (machine == null)
                {
                    machine = new AndonMachine
                    {
                        PlantId = plant.Id,
                        MachineCode = dto.MachineCode,
                        MachineName = dto.MachineCode,
                        IsActive = true
                    };
                    _context.AndonMachines.Add(machine);
                    await _context.SaveChangesAsync();
                }

                // Get Status
                var status = await _context.StatusTypes
                    .FirstOrDefaultAsync(s => s.StatusCode == dto.StatusCode);
                if (status == null)
                    return BadRequest(new { error = "Status not found" });

                // Get FourM Category
                var fourM = await _context.FourMCategories
                    .FirstOrDefaultAsync(f => f.CategoryCode == dto.FourMCode);
                if (fourM == null)
                    return BadRequest(new { error = "4M Category not found" });

                // Check if there is an unresolved record for this machine
                var existingRecord = await _context.AndonRecords
                    .FirstOrDefaultAsync(r => r.MachineId == machine.Id && !r.IsResolved);

                if (existingRecord != null)
                {
                    // Update existing record
                    existingRecord.StatusId = status.Id;
                    existingRecord.FourMCategoryId = fourM.Id;
                    existingRecord.Remark = dto.Remark;
                    existingRecord.RecordedAt = DateTime.Now;
                    existingRecord.CreatedBy = dto.CreatedBy;
                    existingRecord.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Create Record
                    var record = new AndonRecord
                    {
                        PlantId = plant.Id,
                        MachineId = machine.Id,
                        StatusId = status.Id,
                        FourMCategoryId = fourM.Id,
                        Remark = dto.Remark,
                        CreatedBy = dto.CreatedBy,
                        RecordedAt = DateTime.Now,
                        IsResolved = false
                    };
                    _context.AndonRecords.Add(record);
                }

                string finalPlantCode = plant.PlantCode;
                await _context.SaveChangesAsync();

                // Broadcast update via SignalR
                return Ok(new { success = true, message = "Record processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAndonRecordDto dto)
        {
            try
            {
                var record = await _context.AndonRecords
                    .Include(a => a.Plant)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (record == null)
                    return NotFound(new { error = "Record not found" });

                // Update Status if provided
                if (!string.IsNullOrEmpty(dto.StatusCode))
                {
                    var status = await _context.StatusTypes
                        .FirstOrDefaultAsync(s => s.StatusCode == dto.StatusCode);
                    if (status != null)
                        record.StatusId = status.Id;
                }

                // Update FourM if provided
                if (!string.IsNullOrEmpty(dto.FourMCode))
                {
                    var fourM = await _context.FourMCategories
                        .FirstOrDefaultAsync(f => f.CategoryCode == dto.FourMCode);
                    if (fourM != null)
                        record.FourMCategoryId = fourM.Id;
                }

                // Update Remark if provided
                if (dto.Remark != null)
                    record.Remark = dto.Remark;

                // Update IsResolved if provided
                if (dto.IsResolved.HasValue)
                {
                    record.IsResolved = dto.IsResolved.Value;
                    if (dto.IsResolved.Value)
                        record.ResolvedAt = DateTime.Now;
                }

                record.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // Broadcast update via SignalR
                await _hubContext.Clients.All.SendAsync("AndonDataUpdated", record.Plant.PlantCode);

                return Ok(new { message = "Record updated successfully" });
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
                var record = await _context.AndonRecords
                    .Include(a => a.Plant)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (record == null)
                    return NotFound(new { error = "Record not found" });

                var plantCode = record.Plant.PlantCode;
                _context.AndonRecords.Remove(record);
                await _context.SaveChangesAsync();

                // Broadcast update via SignalR
                await _hubContext.Clients.All.SendAsync("AndonDataUpdated", plantCode);

                return Ok(new { message = "Record deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(string? plant = null)
        {
            try
            {
                var query = _context.AndonRecords
                    .Include(a => a.Plant)
                    .Include(a => a.StatusType)
                    .Include(a => a.FourMCategory)
                    .Where(a => !a.IsResolved)
                    .AsQueryable();

                // Filter by plant if specified
                if (!string.IsNullOrEmpty(plant) && plant != "ALL")
                {
                    query = query.Where(a => a.Plant.PlantCode == plant);
                }

                var records = await query.ToListAsync();

                var stats = new AndonStatisticsDto
                {
                    TotalRecords = records.Count,
                    ActiveIssues = records.Count(r => !r.IsResolved),
                    MachineStatus = records
                        .GroupBy(r => r.StatusType.StatusName)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    FourMStatus = records
                        .GroupBy(r => r.FourMCategory.CategoryName)
                        .ToDictionary(g => g.Key, g => g.Count())
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
