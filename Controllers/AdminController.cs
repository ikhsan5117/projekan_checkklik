using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;

namespace AMRVI.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Machine Management
        public async Task<IActionResult> ManageMachines()
        {
            var machines = await _context.Machines
                .Include(m => m.MachineNumbers)
                .OrderBy(m => m.Name)
                .ToListAsync();
            return View(machines);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMachine(string name, string description)
        {
            if (ModelState.IsValid)
            {
                var machine = new Machine
                {
                    Name = name,
                    Description = description,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                _context.Machines.Add(machine);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> EditMachine(int id, string name, string description, bool isActive)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null) return Json(new { success = false, message = "Machine not found" });

            machine.Name = name;
            machine.Description = description;
            machine.IsActive = isActive;
            
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null) return Json(new { success = false, message = "Machine not found" });

            // 1. Get IDs for cascades
            var machineNumberIds = await _context.MachineNumbers
                .Where(mn => mn.MachineId == id)
                .Select(mn => mn.Id)
                .ToListAsync();

            var checklistItemIds = await _context.ChecklistItems
                .Where(ci => ci.MachineId == id)
                .Select(ci => ci.Id)
                .ToListAsync();

            var sessionIds = await _context.InspectionSessions
                .Where(s => machineNumberIds.Contains(s.MachineNumberId))
                .Select(s => s.Id)
                .ToListAsync();

            // 2. Remove Inspection Results (Lowest level)
            // Linked to either these sessions OR these checklist items
            var resultsToDelete = _context.InspectionResults
                .Where(r => sessionIds.Contains(r.InspectionSessionId) || checklistItemIds.Contains(r.ChecklistItemId));
            _context.InspectionResults.RemoveRange(resultsToDelete);

            // 3. Remove Inspection Sessions
            var sessionsToDelete = _context.InspectionSessions
                .Where(s => sessionIds.Contains(s.Id));
            _context.InspectionSessions.RemoveRange(sessionsToDelete);

            // 4. Remove Machine Numbers
            var numbersToDelete = _context.MachineNumbers
                .Where(mn => mn.MachineId == id);
            _context.MachineNumbers.RemoveRange(numbersToDelete);

            // 5. Remove Checklist Items
            var itemsToDelete = _context.ChecklistItems
                .Where(ci => ci.MachineId == id);
            _context.ChecklistItems.RemoveRange(itemsToDelete);

            // 6. Remove Machine
            _context.Machines.Remove(machine);
            
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        #endregion

        #region Machine Number Management
        [HttpGet]
        public async Task<IActionResult> GetMachineNumbers(int machineId)
        {
            var numbers = await _context.MachineNumbers
                .Where(mn => mn.MachineId == machineId)
                .OrderBy(mn => mn.Number)
                .Select(mn => new { mn.Id, mn.Number, mn.Location, mn.IsActive })
                .ToListAsync();
            return Json(new { success = true, data = numbers });
        }

        [HttpPost]
        public async Task<IActionResult> AddMachineNumber(int machineId, string number, string location)
        {
            var machineNumber = new MachineNumber
            {
                MachineId = machineId,
                Number = number,
                Location = location,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            _context.MachineNumbers.Add(machineNumber);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> EditMachineNumber(int id, string number, string location)
        {
            var machineNumber = await _context.MachineNumbers.FindAsync(id);
            if (machineNumber == null) return Json(new { success = false, message = "Not found" });

            machineNumber.Number = number;
            machineNumber.Location = location;
            // machineNumber.IsActive = isActive; // Optional if we want to manage active state too

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMachineNumber(int id)
        {
            var number = await _context.MachineNumbers.FindAsync(id);
            if (number == null) return Json(new { success = false, message = "Not found" });

            _context.MachineNumbers.Remove(number);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        #endregion

        #region Checklist Management
        public async Task<IActionResult> ManageChecklists()
        {
            // We pass the list of machines so the view can populate a filter dropdown
            var machines = await _context.Machines.OrderBy(m => m.Name).ToListAsync();
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
                .Select(ci => new { 
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
                    // Create folder if not exists
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "checklist");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    // Create unique filename
                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Store relative path for web access
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

            // Hard delete for now, or just set inactive
            _context.ChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        #endregion

        public IActionResult ManageUsers()
        {
             // Placeholder for now
            return View();
        }
    }
}
