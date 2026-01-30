using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AMRVI.Models;
using Microsoft.EntityFrameworkCore;

namespace AMRVI.Controllers;

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

        var viewModel = new AMRVI.ViewModels.DashboardViewModel
        {
            TotalMachines = totalMachines,
            TotalChecklistItems = totalChecklists,
            InspectionsToday = inspectionsToday,
            IssuesToday = issuesToday,
            RecentInspections = recentInspections,
            MachineStatuses = machineStats
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
