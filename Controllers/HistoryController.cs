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

            if (session == null) return NotFound();

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Inspection Details");
                
                // Header Styling
                using (var range = sheet.Cells["A1:E1"])
                {
                    range.Value = new[] { "No", "Detail", "Standard", "Judgement", "Remarks" };
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(30, 41, 59));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                var results = session.InspectionResults.OrderBy(r => r.ChecklistItem.OrderNumber).ToList();
                for (int i = 0; i < results.Count; i++)
                {
                    sheet.Cells[i + 2, 1].Value = results[i].ChecklistItem.OrderNumber;
                    sheet.Cells[i + 2, 2].Value = results[i].ChecklistItem.DetailName;
                    sheet.Cells[i + 2, 3].Value = results[i].ChecklistItem.StandardDescription;
                    sheet.Cells[i + 2, 4].Value = results[i].Judgement;
                    sheet.Cells[i + 2, 5].Value = results[i].Remarks;

                    if (results[i].Judgement == "NG")
                    {
                        sheet.Cells[i + 2, 4].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        sheet.Cells[i + 2, 4].Style.Font.Bold = true;
                    }
                }

                sheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Inspection_{id}.xlsx");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportHistory()
        {
            var query = _context.InspectionSessions
                .Include(s => s.MachineNumber)
                .ThenInclude(mn => mn.Machine)
                .Include(s => s.InspectionResults)
                .AsQueryable();

            if (!User.IsInRole("Administrator") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                var username = User.Identity?.Name;
                var fullName = User.FindFirst("FullName")?.Value;
                query = query.Where(s => s.InspectorName == username || (fullName != null && s.InspectorName == fullName));
            }

            var sessions = await query.OrderByDescending(s => s.InspectionDate).ToListAsync();

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("History");
                
                // Header Styling
                using (var range = sheet.Cells["A1:I1"])
                {
                    var headers = new[] { "ID", "Machine", "No.", "Inspector", "Date", "Status", "OK", "NG", "Total" };
                    for (int h = 0; h < headers.Length; h++) sheet.Cells[1, h + 1].Value = headers[h];
                    
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(30, 41, 59));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                for (int i = 0; i < sessions.Count; i++)
                {
                    var s = sessions[i];
                    var ok = s.InspectionResults.Count(r => r.Judgement == "OK");
                    var ng = s.InspectionResults.Count(r => r.Judgement == "NG");
                    
                    sheet.Cells[i + 2, 1].Value = s.Id;
                    sheet.Cells[i + 2, 2].Value = s.MachineNumber.Machine.Name;
                    sheet.Cells[i + 2, 3].Value = s.MachineNumber.Number;
                    sheet.Cells[i + 2, 4].Value = s.InspectorName;
                    sheet.Cells[i + 2, 5].Value = s.InspectionDate.ToString("yyyy-MM-dd HH:mm");
                    sheet.Cells[i + 2, 6].Value = s.IsCompleted ? "Completed" : "In Progress";
                    sheet.Cells[i + 2, 7].Value = ok;
                    sheet.Cells[i + 2, 8].Value = ng;
                    sheet.Cells[i + 2, 9].Value = ok + ng;

                    if (ng > 0)
                    {
                        sheet.Cells[i + 2, 8].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        sheet.Cells[i + 2, 8].Style.Font.Bold = true;
                    }
                }

                sheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"InspectionHistory_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.InspectionSessions
                .Include(s => s.InspectionResults)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            // Optional: Add authorization check here if needed (e.g. only Admins can delete)
            if (!User.IsInRole("Administrator") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Forbid();
            }

            _context.InspectionResults.RemoveRange(session.InspectionResults);
            _context.InspectionSessions.Remove(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
