using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRVI.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AMRVI.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AMRVI.Services.PlantService _plantService;

        public DashboardApiController(ApplicationDbContext context, AMRVI.Services.PlantService plantService)
        {
            _context = context;
            _plantService = plantService;
        }

        // GET: api/dashboard/monthly?year=2026
        [HttpGet("monthly")]
        public IActionResult GetMonthlyData(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31, 23, 59, 59);

            var monthlyData = new List<int>();
            var monthlyOk = new List<int>();
            var monthlyNg = new List<int>();
            var labels = new List<string>();

            for (int month = 1; month <= 12; month++)
            {
                var monthStart = new DateTime(year, month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var okCount = _plantService.GetInspectionResults()
                    .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "OK");

                var ngCount = _plantService.GetInspectionResults()
                    .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "NG");

                monthlyOk.Add(okCount);
                monthlyNg.Add(ngCount);
                labels.Add(monthStart.ToString("MMM"));
            }

            // Get stats for the entire year
            var totalOk = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate && r.Judgement == "OK");

            var totalNg = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate && r.Judgement == "NG");

            var recentInspections = _plantService.GetInspectionSessions()
                .Where(s => s.InspectionDate >= startDate && s.InspectionDate <= endDate)
                .OrderByDescending(s => s.InspectionDate)
                .Take(6)
                .Select(s => new
                {
                    inspectorName = s.InspectorName,
                    inspectionDate = s.InspectionDate,
                    machineNumber = "N/A" // Simplified for multi-plant
                })
                .ToList();

            return Ok(new
            {
                labels = labels,
                okCounts = monthlyOk,
                ngCounts = monthlyNg,
                totalOk = totalOk,
                totalNg = totalNg,
                totalInspections = totalOk + totalNg,
                recentInspections = recentInspections
            });
        }

        // GET: api/dashboard/weekly?year=2026&month=3
        [HttpGet("weekly")]
        public IActionResult GetWeeklyData(int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var weeklyOk = new List<int>();
            var weeklyNg = new List<int>();
            var labels = new List<string>();

            // Calculate weeks in the month
            var currentDate = monthStart;
            int weekNumber = 1;

            while (currentDate <= monthEnd)
            {
                var weekStart = currentDate;
                var weekEnd = currentDate.AddDays(6);
                if (weekEnd > monthEnd) weekEnd = monthEnd;

                var okCount = _plantService.GetInspectionResults()
                    .Count(r => r.CreatedAt >= weekStart && r.CreatedAt <= weekEnd && r.Judgement == "OK");

                var ngCount = _plantService.GetInspectionResults()
                    .Count(r => r.CreatedAt >= weekStart && r.CreatedAt <= weekEnd && r.Judgement == "NG");

                weeklyOk.Add(okCount);
                weeklyNg.Add(ngCount);
                labels.Add($"Week {weekNumber}");

                currentDate = currentDate.AddDays(7);
                weekNumber++;
            }

            // Get stats for the entire month
            var totalOk = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "OK");

            var totalNg = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "NG");

            var recentInspections = _plantService.GetInspectionSessions()
                .Where(s => s.InspectionDate >= monthStart && s.InspectionDate <= monthEnd)
                .OrderByDescending(s => s.InspectionDate)
                .Take(6)
                .Select(s => new
                {
                    inspectorName = s.InspectorName,
                    inspectionDate = s.InspectionDate,
                    machineNumber = "N/A" // Simplified for multi-plant
                })
                .ToList();

            return Ok(new
            {
                labels = labels,
                okCounts = weeklyOk,
                ngCounts = weeklyNg,
                totalOk = totalOk,
                totalNg = totalNg,
                totalInspections = totalOk + totalNg,
                recentInspections = recentInspections
            });
        }

        // GET: api/dashboard/daily?year=2026&month=3&week=2
        [HttpGet("daily")]
        public IActionResult GetDailyData(int year, int month, int week)
        {
            var monthStart = new DateTime(year, month, 1);
            
            // Calculate the start of the specified week
            var weekStart = monthStart.AddDays((week - 1) * 7);
            var weekEnd = weekStart.AddDays(6);

            var dailyOk = new List<int>();
            var dailyNg = new List<int>();
            var labels = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

            for (int day = 0; day < 7; day++)
            {
                var currentDay = weekStart.AddDays(day);
                var dayEnd = currentDay.AddDays(1).AddSeconds(-1);

                var okCount = _plantService.GetInspectionResults()
                    .Count(r => r.CreatedAt >= currentDay && r.CreatedAt <= dayEnd && r.Judgement == "OK");

                var ngCount = _plantService.GetInspectionResults()
                    .Count(r => r.CreatedAt >= currentDay && r.CreatedAt <= dayEnd && r.Judgement == "NG");

                dailyOk.Add(okCount);
                dailyNg.Add(ngCount);
            }

            // Get stats for the entire week
            var totalOk = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt >= weekStart && r.CreatedAt <= weekEnd && r.Judgement == "OK");

            var totalNg = _plantService.GetInspectionResults()
                .Count(r => r.CreatedAt >= weekStart && r.CreatedAt <= weekEnd && r.Judgement == "NG");

            var recentInspections = _plantService.GetInspectionSessions()
                .Where(s => s.InspectionDate >= weekStart && s.InspectionDate <= weekEnd)
                .OrderByDescending(s => s.InspectionDate)
                .Take(6)
                .Select(s => new
                {
                    inspectorName = s.InspectorName,
                    inspectionDate = s.InspectionDate,
                    machineNumber = "N/A" // Simplified for multi-plant
                })
                .ToList();

            return Ok(new
            {
                labels = labels,
                okCounts = dailyOk,
                ngCounts = dailyNg,
                totalOk = totalOk,
                totalNg = totalNg,
                totalInspections = totalOk + totalNg,
                recentInspections = recentInspections
            });
        }
    }
}
