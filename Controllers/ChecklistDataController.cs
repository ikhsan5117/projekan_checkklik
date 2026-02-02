using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;

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
    }
}
