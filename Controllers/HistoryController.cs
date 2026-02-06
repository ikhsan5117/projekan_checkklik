using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using AMRVI.ViewModels;
using AMRVI.Services;
using OfficeOpenXml; // Needed for Excel
using System.Drawing; // Needed for Color

namespace AMRVI.Controllers
{
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PlantService _plantService;

        public HistoryController(ApplicationDbContext context, PlantService plantService)
        {
            _context = context;
            _plantService = plantService;
        }

        public async Task<IActionResult> Index()
        {
            var plant = _plantService.GetPlantName();
            var username = User.Identity?.Name;
            var fullName = User.FindFirst("FullName")?.Value;
            bool restrict = !User.IsInRole("Administrator") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor");

            List<InspectionSession> sessions = new List<InspectionSession>();

            // Mapping Helper Logic
            // We map strict Plant Entities to the Base InspectionSession Entity for View Compatibility
            switch (plant)
            {
                case "BTR":
                    var qBTR = _context.InspectionSessions_BTR
                        .Include(s => s.MachineNumber).ThenInclude(m => m.Machine)
                        .Include(s => s.InspectionResults)
                        .AsQueryable();
                    if(restrict) qBTR = qBTR.Where(s => s.InspectorName == username || (fullName != null && s.InspectorName == fullName));
                    
                    var listBTR = await qBTR.OrderByDescending(s => s.InspectionDate).Take(100).ToListAsync();
                    sessions = listBTR.Select(s => new InspectionSession {
                        Id = s.Id,
                        InspectorName = s.InspectorName,
                        InspectionDate = s.InspectionDate,
                        IsCompleted = s.IsCompleted,
                        CompletedAt = s.CompletedAt,
                        MachineNumberId = s.MachineNumberId,
                        MachineNumber = new MachineNumber { Number = s.MachineNumber?.Number ?? "-", Machine = new Machine { Name = s.MachineNumber?.Machine?.Name ?? "-" } },
                        InspectionResults = s.InspectionResults.Select(r => new InspectionResult { Id = r.Id, Judgement = r.Judgement }).ToList() // Minimal mapping needed for Index
                    }).ToList();
                    break;

                case "HOSE":
                    var qHOSE = _context.InspectionSessions_HOSE.Include(s => s.MachineNumber).ThenInclude(m => m.Machine).Include(s => s.InspectionResults).AsQueryable();
                    if(restrict) qHOSE = qHOSE.Where(s => s.InspectorName == username || (fullName != null && s.InspectorName == fullName));
                    var listHOSE = await qHOSE.OrderByDescending(s => s.InspectionDate).Take(100).ToListAsync();
                    sessions = listHOSE.Select(s => new InspectionSession {
                        Id = s.Id, InspectorName = s.InspectorName, InspectionDate = s.InspectionDate, IsCompleted = s.IsCompleted, CompletedAt = s.CompletedAt, MachineNumberId = s.MachineNumberId,
                        MachineNumber = new MachineNumber { Number = s.MachineNumber?.Number ?? "-", Machine = new Machine { Name = s.MachineNumber?.Machine?.Name ?? "-" } },
                        InspectionResults = s.InspectionResults.Select(r => new InspectionResult { Id = r.Id, Judgement = r.Judgement }).ToList()
                    }).ToList();
                    break;

                case "MOLDED":
                    var qMOLDED = _context.InspectionSessions_MOLDED.Include(s => s.MachineNumber).ThenInclude(m => m.Machine).Include(s => s.InspectionResults).AsQueryable();
                    if(restrict) qMOLDED = qMOLDED.Where(s => s.InspectorName == username || (fullName != null && s.InspectorName == fullName));
                    var listMOLDED = await qMOLDED.OrderByDescending(s => s.InspectionDate).Take(100).ToListAsync();
                    sessions = listMOLDED.Select(s => new InspectionSession {
                        Id = s.Id, InspectorName = s.InspectorName, InspectionDate = s.InspectionDate, IsCompleted = s.IsCompleted, CompletedAt = s.CompletedAt, MachineNumberId = s.MachineNumberId,
                        MachineNumber = new MachineNumber { Number = s.MachineNumber?.Number ?? "-", Machine = new Machine { Name = s.MachineNumber?.Machine?.Name ?? "-" } },
                        InspectionResults = s.InspectionResults.Select(r => new InspectionResult { Id = r.Id, Judgement = r.Judgement }).ToList()
                    }).ToList();
                    break;

                case "MIXING":
                    var qMIXING = _context.InspectionSessions_MIXING.Include(s => s.MachineNumber).ThenInclude(m => m.Machine).Include(s => s.InspectionResults).AsQueryable();
                    if(restrict) qMIXING = qMIXING.Where(s => s.InspectorName == username || (fullName != null && s.InspectorName == fullName));
                    var listMIXING = await qMIXING.OrderByDescending(s => s.InspectionDate).Take(100).ToListAsync();
                    sessions = listMIXING.Select(s => new InspectionSession {
                        Id = s.Id, InspectorName = s.InspectorName, InspectionDate = s.InspectionDate, IsCompleted = s.IsCompleted, CompletedAt = s.CompletedAt, MachineNumberId = s.MachineNumberId,
                        MachineNumber = new MachineNumber { Number = s.MachineNumber?.Number ?? "-", Machine = new Machine { Name = s.MachineNumber?.Machine?.Name ?? "-" } },
                        InspectionResults = s.InspectionResults.Select(r => new InspectionResult { Id = r.Id, Judgement = r.Judgement }).ToList()
                    }).ToList();
                    break;

                default: // RVI
                    var qRVI = _context.InspectionSessions.Include(s => s.MachineNumber).ThenInclude(m => m.Machine).Include(s => s.InspectionResults).AsQueryable();
                    if(restrict) qRVI = qRVI.Where(s => s.InspectorName == username || (fullName != null && s.InspectorName == fullName));
                    sessions = await qRVI.OrderByDescending(s => s.InspectionDate).Take(100).ToListAsync();
                    // No mapping needed for RVI as it IS the base class
                    break;
            }

            // Enrich sessions with calculated ShiftNumber based on inspection time and plant shift settings
            if (sessions.Any())
            {
                var upperPlant = plant?.ToUpper();
                var shiftSettings = _context.ShiftSettings
                    .Where(s => s.Plant.ToUpper() == upperPlant)
                    .OrderBy(s => s.ShiftNumber)
                    .ToList();

                // Fallback default shifts (same as in HomeController) when no settings defined yet
                if (!shiftSettings.Any())
                {
                    shiftSettings = new List<ShiftSetting>
                    {
                        new ShiftSetting { ShiftNumber = 1, StartTime = new TimeSpan(5, 0, 0), EndTime = new TimeSpan(14, 0, 0) },
                        new ShiftSetting { ShiftNumber = 2, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(22, 0, 0) },
                        new ShiftSetting { ShiftNumber = 3, StartTime = new TimeSpan(22, 0, 0), EndTime = new TimeSpan(5, 0, 0) }
                    };
                }

                foreach (var s in sessions)
                {
                    var time = s.InspectionDate.TimeOfDay;

                    // Same logic as dashboard: handle normal and midnight-crossing shifts
                    var activeSetting = shiftSettings.FirstOrDefault(sh =>
                        (sh.StartTime < sh.EndTime && time >= sh.StartTime && time < sh.EndTime) ||
                        (sh.StartTime > sh.EndTime && (time >= sh.StartTime || time < sh.EndTime))
                    );

                    s.ShiftNumber = activeSetting?.ShiftNumber;
                }
            }

            return View(sessions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var plant = _plantService.GetPlantName();
            InspectionSession? session = null;

            switch (plant)
            {
                case "BTR":
                    var sBTR = await _context.InspectionSessions_BTR.Include(s => s.MachineNumber).ThenInclude(m => m.Machine)
                        .Include(s => s.InspectionResults).ThenInclude(r => r.ChecklistItem) // Careful: ChecklistItem here is BTR type
                        .FirstOrDefaultAsync(s => s.Id == id);
                    if(sBTR != null) {
                        session = new InspectionSession {
                            Id = sBTR.Id, InspectorName = sBTR.InspectorName, InspectionDate = sBTR.InspectionDate, IsCompleted = sBTR.IsCompleted, CompletedAt = sBTR.CompletedAt,
                            MachineNumber = new MachineNumber { Number = sBTR.MachineNumber?.Number ?? "-", Machine = new Machine { Name = sBTR.MachineNumber?.Machine?.Name ?? "-" } },
                            InspectionResults = sBTR.InspectionResults.Select(r => new InspectionResult {
                                Id = r.Id, Judgement = r.Judgement, Remarks = r.Remarks,
                                ChecklistItem = new ChecklistItem { OrderNumber = r.ChecklistItem.OrderNumber, DetailName = r.ChecklistItem.DetailName, StandardDescription = r.ChecklistItem.StandardDescription }
                            }).ToList()
                        };
                    }
                    break;
                case "HOSE":
                    var sHOSE = await _context.InspectionSessions_HOSE.Include(s => s.MachineNumber).ThenInclude(m => m.Machine).Include(s => s.InspectionResults).ThenInclude(r => r.ChecklistItem).FirstOrDefaultAsync(s => s.Id == id);
                    if(sHOSE != null) {
                        session = new InspectionSession {
                            Id = sHOSE.Id, InspectorName = sHOSE.InspectorName, InspectionDate = sHOSE.InspectionDate, IsCompleted = sHOSE.IsCompleted, CompletedAt = sHOSE.CompletedAt,
                            MachineNumber = new MachineNumber { Number = sHOSE.MachineNumber?.Number ?? "-", Machine = new Machine { Name = sHOSE.MachineNumber?.Machine?.Name ?? "-" } },
                            InspectionResults = sHOSE.InspectionResults.Select(r => new InspectionResult {
                                Id = r.Id, Judgement = r.Judgement, Remarks = r.Remarks,
                                ChecklistItem = new ChecklistItem { OrderNumber = r.ChecklistItem.OrderNumber, DetailName = r.ChecklistItem.DetailName, StandardDescription = r.ChecklistItem.StandardDescription }
                            }).ToList()
                        };
                    }
                    break;
                case "MOLDED":
                    var sMOLDED = await _context.InspectionSessions_MOLDED.Include(s => s.MachineNumber).ThenInclude(m => m.Machine).Include(s => s.InspectionResults).ThenInclude(r => r.ChecklistItem).FirstOrDefaultAsync(s => s.Id == id);
                    if(sMOLDED != null) {
                        session = new InspectionSession {
                            Id = sMOLDED.Id, InspectorName = sMOLDED.InspectorName, InspectionDate = sMOLDED.InspectionDate, IsCompleted = sMOLDED.IsCompleted, CompletedAt = sMOLDED.CompletedAt,
                            MachineNumber = new MachineNumber { Number = sMOLDED.MachineNumber?.Number ?? "-", Machine = new Machine { Name = sMOLDED.MachineNumber?.Machine?.Name ?? "-" } },
                            InspectionResults = sMOLDED.InspectionResults.Select(r => new InspectionResult {
                                Id = r.Id, Judgement = r.Judgement, Remarks = r.Remarks,
                                ChecklistItem = new ChecklistItem { OrderNumber = r.ChecklistItem.OrderNumber, DetailName = r.ChecklistItem.DetailName, StandardDescription = r.ChecklistItem.StandardDescription }
                            }).ToList()
                        };
                    }
                    break;
                case "MIXING":
                    var sMIXING = await _context.InspectionSessions_MIXING.Include(s => s.MachineNumber).ThenInclude(m => m.Machine).Include(s => s.InspectionResults).ThenInclude(r => r.ChecklistItem).FirstOrDefaultAsync(s => s.Id == id);
                    if(sMIXING != null) {
                        session = new InspectionSession {
                            Id = sMIXING.Id, InspectorName = sMIXING.InspectorName, InspectionDate = sMIXING.InspectionDate, IsCompleted = sMIXING.IsCompleted, CompletedAt = sMIXING.CompletedAt,
                            MachineNumber = new MachineNumber { Number = sMIXING.MachineNumber?.Number ?? "-", Machine = new Machine { Name = sMIXING.MachineNumber?.Machine?.Name ?? "-" } },
                            InspectionResults = sMIXING.InspectionResults.Select(r => new InspectionResult {
                                Id = r.Id, Judgement = r.Judgement, Remarks = r.Remarks,
                                ChecklistItem = new ChecklistItem { OrderNumber = r.ChecklistItem.OrderNumber, DetailName = r.ChecklistItem.DetailName, StandardDescription = r.ChecklistItem.StandardDescription }
                            }).ToList()
                        };
                    }
                    break;
                default:
                    session = await _context.InspectionSessions.Include(s => s.MachineNumber).ThenInclude(mn => mn.Machine).Include(s => s.InspectionResults).ThenInclude(ir => ir.ChecklistItem).FirstOrDefaultAsync(s => s.Id == id);
                    break;
            }

            if (session == null) return NotFound();
            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(int id)
        {
            var plant = _plantService.GetPlantName();
            
            // Re-using Details logic for consistency
            var actionResult = await Details(id);
            if (actionResult is NotFoundResult) return NotFound();
            var viewResult = actionResult as ViewResult;
            if (viewResult == null || viewResult.Model == null) return NotFound();

            var session = viewResult.Model as InspectionSession;
            if (session == null) return NotFound();

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Inspection Details");
                
                // Header
                using (var range = sheet.Cells["A1:E1"])
                {
                    range.Value = new[] { "No", "Detail", "Standard", "Judgement", "Remarks" };
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 41, 59));
                    range.Style.Font.Color.SetColor(Color.White);
                }

                var results = session.InspectionResults.OrderBy(r => r.ChecklistItem?.OrderNumber ?? 0).ToList();
                for (int i = 0; i < results.Count; i++)
                {
                    sheet.Cells[i + 2, 1].Value = results[i].ChecklistItem?.OrderNumber;
                    sheet.Cells[i + 2, 2].Value = results[i].ChecklistItem?.DetailName;
                    sheet.Cells[i + 2, 3].Value = results[i].ChecklistItem?.StandardDescription;
                    sheet.Cells[i + 2, 4].Value = results[i].Judgement;
                    sheet.Cells[i + 2, 5].Value = results[i].Remarks;

                    if (results[i].Judgement == "NG")
                    {
                        sheet.Cells[i + 2, 4].Style.Font.Color.SetColor(Color.Red);
                        sheet.Cells[i + 2, 4].Style.Font.Bold = true;
                    }
                }
                sheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Inspection_{plant}_{id}.xlsx");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportHistory()
        {
            var result = await Index(); // Reuse Index logic to get list
            if (result is not ViewResult viewResult || viewResult.Model is not List<InspectionSession> sessions) return NotFound();

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("History");
                
                using (var range = sheet.Cells["A1:J1"])
                {
                    var headers = new[] { "ID", "Machine", "No.", "Inspector", "Date", "Status", "Shift", "OK", "NG", "Total" };
                    for (int h = 0; h < headers.Length; h++) sheet.Cells[1, h + 1].Value = headers[h];
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 41, 59));
                    range.Style.Font.Color.SetColor(Color.White);
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
                    sheet.Cells[i + 2, 7].Value = s.ShiftNumber.HasValue ? s.ShiftNumber.Value.ToString() : "";
                    sheet.Cells[i + 2, 8].Value = ok;
                    sheet.Cells[i + 2, 9].Value = ng;
                    sheet.Cells[i + 2, 10].Value = ok + ng;

                    if (ng > 0)
                    {
                        sheet.Cells[i + 2, 9].Style.Font.Color.SetColor(Color.Red);
                        sheet.Cells[i + 2, 9].Style.Font.Bold = true;
                    }
                }

                sheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"History_{_plantService.GetPlantName()}_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("Administrator") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor")) return Forbid();

            var plant = _plantService.GetPlantName();
            switch (plant)
            {
                case "BTR":
                    var sB = await _context.InspectionSessions_BTR.Include(s=>s.InspectionResults).FirstOrDefaultAsync(s => s.Id == id);
                    if(sB != null) { _context.InspectionResults_BTR.RemoveRange(sB.InspectionResults); _context.InspectionSessions_BTR.Remove(sB); }
                    break;
                case "HOSE":
                    var sH = await _context.InspectionSessions_HOSE.Include(s=>s.InspectionResults).FirstOrDefaultAsync(s => s.Id == id);
                    if(sH != null) { _context.InspectionResults_HOSE.RemoveRange(sH.InspectionResults); _context.InspectionSessions_HOSE.Remove(sH); }
                    break;
                case "MOLDED":
                    var sM = await _context.InspectionSessions_MOLDED.Include(s=>s.InspectionResults).FirstOrDefaultAsync(s => s.Id == id);
                    if(sM != null) { _context.InspectionResults_MOLDED.RemoveRange(sM.InspectionResults); _context.InspectionSessions_MOLDED.Remove(sM); }
                    break;
                case "MIXING":
                    var sX = await _context.InspectionSessions_MIXING.Include(s=>s.InspectionResults).FirstOrDefaultAsync(s => s.Id == id);
                    if(sX != null) { _context.InspectionResults_MIXING.RemoveRange(sX.InspectionResults); _context.InspectionSessions_MIXING.Remove(sX); }
                    break;
                default:
                    var sR = await _context.InspectionSessions.Include(s=>s.InspectionResults).FirstOrDefaultAsync(s => s.Id == id);
                    if(sR != null) { _context.InspectionResults.RemoveRange(sR.InspectionResults); _context.InspectionSessions.Remove(sR); }
                    break;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
