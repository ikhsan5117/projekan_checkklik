using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using AMRVI.ViewModels;
using Microsoft.AspNetCore.SignalR;
using AMRVI.Hubs;


namespace AMRVI.Controllers
{
    public class InspectionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InspectionController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public InspectionController(ApplicationDbContext context, ILogger<InspectionController> logger, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var inspectorName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "Guest";

            var oneDayAgo = DateTime.Now.AddDays(-1);
            var activeSession = await _context.InspectionSessions
                .Include(s => s.MachineNumber)
                .ThenInclude(mn => mn.Machine)
                .Where(s => s.InspectorName == inspectorName && !s.IsCompleted && s.CreatedAt > oneDayAgo)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            var viewModel = new InspectionViewModel
            {
                Machines = await _context.Machines
                    .Where(m => m.IsActive)
                    .Select(m => new MachineViewModel
                    {
                        Id = m.Id,
                        Name = m.Name
                    })
                    .ToListAsync(),
                
                InspectionSessionId = activeSession?.Id ?? 0,
                SelectedMachineId = activeSession?.MachineNumber?.MachineId,
                SelectedMachineNumberId = activeSession?.MachineNumberId,
                MachineName = activeSession?.MachineNumber?.Machine?.Name ?? "",
                MachineNumber = activeSession?.MachineNumber?.Number ?? ""
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineNumbers(int machineId)
        {
            var machineNumbers = await _context.MachineNumbers
                .Where(mn => mn.MachineId == machineId && mn.IsActive)
                .OrderBy(mn => mn.Number)
                .Select(mn => new MachineNumberViewModel
                {
                    Id = mn.Id,
                    Number = mn.Number,
                    MachineId = mn.MachineId
                })
                .ToListAsync();

            return Json(machineNumbers);
        }

        [HttpGet]
        public async Task<IActionResult> ResumeInspection(int sessionId)
        {
            try
            {
                var session = await _context.InspectionSessions
                    .Include(s => s.MachineNumber)
                    .ThenInclude(mn => mn.Machine)
                    .FirstOrDefaultAsync(s => s.Id == sessionId);

                if (session == null || session.IsCompleted)
                {
                    return Json(new { success = false, message = "Active session not found" });
                }

                // Get checklist items
                var checklistItems = await _context.ChecklistItems
                    .Where(ci => ci.MachineId == session.MachineNumber.MachineId && ci.IsActive)
                    .OrderBy(ci => ci.OrderNumber)
                    .Select(ci => new
                    {
                        id = ci.Id,
                        orderNumber = ci.OrderNumber,
                        detailName = ci.DetailName,
                        standardDescription = ci.StandardDescription,
                        imagePath = ci.ImagePath ?? ""
                    })
                    .ToListAsync();

                // Get current results
                var results = await _context.InspectionResults
                    .Where(ir => ir.InspectionSessionId == sessionId)
                    .ToDictionaryAsync(ir => ir.ChecklistItemId, ir => new { result = ir.Judgement, remarks = ir.Remarks });

                // Find where we left off (first item without a result)
                int resumeIndex = 0;
                for (int i = 0; i < checklistItems.Count; i++)
                {
                    if (!results.ContainsKey(checklistItems[i].id))
                    {
                        resumeIndex = i;
                        break;
                    }
                    // If all items have results, stay on last item or show completion
                    if (i == checklistItems.Count - 1) resumeIndex = i;
                }

                return Json(new
                {
                    success = true,
                    sessionId = session.Id,
                    checklistItems = checklistItems,
                    judgements = results,
                    currentIndex = resumeIndex,
                    machineId = session.MachineNumber.MachineId,
                    machineNumberId = session.MachineNumberId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming inspection");
                return Json(new { success = false, message = "Error resuming inspection" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AbandonSession(int sessionId)
        {
            try
            {
                var session = await _context.InspectionSessions.FindAsync(sessionId);
                if (session != null)
                {
                    // Instead of deleting, we mark it as completed/abandoned
                    session.IsCompleted = true; 
                    session.CompletedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error abandoning session");
                return Json(new { success = false, message = "Error abandoning session" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartInspection(int machineId, int machineNumberId)
        {
            try
            {
                // Create new inspection session
                var session = new InspectionSession
                {
                    MachineNumberId = machineNumberId,
                    InspectorName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "Guest",
                    InspectionDate = DateTime.Now,
                    IsCompleted = false,
                    CreatedAt = DateTime.Now
                };

                _context.InspectionSessions.Add(session);
                await _context.SaveChangesAsync();

                // Get ALL checklist items
                var checklistItems = await _context.ChecklistItems
                    .Where(ci => ci.MachineId == machineId && ci.IsActive)
                    .OrderBy(ci => ci.OrderNumber)
                    .Select(ci => new 
                    {
                        id = ci.Id,
                        orderNumber = ci.OrderNumber,
                        detailName = ci.DetailName,
                        standardDescription = ci.StandardDescription,
                        imagePath = ci.ImagePath ?? ""
                    })
                    .ToListAsync();

                if (checklistItems.Count == 0)
                {
                    return Json(new { success = false, message = "No checklist items found for this machine" });
                }

                var machine = await _context.Machines.FindAsync(machineId);
                var machineNumber = await _context.MachineNumbers.FindAsync(machineNumberId);

                return Json(new
                {
                    success = true,
                    sessionId = session.Id,
                    checklistItems = checklistItems, // Return full list
                    machineName = machine?.Name,
                    machineNumber = machineNumber?.Number
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting inspection");
                return Json(new { success = false, message = "Error starting inspection" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChecklistItem(int sessionId, int checklistItemId)
        {
            try
            {
                var session = await _context.InspectionSessions
                    .Include(s => s.MachineNumber)
                    .ThenInclude(mn => mn.Machine)
                    .FirstOrDefaultAsync(s => s.Id == sessionId);

                if (session == null)
                {
                    return Json(new { success = false, message = "Session not found" });
                }

                var checklistItem = await _context.ChecklistItems
                    .FirstOrDefaultAsync(ci => ci.Id == checklistItemId);

                if (checklistItem == null)
                {
                    return Json(new { success = false, message = "Checklist item not found" });
                }

                // Check if already answered
                var existingResult = await _context.InspectionResults
                    .FirstOrDefaultAsync(ir => ir.InspectionSessionId == sessionId && ir.ChecklistItemId == checklistItemId);

                var totalChecklists = await _context.ChecklistItems
                    .CountAsync(ci => ci.MachineId == session.MachineNumber.MachineId && ci.IsActive);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = checklistItem.Id,
                        orderNumber = checklistItem.OrderNumber,
                        detailName = checklistItem.DetailName,
                        standardDescription = checklistItem.StandardDescription,
                        imagePath = checklistItem.ImagePath,
                        currentJudgement = existingResult?.Judgement,
                        currentIndex = checklistItem.OrderNumber,
                        totalChecklists = totalChecklists
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting checklist item");
                return Json(new { success = false, message = "Error getting checklist item" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitJudgement([FromBody] InspectionSubmitModel model)
        {
            try
            {
                var session = await _context.InspectionSessions
                    .Include(s => s.MachineNumber)
                    .FirstOrDefaultAsync(s => s.Id == model.InspectionSessionId);

                if (session == null)
                {
                    return Json(new { success = false, message = "Session not found" });
                }

                // Check if already exists
                var existingResult = await _context.InspectionResults
                    .FirstOrDefaultAsync(ir => ir.InspectionSessionId == model.InspectionSessionId 
                        && ir.ChecklistItemId == model.ChecklistItemId);

                if (existingResult != null)
                {
                    // Update existing
                    existingResult.Judgement = model.Judgement;
                    existingResult.Remarks = model.Remarks;
                }
                else
                {
                    // Create new
                    var result = new InspectionResult
                    {
                        InspectionSessionId = model.InspectionSessionId,
                        ChecklistItemId = model.ChecklistItemId,
                        Judgement = model.Judgement,
                        Remarks = model.Remarks,
                        CreatedAt = DateTime.Now
                    };
                    _context.InspectionResults.Add(result);
                }

                await _context.SaveChangesAsync();

                // Get next checklist item
                var currentItem = await _context.ChecklistItems.FindAsync(model.ChecklistItemId);
                var nextItem = await _context.ChecklistItems
                    .Where(ci => ci.MachineId == session.MachineNumber.MachineId 
                        && ci.IsActive 
                        && ci.OrderNumber > currentItem!.OrderNumber)
                    .OrderBy(ci => ci.OrderNumber)
                    .FirstOrDefaultAsync();

                if (nextItem != null)
                {
                    return Json(new
                    {
                        success = true,
                        hasNext = true,
                        nextChecklistItemId = nextItem.Id
                    });
                }
                else
                {
                    // Complete the session
                    session.IsCompleted = true;
                    session.CompletedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Notify all clients via SignalR that new data is available
                    await _hubContext.Clients.All.SendAsync("RefreshData", $"New inspection completed: {session.InspectorName}");

                    return Json(new
                    {
                        success = true,
                        hasNext = false,
                        message = "Inspection completed successfully!"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting judgement");
                return Json(new { success = false, message = "Error submitting judgement" });
            }
        }
    }
}
