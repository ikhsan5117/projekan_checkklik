using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using AMRVI.Models;
using AMRVI.ViewModels;
using Microsoft.AspNetCore.SignalR;
using AMRVI.Hubs;
using AMRVI.Services; // Added

namespace AMRVI.Controllers
{
    public class InspectionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InspectionController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly PlantService _plantService;

        public InspectionController(ApplicationDbContext context, ILogger<InspectionController> logger, IHubContext<NotificationHub> hubContext, PlantService plantService)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
            _plantService = plantService;
        }

        public async Task<IActionResult> Index()
        {
            var inspectorName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "Guest";
            var plant = _plantService.GetPlantName();

            var oneDayAgo = DateTime.Now.AddDays(-1);
            
            // Helper DTO for active session info
            int activeSessionId = 0;
            int? selectedMachineId = null;
            int? selectedMachineNumberId = null;
            string machineName = "";
            string machineNumber = "";

            // 1. Get Active Session based on Plant
            switch (plant)
            {
                case "BTR":
                    var sBTR = await _context.InspectionSessions_BTR
                        .Include(s => s.MachineNumber).ThenInclude(mn => mn.Machine)
                        .Where(s => s.InspectorName == inspectorName && !s.IsCompleted && s.CreatedAt > oneDayAgo)
                        .OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync();
                    if(sBTR != null) { activeSessionId = sBTR.Id; selectedMachineId = sBTR.MachineNumber?.MachineId; selectedMachineNumberId = sBTR.MachineNumberId; machineName = sBTR.MachineNumber?.Machine?.Name ?? ""; machineNumber = sBTR.MachineNumber?.Number ?? ""; }
                    break;
                case "HOSE":
                    var sHOSE = await _context.InspectionSessions_HOSE
                        .Include(s => s.MachineNumber).ThenInclude(mn => mn.Machine)
                        .Where(s => s.InspectorName == inspectorName && !s.IsCompleted && s.CreatedAt > oneDayAgo)
                        .OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync();
                     if(sHOSE != null) { activeSessionId = sHOSE.Id; selectedMachineId = sHOSE.MachineNumber?.MachineId; selectedMachineNumberId = sHOSE.MachineNumberId; machineName = sHOSE.MachineNumber?.Machine?.Name ?? ""; machineNumber = sHOSE.MachineNumber?.Number ?? ""; }
                    break;
                case "MOLDED":
                    var sMOLDED = await _context.InspectionSessions_MOLDED
                        .Include(s => s.MachineNumber).ThenInclude(mn => mn.Machine)
                        .Where(s => s.InspectorName == inspectorName && !s.IsCompleted && s.CreatedAt > oneDayAgo)
                        .OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync();
                     if(sMOLDED != null) { activeSessionId = sMOLDED.Id; selectedMachineId = sMOLDED.MachineNumber?.MachineId; selectedMachineNumberId = sMOLDED.MachineNumberId; machineName = sMOLDED.MachineNumber?.Machine?.Name ?? ""; machineNumber = sMOLDED.MachineNumber?.Number ?? ""; }
                    break;
                case "MIXING":
                    var sMIXING = await _context.InspectionSessions_MIXING
                        .Include(s => s.MachineNumber).ThenInclude(mn => mn.Machine)
                        .Where(s => s.InspectorName == inspectorName && !s.IsCompleted && s.CreatedAt > oneDayAgo)
                        .OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync();
                     if(sMIXING != null) { activeSessionId = sMIXING.Id; selectedMachineId = sMIXING.MachineNumber?.MachineId; selectedMachineNumberId = sMIXING.MachineNumberId; machineName = sMIXING.MachineNumber?.Machine?.Name ?? ""; machineNumber = sMIXING.MachineNumber?.Number ?? ""; }
                    break;
                default:
                    var sRVI = await _context.InspectionSessions
                        .Include(s => s.MachineNumber).ThenInclude(mn => mn.Machine)
                        .Where(s => s.InspectorName == inspectorName && !s.IsCompleted && s.CreatedAt > oneDayAgo)
                        .OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync();
                     if(sRVI != null) { activeSessionId = sRVI.Id; selectedMachineId = sRVI.MachineNumber?.MachineId; selectedMachineNumberId = sRVI.MachineNumberId; machineName = sRVI.MachineNumber?.Machine?.Name ?? ""; machineNumber = sRVI.MachineNumber?.Number ?? ""; }
                    break;
            }

            // 2. Get Machines List based on Plant
            List<MachineViewModel> machines = new List<MachineViewModel>();
            switch (plant)
            {
                case "BTR": machines = await _context.Machines_BTR.Where(m => m.IsActive).Select(m => new MachineViewModel { Id = m.Id, Name = m.Name }).ToListAsync(); break;
                case "HOSE": machines = await _context.Machines_HOSE.Where(m => m.IsActive).Select(m => new MachineViewModel { Id = m.Id, Name = m.Name }).ToListAsync(); break;
                case "MOLDED": machines = await _context.Machines_MOLDED.Where(m => m.IsActive).Select(m => new MachineViewModel { Id = m.Id, Name = m.Name }).ToListAsync(); break;
                case "MIXING": machines = await _context.Machines_MIXING.Where(m => m.IsActive).Select(m => new MachineViewModel { Id = m.Id, Name = m.Name }).ToListAsync(); break;
                default: machines = await _context.Machines.Where(m => m.IsActive).Select(m => new MachineViewModel { Id = m.Id, Name = m.Name }).ToListAsync(); break;
            }

            var viewModel = new InspectionViewModel
            {
                Machines = machines,
                InspectionSessionId = activeSessionId,
                SelectedMachineId = selectedMachineId,
                SelectedMachineNumberId = selectedMachineNumberId,
                MachineName = machineName,
                MachineNumber = machineNumber
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineNumbers(int machineId)
        {
            var plant = _plantService.GetPlantName();
            List<MachineNumberViewModel> machineNumbers = new List<MachineNumberViewModel>();

            switch (plant)
            {
                case "BTR":
                    machineNumbers = await _context.MachineNumbers_BTR
                        .Where(mn => mn.MachineId == machineId && mn.IsActive).OrderBy(mn => mn.Number)
                        .Select(mn => new MachineNumberViewModel { Id = mn.Id, Number = mn.Number, MachineId = mn.MachineId }).ToListAsync();
                    break;
                case "HOSE":
                     machineNumbers = await _context.MachineNumbers_HOSE
                        .Where(mn => mn.MachineId == machineId && mn.IsActive).OrderBy(mn => mn.Number)
                        .Select(mn => new MachineNumberViewModel { Id = mn.Id, Number = mn.Number, MachineId = mn.MachineId }).ToListAsync();
                    break;
                case "MOLDED":
                     machineNumbers = await _context.MachineNumbers_MOLDED
                        .Where(mn => mn.MachineId == machineId && mn.IsActive).OrderBy(mn => mn.Number)
                        .Select(mn => new MachineNumberViewModel { Id = mn.Id, Number = mn.Number, MachineId = mn.MachineId }).ToListAsync();
                    break;
                case "MIXING":
                     machineNumbers = await _context.MachineNumbers_MIXING
                        .Where(mn => mn.MachineId == machineId && mn.IsActive).OrderBy(mn => mn.Number)
                        .Select(mn => new MachineNumberViewModel { Id = mn.Id, Number = mn.Number, MachineId = mn.MachineId }).ToListAsync();
                    break;
                default:
                     machineNumbers = await _context.MachineNumbers
                        .Where(mn => mn.MachineId == machineId && mn.IsActive).OrderBy(mn => mn.Number)
                        .Select(mn => new MachineNumberViewModel { Id = mn.Id, Number = mn.Number, MachineId = mn.MachineId }).ToListAsync();
                    break;
            }

            return Json(machineNumbers);
        }

        [HttpGet]
        public async Task<IActionResult> ResumeInspection(int sessionId)
        {
            try
            {
                var plant = _plantService.GetPlantName();
                object? resultDto = null;

                // Shared logic encapsulated in generic/dynamic way is hard, so explicit repetition
                switch(plant)
                {
                    case "BTR":
                        var sBTR = await _context.InspectionSessions_BTR.Include(s => s.MachineNumber).FirstOrDefaultAsync(s => s.Id == sessionId);
                        if (sBTR == null || sBTR.IsCompleted) return Json(new { success = false, message = "Session not found" });
                        
                        var itemsBTR = await _context.ChecklistItems_BTR.Where(c => c.MachineId == sBTR.MachineNumber.MachineId && c.IsActive).OrderBy(c => c.OrderNumber)
                            .Select(c => new { id = c.Id, orderNumber = c.OrderNumber, detailName = c.DetailName, standardDescription = c.StandardDescription, imagePath = c.ImagePath ?? "" }).ToListAsync();
                        
                        var resBTR = await _context.InspectionResults_BTR.Where(r => r.InspectionSessionId == sessionId).ToDictionaryAsync(r => r.ChecklistItemId, r => new { result = r.Judgement, remarks = r.Remarks });
                        
                        int idxBTR = 0;
                        for(int i=0; i<itemsBTR.Count; i++) { if(!resBTR.ContainsKey(itemsBTR[i].id)) { idxBTR = i; break; }  if(i==itemsBTR.Count-1) idxBTR = i; }

                        resultDto = new { success = true, sessionId = sBTR.Id, checklistItems = itemsBTR, judgements = resBTR, currentIndex = idxBTR, machineId = sBTR.MachineNumber.MachineId, machineNumberId = sBTR.MachineNumberId };
                        break;

                    case "HOSE":
                        var sHOSE = await _context.InspectionSessions_HOSE.Include(s => s.MachineNumber).FirstOrDefaultAsync(s => s.Id == sessionId);
                        if (sHOSE == null || sHOSE.IsCompleted) return Json(new { success = false, message = "Session not found" });
                        var itemsHOSE = await _context.ChecklistItems_HOSE.Where(c => c.MachineId == sHOSE.MachineNumber.MachineId && c.IsActive).OrderBy(c => c.OrderNumber)
                            .Select(c => new { id = c.Id, orderNumber = c.OrderNumber, detailName = c.DetailName, standardDescription = c.StandardDescription, imagePath = c.ImagePath ?? "" }).ToListAsync();
                        var resHOSE = await _context.InspectionResults_HOSE.Where(r => r.InspectionSessionId == sessionId).ToDictionaryAsync(r => r.ChecklistItemId, r => new { result = r.Judgement, remarks = r.Remarks });
                        int idxHOSE = 0; for(int i=0; i<itemsHOSE.Count; i++) { if(!resHOSE.ContainsKey(itemsHOSE[i].id)) { idxHOSE = i; break; }  if(i==itemsHOSE.Count-1) idxHOSE = i; }
                        resultDto = new { success = true, sessionId = sHOSE.Id, checklistItems = itemsHOSE, judgements = resHOSE, currentIndex = idxHOSE, machineId = sHOSE.MachineNumber.MachineId, machineNumberId = sHOSE.MachineNumberId };
                        break;
                    
                    case "MOLDED":
                        var sMOLDED = await _context.InspectionSessions_MOLDED.Include(s => s.MachineNumber).FirstOrDefaultAsync(s => s.Id == sessionId);
                        if (sMOLDED == null || sMOLDED.IsCompleted) return Json(new { success = false, message = "Session not found" });
                        var itemsMOLDED = await _context.ChecklistItems_MOLDED.Where(c => c.MachineId == sMOLDED.MachineNumber.MachineId && c.IsActive).OrderBy(c => c.OrderNumber)
                            .Select(c => new { id = c.Id, orderNumber = c.OrderNumber, detailName = c.DetailName, standardDescription = c.StandardDescription, imagePath = c.ImagePath ?? "" }).ToListAsync();
                        var resMOLDED = await _context.InspectionResults_MOLDED.Where(r => r.InspectionSessionId == sessionId).ToDictionaryAsync(r => r.ChecklistItemId, r => new { result = r.Judgement, remarks = r.Remarks });
                        int idxMOLDED = 0; for(int i=0; i<itemsMOLDED.Count; i++) { if(!resMOLDED.ContainsKey(itemsMOLDED[i].id)) { idxMOLDED = i; break; }  if(i==itemsMOLDED.Count-1) idxMOLDED = i; }
                        resultDto = new { success = true, sessionId = sMOLDED.Id, checklistItems = itemsMOLDED, judgements = resMOLDED, currentIndex = idxMOLDED, machineId = sMOLDED.MachineNumber.MachineId, machineNumberId = sMOLDED.MachineNumberId };
                        break;
                    
                    case "MIXING":
                        var sMIXING = await _context.InspectionSessions_MIXING.Include(s => s.MachineNumber).FirstOrDefaultAsync(s => s.Id == sessionId);
                        if (sMIXING == null || sMIXING.IsCompleted) return Json(new { success = false, message = "Session not found" });
                        var itemsMIXING = await _context.ChecklistItems_MIXING.Where(c => c.MachineId == sMIXING.MachineNumber.MachineId && c.IsActive).OrderBy(c => c.OrderNumber)
                            .Select(c => new { id = c.Id, orderNumber = c.OrderNumber, detailName = c.DetailName, standardDescription = c.StandardDescription, imagePath = c.ImagePath ?? "" }).ToListAsync();
                        var resMIXING = await _context.InspectionResults_MIXING.Where(r => r.InspectionSessionId == sessionId).ToDictionaryAsync(r => r.ChecklistItemId, r => new { result = r.Judgement, remarks = r.Remarks });
                        int idxMIXING = 0; for(int i=0; i<itemsMIXING.Count; i++) { if(!resMIXING.ContainsKey(itemsMIXING[i].id)) { idxMIXING = i; break; }  if(i==itemsMIXING.Count-1) idxMIXING = i; }
                        resultDto = new { success = true, sessionId = sMIXING.Id, checklistItems = itemsMIXING, judgements = resMIXING, currentIndex = idxMIXING, machineId = sMIXING.MachineNumber.MachineId, machineNumberId = sMIXING.MachineNumberId };
                        break;

                    default:
                        var sRVI = await _context.InspectionSessions.Include(s => s.MachineNumber).FirstOrDefaultAsync(s => s.Id == sessionId);
                        if (sRVI == null || sRVI.IsCompleted) return Json(new { success = false, message = "Session not found" });
                        var itemsRVI = await _context.ChecklistItems.Where(c => c.MachineId == sRVI.MachineNumber.MachineId && c.IsActive).OrderBy(c => c.OrderNumber)
                             .Select(c => new { id = c.Id, orderNumber = c.OrderNumber, detailName = c.DetailName, standardDescription = c.StandardDescription, imagePath = c.ImagePath ?? "" }).ToListAsync();
                        var resRVI = await _context.InspectionResults.Where(r => r.InspectionSessionId == sessionId).ToDictionaryAsync(r => r.ChecklistItemId, r => new { result = r.Judgement, remarks = r.Remarks });
                        int idxRVI = 0; for(int i=0; i<itemsRVI.Count; i++) { if(!resRVI.ContainsKey(itemsRVI[i].id)) { idxRVI = i; break; }  if(i==itemsRVI.Count-1) idxRVI = i; }
                        resultDto = new { success = true, sessionId = sRVI.Id, checklistItems = itemsRVI, judgements = resRVI, currentIndex = idxRVI, machineId = sRVI.MachineNumber.MachineId, machineNumberId = sRVI.MachineNumberId };
                        break;
                }

                return Json(resultDto);
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
                var plant = _plantService.GetPlantName();
                switch(plant) {
                    case "BTR": var sB = await _context.InspectionSessions_BTR.FindAsync(sessionId); if(sB!=null) { sB.IsCompleted=true; sB.CompletedAt=DateTime.Now; } break;
                    case "HOSE": var sH = await _context.InspectionSessions_HOSE.FindAsync(sessionId); if(sH!=null) { sH.IsCompleted=true; sH.CompletedAt=DateTime.Now; } break;
                    case "MOLDED": var sM = await _context.InspectionSessions_MOLDED.FindAsync(sessionId); if(sM!=null) { sM.IsCompleted=true; sM.CompletedAt=DateTime.Now; } break;
                    case "MIXING": var sX = await _context.InspectionSessions_MIXING.FindAsync(sessionId); if(sX!=null) { sX.IsCompleted=true; sX.CompletedAt=DateTime.Now; } break;
                    default: var sR = await _context.InspectionSessions.FindAsync(sessionId); if(sR!=null) { sR.IsCompleted=true; sR.CompletedAt=DateTime.Now; } break;
                }
                await _context.SaveChangesAsync();
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
                var plant = _plantService.GetPlantName();
                var inspectorName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "Guest";
                object? result = null;

                switch (plant)
                {
                    case "BTR":
                        var sessBTR = new InspectionSession_BTR { MachineNumberId = machineNumberId, InspectorName = inspectorName, InspectionDate = DateTime.Now, IsCompleted = false, CreatedAt = DateTime.Now };
                        _context.InspectionSessions_BTR.Add(sessBTR); await _context.SaveChangesAsync();
                        var itemsBTR = await _context.ChecklistItems_BTR.Where(c=>c.MachineId==machineId && c.IsActive).OrderBy(c=>c.OrderNumber).Select(c=>new{id=c.Id, orderNumber=c.OrderNumber, detailName=c.DetailName, standardDescription=c.StandardDescription, imagePath=c.ImagePath??""}).ToListAsync();
                        var macBTR = await _context.Machines_BTR.FindAsync(machineId); var numBTR = await _context.MachineNumbers_BTR.FindAsync(machineNumberId);
                        result = new { success = true, sessionId = sessBTR.Id, checklistItems = itemsBTR, machineName = macBTR?.Name, machineNumber = numBTR?.Number };
                        break;
                    case "HOSE":
                        var sessHOSE = new InspectionSession_HOSE { MachineNumberId = machineNumberId, InspectorName = inspectorName, InspectionDate = DateTime.Now, IsCompleted = false, CreatedAt = DateTime.Now };
                        _context.InspectionSessions_HOSE.Add(sessHOSE); await _context.SaveChangesAsync();
                        var itemsHOSE = await _context.ChecklistItems_HOSE.Where(c=>c.MachineId==machineId && c.IsActive).OrderBy(c=>c.OrderNumber).Select(c=>new{id=c.Id, orderNumber=c.OrderNumber, detailName=c.DetailName, standardDescription=c.StandardDescription, imagePath=c.ImagePath??""}).ToListAsync();
                        var macHOSE = await _context.Machines_HOSE.FindAsync(machineId); var numHOSE = await _context.MachineNumbers_HOSE.FindAsync(machineNumberId);
                        result = new { success = true, sessionId = sessHOSE.Id, checklistItems = itemsHOSE, machineName = macHOSE?.Name, machineNumber = numHOSE?.Number };
                        break;
                    case "MOLDED":
                        var sessMOLDED = new InspectionSession_MOLDED { MachineNumberId = machineNumberId, InspectorName = inspectorName, InspectionDate = DateTime.Now, IsCompleted = false, CreatedAt = DateTime.Now };
                        _context.InspectionSessions_MOLDED.Add(sessMOLDED); await _context.SaveChangesAsync();
                        var itemsMOLDED = await _context.ChecklistItems_MOLDED.Where(c=>c.MachineId==machineId && c.IsActive).OrderBy(c=>c.OrderNumber).Select(c=>new{id=c.Id, orderNumber=c.OrderNumber, detailName=c.DetailName, standardDescription=c.StandardDescription, imagePath=c.ImagePath??""}).ToListAsync();
                        var macMOLDED = await _context.Machines_MOLDED.FindAsync(machineId); var numMOLDED = await _context.MachineNumbers_MOLDED.FindAsync(machineNumberId);
                        result = new { success = true, sessionId = sessMOLDED.Id, checklistItems = itemsMOLDED, machineName = macMOLDED?.Name, machineNumber = numMOLDED?.Number };
                        break;
                    case "MIXING":
                        var sessMIXING= new InspectionSession_MIXING { MachineNumberId = machineNumberId, InspectorName = inspectorName, InspectionDate = DateTime.Now, IsCompleted = false, CreatedAt = DateTime.Now };
                        _context.InspectionSessions_MIXING.Add(sessMIXING); await _context.SaveChangesAsync();
                        var itemsMIXING = await _context.ChecklistItems_MIXING.Where(c=>c.MachineId==machineId && c.IsActive).OrderBy(c=>c.OrderNumber).Select(c=>new{id=c.Id, orderNumber=c.OrderNumber, detailName=c.DetailName, standardDescription=c.StandardDescription, imagePath=c.ImagePath??""}).ToListAsync();
                        var macMIXING = await _context.Machines_MIXING.FindAsync(machineId); var numMIXING = await _context.MachineNumbers_MIXING.FindAsync(machineNumberId);
                        result = new { success = true, sessionId = sessMIXING.Id, checklistItems = itemsMIXING, machineName = macMIXING?.Name, machineNumber = numMIXING?.Number };
                        break;
                    default:
                        var sessRVI = new InspectionSession { MachineNumberId = machineNumberId, InspectorName = inspectorName, InspectionDate = DateTime.Now, IsCompleted = false, CreatedAt = DateTime.Now };
                        _context.InspectionSessions.Add(sessRVI); await _context.SaveChangesAsync();
                        var itemsRVI = await _context.ChecklistItems.Where(c=>c.MachineId==machineId && c.IsActive).OrderBy(c=>c.OrderNumber).Select(c=>new{id=c.Id, orderNumber=c.OrderNumber, detailName=c.DetailName, standardDescription=c.StandardDescription, imagePath=c.ImagePath??""}).ToListAsync();
                        var macRVI = await _context.Machines.FindAsync(machineId); var numRVI = await _context.MachineNumbers.FindAsync(machineNumberId);
                        result = new { success = true, sessionId = sessRVI.Id, checklistItems = itemsRVI, machineName = macRVI?.Name, machineNumber = numRVI?.Number };
                        break;
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting inspection");
                return Json(new { success = false, message = "Error starting inspection" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitJudgement([FromBody] InspectionSubmitModel model)
        {
            try
            {
                var plant = _plantService.GetPlantName();
                int nextId = 0;
                bool completed = false;

                switch (plant)
                {
                    case "BTR":
                         var resBTR = await _context.InspectionResults_BTR.FirstOrDefaultAsync(ir => ir.InspectionSessionId == model.InspectionSessionId && ir.ChecklistItemId == model.ChecklistItemId);
                         if (resBTR != null) { resBTR.Judgement = model.Judgement; resBTR.Remarks = model.Remarks; }
                         else { _context.InspectionResults_BTR.Add(new InspectionResult_BTR { InspectionSessionId = model.InspectionSessionId, ChecklistItemId = model.ChecklistItemId, Judgement = model.Judgement, Remarks = model.Remarks, CreatedAt = DateTime.Now }); }
                         await _context.SaveChangesAsync();
                         
                         var currBTR = await _context.ChecklistItems_BTR.FindAsync(model.ChecklistItemId);
                         if (currBTR != null)
                         {
                             var nextBTR = await _context.ChecklistItems_BTR.Where(c => c.MachineId == currBTR.MachineId && c.IsActive && c.OrderNumber > currBTR.OrderNumber).OrderBy(c => c.OrderNumber).FirstOrDefaultAsync();
                             if(nextBTR!=null) { nextId=nextBTR.Id; } 
                             else { completed=true; var sess = await _context.InspectionSessions_BTR.FindAsync(model.InspectionSessionId); if(sess!=null) { sess.IsCompleted=true; sess.CompletedAt=DateTime.Now; await _hubContext.Clients.All.SendAsync("RefreshData", $"New BTR inspection completed: {sess.InspectorName}"); } }
                         }
                         await _context.SaveChangesAsync();
                         break;
                     case "HOSE":
                         var resHOSE = await _context.InspectionResults_HOSE.FirstOrDefaultAsync(ir => ir.InspectionSessionId == model.InspectionSessionId && ir.ChecklistItemId == model.ChecklistItemId);
                         if (resHOSE != null) { resHOSE.Judgement = model.Judgement; resHOSE.Remarks = model.Remarks; }
                         else { _context.InspectionResults_HOSE.Add(new InspectionResult_HOSE { InspectionSessionId = model.InspectionSessionId, ChecklistItemId = model.ChecklistItemId, Judgement = model.Judgement, Remarks = model.Remarks, CreatedAt = DateTime.Now }); }
                         await _context.SaveChangesAsync();
                         
                         var currHOSE = await _context.ChecklistItems_HOSE.FindAsync(model.ChecklistItemId);
                         if (currHOSE != null)
                         {
                             var nextHOSE = await _context.ChecklistItems_HOSE.Where(c => c.MachineId == currHOSE.MachineId && c.IsActive && c.OrderNumber > currHOSE.OrderNumber).OrderBy(c => c.OrderNumber).FirstOrDefaultAsync();
                             if(nextHOSE!=null) { nextId=nextHOSE.Id; } 
                             else { completed=true; var sess = await _context.InspectionSessions_HOSE.FindAsync(model.InspectionSessionId); if(sess!=null) { sess.IsCompleted=true; sess.CompletedAt=DateTime.Now; await _hubContext.Clients.All.SendAsync("RefreshData", $"New HOSE inspection completed: {sess.InspectorName}"); } }
                         }
                         await _context.SaveChangesAsync();
                         break;
                     case "MOLDED":
                         var resMOLDED = await _context.InspectionResults_MOLDED.FirstOrDefaultAsync(ir => ir.InspectionSessionId == model.InspectionSessionId && ir.ChecklistItemId == model.ChecklistItemId);
                         if (resMOLDED != null) { resMOLDED.Judgement = model.Judgement; resMOLDED.Remarks = model.Remarks; }
                         else { _context.InspectionResults_MOLDED.Add(new InspectionResult_MOLDED { InspectionSessionId = model.InspectionSessionId, ChecklistItemId = model.ChecklistItemId, Judgement = model.Judgement, Remarks = model.Remarks, CreatedAt = DateTime.Now }); }
                         await _context.SaveChangesAsync();
                         
                         var currMOLDED = await _context.ChecklistItems_MOLDED.FindAsync(model.ChecklistItemId);
                         if (currMOLDED != null)
                         {
                             var nextMOLDED = await _context.ChecklistItems_MOLDED.Where(c => c.MachineId == currMOLDED.MachineId && c.IsActive && c.OrderNumber > currMOLDED.OrderNumber).OrderBy(c => c.OrderNumber).FirstOrDefaultAsync();
                             if(nextMOLDED!=null) { nextId=nextMOLDED.Id; } 
                             else { completed=true; var sess = await _context.InspectionSessions_MOLDED.FindAsync(model.InspectionSessionId); if(sess!=null) { sess.IsCompleted=true; sess.CompletedAt=DateTime.Now; await _hubContext.Clients.All.SendAsync("RefreshData", $"New MOLDED inspection completed: {sess.InspectorName}"); } }
                         }
                         await _context.SaveChangesAsync();
                         break;
                     case "MIXING":
                         var resMIXING = await _context.InspectionResults_MIXING.FirstOrDefaultAsync(ir => ir.InspectionSessionId == model.InspectionSessionId && ir.ChecklistItemId == model.ChecklistItemId);
                         if (resMIXING != null) { resMIXING.Judgement = model.Judgement; resMIXING.Remarks = model.Remarks; }
                         else { _context.InspectionResults_MIXING.Add(new InspectionResult_MIXING { InspectionSessionId = model.InspectionSessionId, ChecklistItemId = model.ChecklistItemId, Judgement = model.Judgement, Remarks = model.Remarks, CreatedAt = DateTime.Now }); }
                         await _context.SaveChangesAsync();
                         
                         var currMIXING = await _context.ChecklistItems_MIXING.FindAsync(model.ChecklistItemId);
                         if (currMIXING != null)
                         {
                             var nextMIXING = await _context.ChecklistItems_MIXING.Where(c => c.MachineId == currMIXING.MachineId && c.IsActive && c.OrderNumber > currMIXING.OrderNumber).OrderBy(c => c.OrderNumber).FirstOrDefaultAsync();
                             if(nextMIXING!=null) { nextId=nextMIXING.Id; } 
                             else { completed=true; var sess = await _context.InspectionSessions_MIXING.FindAsync(model.InspectionSessionId); if(sess!=null) { sess.IsCompleted=true; sess.CompletedAt=DateTime.Now; await _hubContext.Clients.All.SendAsync("RefreshData", $"New MIXING inspection completed: {sess.InspectorName}"); } }
                         }
                         await _context.SaveChangesAsync();
                         break;
                     default:
                         var resRVI = await _context.InspectionResults.FirstOrDefaultAsync(ir => ir.InspectionSessionId == model.InspectionSessionId && ir.ChecklistItemId == model.ChecklistItemId);
                         if (resRVI != null) { resRVI.Judgement = model.Judgement; resRVI.Remarks = model.Remarks; }
                         else { _context.InspectionResults.Add(new InspectionResult { InspectionSessionId = model.InspectionSessionId, ChecklistItemId = model.ChecklistItemId, Judgement = model.Judgement, Remarks = model.Remarks, CreatedAt = DateTime.Now }); }
                         await _context.SaveChangesAsync();
                         
                         var currRVI = await _context.ChecklistItems.FindAsync(model.ChecklistItemId);
                         if (currRVI != null)
                         {
                             var nextRVI = await _context.ChecklistItems.Where(c => c.MachineId == currRVI.MachineId && c.IsActive && c.OrderNumber > currRVI.OrderNumber).OrderBy(c => c.OrderNumber).FirstOrDefaultAsync();
                             if(nextRVI!=null) { nextId=nextRVI.Id; } 
                             else { completed=true; var sess = await _context.InspectionSessions.FindAsync(model.InspectionSessionId); if(sess!=null) { sess.IsCompleted=true; sess.CompletedAt=DateTime.Now; await _hubContext.Clients.All.SendAsync("RefreshData", $"New RVI inspection completed: {sess.InspectorName}"); } }
                         }
                         await _context.SaveChangesAsync();
                         break;
                }

                if (completed)
                {
                    return Json(new { success = true, hasNext = false, message = "Inspection completed successfully!" });
                }
                else
                {
                    return Json(new { success = true, hasNext = true, nextChecklistItemId = nextId });
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
