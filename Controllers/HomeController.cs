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
        var viewModel = GetDashboardViewModel();
        return View(viewModel);
    }

    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult DashboardPartial()
    {
        var viewModel = GetDashboardViewModel();
        return PartialView("_DashboardContent", viewModel);
    }

    [HttpGet]
    public IActionResult GetTrendData(int month, int year)
    {
        try 
        {
            // Validate inputs
            if (month < 1 || month > 12) return BadRequest("Invalid month");
            if (year < 2000 || year > 2100) return BadRequest("Invalid year");

            var startDate = new DateTime(year, month, 1);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var endDate = startDate.AddDays(daysInMonth - 1);

            // 1. Chart Data
            var ngTrendDaily = new List<object>();
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                // Note: This query inside loop is not optimal for high load but acceptable for low traffic simplified app.
                // Optimally, fetch all for range then aggregate in memory.
                var ngCount = _plantService.GetInspectionResults()
                    .Count(r => r.CreatedAt.Date == date && r.Judgement == "NG");

                ngTrendDaily.Add(new { 
                    date = date.ToString("yyyy-MM-dd"), 
                    label = date.ToString("dd MMM"),
                    ngCount = ngCount 
                });
            }

            // 2. Table Data (NG Details)
            var ngResults = _plantService.GetInspectionResults()
                .Where(r => r.Judgement == "NG" && r.CreatedAt.Date >= startDate && r.CreatedAt.Date <= endDate)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var ngDetailRecords = new List<object>();
            foreach (var result in ngResults)
            {
                var session = _plantService.GetInspectionSessions().FirstOrDefault(s => s.Id == result.InspectionSessionId);
                var machineNumber = session != null ? _plantService.GetMachineNumbers().FirstOrDefault(m => m.Id == session.MachineNumberId) : null;
                var checklistItem = _plantService.GetChecklistItems().FirstOrDefault(c => c.Id == result.ChecklistItemId);

                ngDetailRecords.Add(new {
                    date = result.CreatedAt.ToString("dd MMM yyyy"),
                    rawDate = result.CreatedAt.ToString("yyyy-MM-dd"), // Add separate raw date for filtering
                    machine = machineNumber?.Number ?? "-",
                    checklist = checklistItem?.DetailName ?? "-",
                    standard = checklistItem?.StandardDescription ?? "-",
                    problem = result.Remarks ?? "-"
                });
            }

            return Json(new { success = true, chartData = ngTrendDaily, tableData = ngDetailRecords });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trend data");
            return Json(new { success = false, message = "Error loading data" });
        }
    }

    private AMRVI.ViewModels.DashboardViewModel GetDashboardViewModel()
    {
        var now = DateTime.Now;
        var today = DateTime.Today;

        // Fetch Shift Settings for Current Plant
        var plant = _plantService.GetPlantName()?.ToUpper();
        var shiftSettings = _context.ShiftSettings
            .Where(s => s.Plant.ToUpper() == plant)
            .OrderBy(s => s.ShiftNumber)
            .ToList();

        // Fallback to default if not found
        if (!shiftSettings.Any())
        {
            shiftSettings = new List<ShiftSetting>
            {
                new ShiftSetting { ShiftNumber = 1, StartTime = new TimeSpan(5, 0, 0), EndTime = new TimeSpan(14, 0, 0) },
                new ShiftSetting { ShiftNumber = 2, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(22, 0, 0) },
                new ShiftSetting { ShiftNumber = 3, StartTime = new TimeSpan(22, 0, 0), EndTime = new TimeSpan(5, 0, 0) }
            };
        }

        int currentShift = 1;
        DateTime shiftStartTime = today;

        // Determine Current Shift based on TimeRange
        var timeNow = now.TimeOfDay;
        var activeSetting = shiftSettings.FirstOrDefault(s => 
            (s.StartTime < s.EndTime && timeNow >= s.StartTime && timeNow < s.EndTime) ||
            (s.StartTime > s.EndTime && (timeNow >= s.StartTime || timeNow < s.EndTime))
        );

        if (activeSetting != null)
        {
            currentShift = activeSetting.ShiftNumber;
            // Calculate shiftStartTime (handle midnight crossing)
            if (activeSetting.StartTime > activeSetting.EndTime && timeNow < activeSetting.EndTime)
            {
                shiftStartTime = today.AddDays(-1).Add(activeSetting.StartTime);
            }
            else
            {
                shiftStartTime = today.Add(activeSetting.StartTime);
            }
        }
        else 
        {
            // Default behavior if something is wrong
            currentShift = 1;
            shiftStartTime = today.AddHours(5);
        }

        // Calculate Next Shift Change Time
        DateTime? nextShiftChange = null;
        if (activeSetting != null)
        {
            if (activeSetting.StartTime < activeSetting.EndTime)
            {
                nextShiftChange = today.Add(activeSetting.EndTime);
            }
            else
            {
                // wraps midnight
                if (timeNow >= activeSetting.StartTime)
                    nextShiftChange = today.AddDays(1).Add(activeSetting.EndTime);
                else
                    nextShiftChange = today.Add(activeSetting.EndTime);
            }
        }

        // Fetch Stats using PlantService
        var totalMachines = _plantService.GetMachineNumbers().Count(mn => mn.IsActive);
        var totalChecklists = _plantService.GetChecklistItems().Count(c => c.IsActive);
        
        var inspectionsToday = _plantService.GetInspectionSessions()
            .Where(s => s.InspectionDate >= shiftStartTime && s.IsCompleted)
            .Select(s => s.MachineNumberId)
            .Distinct()
            .Count();

        var issuesToday = _plantService.GetInspectionResults()
            .Count(r => r.CreatedAt >= shiftStartTime && r.Judgement == "NG");

        // Fetch Recent Inspections
        var recentInspections = _plantService.GetInspectionSessions()
            .OrderByDescending(s => s.InspectionDate)
            .Take(5)
            .ToList();

        // Machine Status
        var machineStats = _plantService.GetMachines()
            .Select(m => new AMRVI.ViewModels.MachineStatus
            {
                MachineName = m.Name,
                TotalInspections = 0, 
                LastInspection = DateTime.Now
            })
            .Take(4)
            .ToList();

        // Calculate Monthly Chart Data (Yearly Overview)
        var currentYear = today.Year;
        var okCountsPerMonth = new List<int>();
        var ngCountsPerMonth = new List<int>();
        var monthLabels = new List<string>();
        
        for (int month = 1; month <= 12; month++)
        {
            var monthStart = new DateTime(currentYear, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            var okCount = _plantService.GetInspectionResults().Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "OK");
            var ngCount = _plantService.GetInspectionResults().Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "NG");
            okCountsPerMonth.Add(okCount);
            ngCountsPerMonth.Add(ngCount);
            monthLabels.Add(monthStart.ToString("MMM"));
        }

        // Weekly Performance
        var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek + 1);
        var thisWeekOkCount = _plantService.GetInspectionResults().Count(r => r.CreatedAt >= startOfThisWeek && r.CreatedAt < today.AddDays(1) && r.Judgement == "OK");
        var thisWeekNgCount = _plantService.GetInspectionResults().Count(r => r.CreatedAt >= startOfThisWeek && r.CreatedAt < today.AddDays(1) && r.Judgement == "NG");
        var thisWeekTotal = thisWeekOkCount + thisWeekNgCount;
        double okPercentage = (thisWeekTotal > 0) ? ((double)thisWeekOkCount / thisWeekTotal) * 100 : 0;
        double ngPercentage = (thisWeekTotal > 0) ? ((double)thisWeekNgCount / thisWeekTotal) * 100 : 0;

        // Total Counts
        var totalOkCount = _plantService.GetInspectionResults().Count(r => r.Judgement == "OK");
        var totalNgCount = _plantService.GetInspectionResults().Count(r => r.Judgement == "NG");

        // Machine Update Status (Filter by Shift Start Time)
        var allMachineNumbers = _plantService.GetMachineNumbers().Where(mn => mn.IsActive).ToList();
        var machineUpdateList = new List<AMRVI.ViewModels.MachineUpdateStatus>();
        foreach (var machineNumber in allMachineNumbers)
        {
            var lastInspection = _plantService.GetInspectionSessions().Where(s => s.MachineNumberId == machineNumber.Id).OrderByDescending(s => s.InspectionDate).FirstOrDefault();
            machineUpdateList.Add(new AMRVI.ViewModels.MachineUpdateStatus
            {
                MachineNumberId = machineNumber.Id,
                MachineNumber = machineNumber.Number ?? "",
                IsUpdatedToday = lastInspection != null && lastInspection.InspectionDate >= shiftStartTime,
                LastInspectionDate = lastInspection?.InspectionDate
            });
        }
        var updatedMachinesCount = machineUpdateList.Count(m => m.IsUpdatedToday);
        var notUpdatedMachinesCount = machineUpdateList.Count(m => !m.IsUpdatedToday);

        // NEW: NG Trend Data (Current Month Only)
        // Default View: Current Month (1st to Current Day/End of Month)
        var currentMonthStart = new DateTime(today.Year, today.Month, 1);
        var daysInCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
        var ngTrendDaily = new List<AMRVI.ViewModels.NgTrendData>();

        for (int day = 1; day <= daysInCurrentMonth; day++)
        {
            var date = new DateTime(today.Year, today.Month, day);
            // Optimally query once, but consistent with structure
            var ngCount = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt.Date == date && r.Judgement == "NG");

            ngTrendDaily.Add(new AMRVI.ViewModels.NgTrendData { Date = date, NgCount = ngCount });
        }

        // NEW: NG Detail Records (Current Month Only)
        // User requested: "tampilan pertamanya bulan yang sekarang"
        // So we filter table by Current Month as well.
        var ngResults = _plantService.GetInspectionResults()
            .Where(r => r.Judgement == "NG" && r.CreatedAt >= currentMonthStart && r.CreatedAt < currentMonthStart.AddMonths(1))
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var ngDetailRecords = new List<AMRVI.ViewModels.NgDetailRecord>();
        foreach (var result in ngResults)
        {
            var session = _plantService.GetInspectionSessions().FirstOrDefault(s => s.Id == result.InspectionSessionId);
            var machineNumber = session != null ? _plantService.GetMachineNumbers().FirstOrDefault(m => m.Id == session.MachineNumberId) : null;
            var checklistItem = _plantService.GetChecklistItems().FirstOrDefault(c => c.Id == result.ChecklistItemId);

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

        return new AMRVI.ViewModels.DashboardViewModel
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
            MachineUpdateList = machineUpdateList,
            UpdatedMachinesCount = updatedMachinesCount,
            NotUpdatedMachinesCount = notUpdatedMachinesCount,
            NgTrendDaily = ngTrendDaily,
            NgDetailRecords = ngDetailRecords,
            CurrentShift = currentShift,
            NextShiftChangeTime = nextShiftChange
        };
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
