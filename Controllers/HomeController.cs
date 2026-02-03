using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AMRVI.Models;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace AMRVI.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AMRVI.Data.ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, AMRVI.Data.ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var today = DateTime.Today;

        // Fetch Stats
        var totalMachines = _context.MachineNumbers.Count(mn => mn.IsActive);
        var totalChecklists = _context.ChecklistItems.Count(c => c.IsActive);
        
        var inspectionsToday = _context.InspectionSessions
            .Where(s => s.InspectionDate >= today && s.IsCompleted)
            .Select(s => s.MachineNumberId)
            .Distinct()
            .Count();

        var issuesToday = _context.InspectionResults
            .Count(r => r.CreatedAt >= today && r.Judgement == "NG");

        // Fetch Recent Inspections
        var recentInspections = _context.InspectionSessions
            .Include(s => s.MachineNumber)
            .ThenInclude(mn => mn.Machine)
            .OrderByDescending(s => s.InspectionDate)
            .Take(5)
            .ToList();

        // Machine Status (Mocking logic for now: active if inspected today)
        // A more complex query could group by machine and check last status
        var machineStats = _context.Machines
            .Select(m => new AMRVI.ViewModels.MachineStatus
            {
                MachineName = m.Name,
                TotalInspections = m.MachineNumbers.SelectMany(mn => mn.InspectionSessions).Count(),
                // Ideally we'd join dynamically but simplistic approach for dashboard speed:
                LastInspection = m.MachineNumbers
                    .SelectMany(mn => mn.InspectionSessions)
                    .OrderByDescending(s => s.InspectionDate)
                    .Select(s => s.InspectionDate)
                    .FirstOrDefault()
            })
            .OrderByDescending(m => m.LastInspection)
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
            
            var okCount = _context.InspectionResults
                .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "OK");
            
            var ngCount = _context.InspectionResults
                .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "NG");
            
            okCountsPerMonth.Add(okCount);
            ngCountsPerMonth.Add(ngCount);
            monthLabels.Add(monthStart.ToString("MMM"));
        }

        // Calculate Weekly Performance (OK vs NG Percentage for This Week)
        var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Monday of this week
        
        var thisWeekOkCount = _context.InspectionResults
            .Count(r => r.CreatedAt >= startOfThisWeek && r.CreatedAt < today.AddDays(1) && r.Judgement == "OK");
        
        var thisWeekNgCount = _context.InspectionResults
            .Count(r => r.CreatedAt >= startOfThisWeek && r.CreatedAt < today.AddDays(1) && r.Judgement == "NG");
        
        var thisWeekTotal = thisWeekOkCount + thisWeekNgCount;
        
        double okPercentage = 0;
        double ngPercentage = 0;
        
        if (thisWeekTotal > 0)
        {
            okPercentage = ((double)thisWeekOkCount / thisWeekTotal) * 100;
            ngPercentage = ((double)thisWeekNgCount / thisWeekTotal) * 100;
        }

        // Calculate Total OK/NG Counts (All-Time)
        var totalOkCount = _context.InspectionResults.Count(r => r.Judgement == "OK");
        var totalNgCount = _context.InspectionResults.Count(r => r.Judgement == "NG");

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
            CurrentYear = currentYear
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
