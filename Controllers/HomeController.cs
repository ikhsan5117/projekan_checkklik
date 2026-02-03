using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AMRVI.Models;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace AMRVI.Controllers;

[Authorize(Roles = "Administrator,Admin,Supervisor")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AMRVI.Data.ApplicationDbContext _context;
    private readonly AMRVI.Services.PlantService _plantService;

    public HomeController(ILogger<HomeController> logger, AMRVI.Data.ApplicationDbContext context, AMRVI.Services.PlantService plantService)
    {
        _logger = logger;
        _context = context;
        _plantService = plantService;
    }

    public IActionResult Index()
    {
        var today = DateTime.Today;

        // Fetch Stats using PlantService for multi-plant support
        var totalMachines = _plantService.GetMachineNumbers().Count(mn => mn.IsActive);
        var totalChecklists = _plantService.GetChecklistItems().Count(c => c.IsActive);
        
        var inspectionsToday = _plantService.GetInspectionSessions()
            .Where(s => s.InspectionDate >= today && s.IsCompleted)
            .Select(s => s.MachineNumberId)
            .Distinct()
            .Count();

        var issuesToday = _plantService.GetInspectionResults()
            .Count(r => r.CreatedAt >= today && r.Judgement == "NG");

        // Fetch Recent Inspections
        var recentInspections = _plantService.GetInspectionSessions()
            .OrderByDescending(s => s.InspectionDate)
            .Take(5)
            .ToList();

        // Machine Status using PlantService
        var machineStats = _plantService.GetMachines()
            .Select(m => new AMRVI.ViewModels.MachineStatus
            {
                MachineName = m.Name,
                TotalInspections = 0, // Simplified for multi-plant
                LastInspection = DateTime.Now
            })
            .Take(4)
            .ToList();

        // Calculate Monthly Chart Data (Last 12 months for current year)
        var currentYear = today.Year;
        var okCountsPerMonth = new List<int>();
        var ngCountsPerMonth = new List<int>();
        var monthLabels = new List<string>();
        
        for (int month = 1; month <= 12; month++)
        {
            var monthStart = new DateTime(currentYear, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            
            var okCount = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "OK");
            
            var ngCount = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "NG");
            
            okCountsPerMonth.Add(okCount);
            ngCountsPerMonth.Add(ngCount);
            monthLabels.Add(monthStart.ToString("MMM"));
        }

        // Calculate Weekly Performance (OK vs NG Percentage for This Week)
        var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Monday of this week
        
        var thisWeekOkCount = _plantService.GetInspectionResults()
            .Count(r => r.CreatedAt >= startOfThisWeek && r.CreatedAt < today.AddDays(1) && r.Judgement == "OK");
        
        var thisWeekNgCount = _plantService.GetInspectionResults()
            .Count(r => r.CreatedAt >= startOfThisWeek && r.CreatedAt < today.AddDays(1) && r.Judgement == "NG");
        
        var thisWeekTotal = thisWeekOkCount + thisWeekNgCount;
        
        double okPercentage = 0;
        double ngPercentage = 0;
        
        if (thisWeekTotal > 0)
        {
            okPercentage = ((double)thisWeekOkCount / thisWeekTotal) * 100;
            ngPercentage = ((double)thisWeekNgCount / thisWeekTotal) * 100;
        }

        // Calculate Total OK/NG Counts (All-Time) using PlantService
        var totalOkCount = _plantService.GetInspectionResults().Count(r => r.Judgement == "OK");
        var totalNgCount = _plantService.GetInspectionResults().Count(r => r.Judgement == "NG");

        // NEW: Machine Update Status
        var allMachineNumbers = _plantService.GetMachineNumbers()
            .Where(mn => mn.IsActive)
            .ToList();

        var machineUpdateList = new List<AMRVI.ViewModels.MachineUpdateStatus>();
        
        foreach (var machineNumber in allMachineNumbers)
        {
            var lastInspection = _plantService.GetInspectionSessions()
                .Where(s => s.MachineNumberId == machineNumber.Id)
                .OrderByDescending(s => s.InspectionDate)
                .FirstOrDefault();

            var isUpdatedToday = lastInspection != null && lastInspection.InspectionDate >= today;

            machineUpdateList.Add(new AMRVI.ViewModels.MachineUpdateStatus
            {
                MachineNumberId = machineNumber.Id,
                MachineNumber = machineNumber.Number ?? "",
                MachineName = "", // Interface doesn't have Machine navigation property
                IsUpdatedToday = isUpdatedToday,
                LastInspectionDate = lastInspection?.InspectionDate
            });
        }

        var updatedMachinesCount = machineUpdateList.Count(m => m.IsUpdatedToday);
        var notUpdatedMachinesCount = machineUpdateList.Count(m => !m.IsUpdatedToday);

        // NEW: NG Trend Data (Last 30 days)
        var last30Days = today.AddDays(-29);
        var ngTrendDaily = new List<AMRVI.ViewModels.NgTrendData>();

        for (int i = 0; i < 30; i++)
        {
            var date = last30Days.AddDays(i);
            var ngCount = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt.Date == date && r.Judgement == "NG");

            ngTrendDaily.Add(new AMRVI.ViewModels.NgTrendData
            {
                Date = date,
                NgCount = ngCount
            });
        }

        // NEW: NG Detail Records (Last 20 NG findings)
        var ngResults = _plantService.GetInspectionResults()
            .Where(r => r.Judgement == "NG")
            .OrderByDescending(r => r.CreatedAt)
            .Take(20)
            .ToList();

        var ngDetailRecords = new List<AMRVI.ViewModels.NgDetailRecord>();
        
        foreach (var result in ngResults)
        {
            // Get related data separately
            var session = _plantService.GetInspectionSessions()
                .FirstOrDefault(s => s.Id == result.InspectionSessionId);
            
            var machineNumber = session != null 
                ? _plantService.GetMachineNumbers().FirstOrDefault(m => m.Id == session.MachineNumberId)
                : null;
            
            var checklistItem = _plantService.GetChecklistItems()
                .FirstOrDefault(c => c.Id == result.ChecklistItemId);

            ngDetailRecords.Add(new AMRVI.ViewModels.NgDetailRecord
            {
                Date = result.CreatedAt,
                MachineNumberId = session?.MachineNumberId ?? 0,
                MachineNumber = machineNumber?.Number ?? "-",
                NgCount = 1,
                ChecklistItem = checklistItem?.DetailName ?? "-",
                Standard = checklistItem?.StandardDescription ?? "-",
                Problem = result.Remarks ?? "-"
            });
        }

        var viewModel = new AMRVI.ViewModels.DashboardViewModel
        {
            TotalMachines = totalMachines,
            TotalChecklistItems = totalChecklists,
            InspectionsToday = inspectionsToday,
            IssuesToday = issuesToday,
            RecentInspections = recentInspections,
            MachineStatuses = machineStats,
            OkCountsPerDay = okCountsPerMonth,
            NgCountsPerDay = ngCountsPerMonth,
            ChartLabels = monthLabels,
            OkPercentage = okPercentage,
            NgPercentage = ngPercentage,
            ThisWeekOkCount = thisWeekOkCount,
            ThisWeekNgCount = thisWeekNgCount,
            TotalOkCount = totalOkCount,
            TotalNgCount = totalNgCount,
            ViewLevel = "monthly",
            CurrentYear = currentYear,
            // NEW: Machine Update Status
            MachineUpdateList = machineUpdateList,
            UpdatedMachinesCount = updatedMachinesCount,
            NotUpdatedMachinesCount = notUpdatedMachinesCount,
            // NEW: NG Trend Data
            NgTrendDaily = ngTrendDaily,
            NgDetailRecords = ngDetailRecords
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
