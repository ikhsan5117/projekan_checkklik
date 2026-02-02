using AMRVI.Models;

namespace AMRVI.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalMachines { get; set; }
        public int InspectionsToday { get; set; }
        public int IssuesToday { get; set; }
        public int TotalChecklistItems { get; set; }
        public List<InspectionSession> RecentInspections { get; set; } = new List<InspectionSession>();
        public List<MachineStatus> MachineStatuses { get; set; } = new List<MachineStatus>();
        
        // Weekly Chart Data (Last 7 days: Mon-Sun)
        public List<int> OkCountsPerDay { get; set; } = new List<int>();
        public List<int> NgCountsPerDay { get; set; } = new List<int>();
        
        // Weekly OK/NG Percentages
        public double OkPercentage { get; set; }
        public double NgPercentage { get; set; }
        
        // Weekly OK/NG Counts
        public int ThisWeekOkCount { get; set; }
        public int ThisWeekNgCount { get; set; }
        
        // Total OK/NG Counts (All-Time)
        public int TotalOkCount { get; set; }
        public int TotalNgCount { get; set; }
        
        // Drill-Down State
        public string ViewLevel { get; set; } = "monthly"; // "monthly", "weekly", "daily"
        public int CurrentYear { get; set; }
        public int? CurrentMonth { get; set; }
        public int? CurrentWeek { get; set; }
        public List<string> ChartLabels { get; set; } = new List<string>();
    }

    public class MachineStatus 
    {
        public string MachineName { get; set; } = "";
        public int TotalInspections { get; set; }
        public int IssuesFound { get; set; }
        public DateTime LastInspection { get; set; }
    }
}
