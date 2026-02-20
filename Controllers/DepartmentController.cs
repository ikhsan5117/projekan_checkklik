using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using Microsoft.AspNetCore.Authorization;

namespace AMRVI.Controllers
{
    [Authorize] // Admin only? 
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: All Departments
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
            return Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Departments.AnyAsync(d => d.Name.ToLower() == model.Name.ToLower()))
                    return Json(new { success = false, message = "Departemen dengan nama tersebut sudah ada." });

                model.IsActive = true;
                _context.Departments.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Data tidak valid" });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return Json(new { success = false, message = "Not found" });
            return Json(new { success = true, data = dept });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Department model)
        {
             var existing = await _context.Departments.FindAsync(model.Id);
             if (existing == null) return Json(new { success = false, message = "Data tidak ditemukan" });

             existing.Name = model.Name;
             existing.Description = model.Description;
             
             await _context.SaveChangesAsync();
             return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept != null)
            {
                // Soft delete or hard delete? Let's do Hard Delete for simplicity or Soft Delete if safer
                // dept.IsActive = false; 
                _context.Departments.Remove(dept); // Hard delete for now unless used elsewhere
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Gagal menghapus" });
        }
    }
}
