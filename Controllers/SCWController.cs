using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Services;
using AMRVI.DTOs;
using AMRVI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AMRVI.Controllers
{
    [Authorize]
    public class SCWController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PlantService _plantService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public SCWController(
            ApplicationDbContext context,
            PlantService plantService,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _plantService = plantService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "SCW - Stop Call Work";
            ViewBag.CurrentPlant = _plantService.GetPlantName();
            
            // Get machines from checklist based on current plant
            var plantCode = _plantService.GetPlantName();
            
            // Get machines list (from existing checklist system)
            var machines = await GetMachinesForPlant(plantCode);
            ViewBag.Machines = machines;

            // Get status types
            var statusTypes = await _context.StatusTypes
                .OrderBy(s => s.Priority)
                .ToListAsync();
            ViewBag.StatusTypes = statusTypes;

            // Get 4M categories
            var fourMCategories = await _context.FourMCategories
                .OrderBy(f => f.Priority)
                .ToListAsync();
            ViewBag.FourMCategories = fourMCategories;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineNumbers(int machineId)
        {
            try
            {
                var plantCode = _plantService.GetPlantName();
                var machineNumbers = await GetMachineNumbersForMachine(plantCode, machineId);
                return Ok(machineNumbers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] CreateAndonRecordDto dto)
        {
            try
            {
                var plantCode = _plantService.GetPlantName();
                var userName = User.Identity?.Name ?? "Unknown";

                // Get Plant
                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.PlantCode == plantCode);
                if (plant == null)
                    return BadRequest(new { error = "Plant not found" });

                // Get or Create Machine
                var machine = await _context.AndonMachines
                    .FirstOrDefaultAsync(m => m.PlantId == plant.Id && m.MachineCode == dto.MachineCode);
                
                if (machine == null)
                {
                    machine = new Models.AndonMachine
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
                    existingRecord.CreatedBy = userName;
                    existingRecord.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Create New Record
                    var record = new Models.AndonRecord
                    {
                        PlantId = plant.Id,
                        MachineId = machine.Id,
                        StatusId = status.Id,
                        FourMCategoryId = fourM.Id,
                        Remark = dto.Remark,
                        CreatedBy = userName,
                        RecordedAt = DateTime.Now,
                        IsResolved = false
                    };
                    _context.AndonRecords.Add(record);
                }

                await _context.SaveChangesAsync();

                // Broadcast update via SignalR with details
                await _hubContext.Clients.All.SendAsync("AndonDataUpdated", new { 
                    PlantCode = plantCode, 
                    MachineCode = dto.MachineCode, 
                    StatusName = status.StatusName,
                    FourMName = fourM.CategoryName,
                    Remark = dto.Remark
                });

                return Ok(new { success = true, message = "Data berhasil disimpan!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private async Task<List<object>> GetMachinesForPlant(string plantCode)
        {
            // Get machines based on plant code from existing checklist system
            switch (plantCode)
            {
                case "RVI":
                    return await _context.Machines
                        .Where(m => m.IsActive)
                        .Select(m => new { id = m.Id, name = m.Name })
                        .Cast<object>()
                        .ToListAsync();
                case "BTR":
                    return await _context.Machines_BTR
                        .Where(m => m.IsActive)
                        .Select(m => new { id = m.Id, name = m.Name })
                        .Cast<object>()
                        .ToListAsync();
                case "HOSE":
                    return await _context.Machines_HOSE
                        .Where(m => m.IsActive)
                        .Select(m => new { id = m.Id, name = m.Name })
                        .Cast<object>()
                        .ToListAsync();
                case "MOLDED":
                    return await _context.Machines_MOLDED
                        .Where(m => m.IsActive)
                        .Select(m => new { id = m.Id, name = m.Name })
                        .Cast<object>()
                        .ToListAsync();
                case "MIXING":
                    return await _context.Machines_MIXING
                        .Where(m => m.IsActive)
                        .Select(m => new { id = m.Id, name = m.Name })
                        .Cast<object>()
                        .ToListAsync();
                default:
                    return new List<object>();
            }
        }

        private async Task<List<object>> GetMachineNumbersForMachine(string plantCode, int machineId)
        {
            // Get machine numbers based on plant code
            switch (plantCode)
            {
                case "RVI":
                    return await _context.MachineNumbers
                        .Where(mn => mn.MachineId == machineId && mn.IsActive)
                        .Select(mn => new { id = mn.Id, number = mn.Number, location = mn.Location })
                        .Cast<object>()
                        .ToListAsync();
                case "BTR":
                    return await _context.MachineNumbers_BTR
                        .Where(mn => mn.MachineId == machineId && mn.IsActive)
                        .Select(mn => new { id = mn.Id, number = mn.Number, location = mn.Location })
                        .Cast<object>()
                        .ToListAsync();
                case "HOSE":
                    return await _context.MachineNumbers_HOSE
                        .Where(mn => mn.MachineId == machineId && mn.IsActive)
                        .Select(mn => new { id = mn.Id, number = mn.Number, location = mn.Location })
                        .Cast<object>()
                        .ToListAsync();
                case "MOLDED":
                    return await _context.MachineNumbers_MOLDED
                        .Where(mn => mn.MachineId == machineId && mn.IsActive)
                        .Select(mn => new { id = mn.Id, number = mn.Number, location = mn.Location })
                        .Cast<object>()
                        .ToListAsync();
                case "MIXING":
                    return await _context.MachineNumbers_MIXING
                        .Where(mn => mn.MachineId == machineId && mn.IsActive)
                        .Select(mn => new { id = mn.Id, number = mn.Number, location = mn.Location })
                        .Cast<object>()
                        .ToListAsync();
                default:
                    return new List<object>();
            }
        }
    }
}
