using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.ViewModels;

namespace AMRVI.Controllers
{
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.InspectionSessions
                .Include(s => s.MachineNumber)
                .ThenInclude(mn => mn.Machine)
                .Include(s => s.InspectionResults)
                .AsQueryable();

            // Role-based filtering: Only Admins/Supervisors see all data
            if (!User.IsInRole("Administrator") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                var username = User.Identity?.Name;
                var fullName = User.FindFirst("FullName")?.Value;
                
                // Show records where InspectorName matches either username or Full Name
                query = query.Where(s => s.InspectorName == username || (fullName != null && s.InspectorName == fullName));
            }

            var sessions = await query
                .OrderByDescending(s => s.InspectionDate)
                .Take(100)
                .ToListAsync();

            return View(sessions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var session = await _context.InspectionSessions
                .Include(s => s.MachineNumber)
                .ThenInclude(mn => mn.Machine)
                .Include(s => s.InspectionResults)
                .ThenInclude(ir => ir.ChecklistItem)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(int id)
        {
            var session = await _context.InspectionSessions
                .Include(s => s.MachineNumber)
                .ThenInclude(mn => mn.Machine)
                .Include(s => s.InspectionResults)
                .ThenInclude(ir => ir.ChecklistItem)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            // TODO: Implement Excel export using EPPlus or ClosedXML
            // For now, return a simple CSV
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("No,Detail,Standard,Judgement,Remarks");

            foreach (var result in session.InspectionResults.OrderBy(r => r.ChecklistItem.OrderNumber))
            {
                csv.AppendLine($"{result.ChecklistItem.OrderNumber},{result.ChecklistItem.DetailName},{result.ChecklistItem.StandardDescription},{result.Judgement},{result.Remarks}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"Inspection_{session.Id}_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
