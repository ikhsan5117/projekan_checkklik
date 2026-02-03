using AMRVI.Models;
using AMRVI.Models.Interfaces;

namespace AMRVI.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalMachines { get; set; }
        public int InspectionsToday { get; set; }
        public int IssuesToday { get; set; }
        public int TotalChecklistItems { get; set; }
        public List<IInspectionSession> RecentInspections { get; set; } = new List<IInspectionSession>();
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
        
        // NEW: Machine Update Status
        public List<MachineUpdateStatus> MachineUpdateList { get; set; } = new List<MachineUpdateStatus>();
        public int UpdatedMachinesCount { get; set; }
        public int NotUpdatedMachinesCount { get; set; }
        
        // NEW: NG Trend Data
        public List<NgTrendData> NgTrendDaily { get; set; } = new List<NgTrendData>();
        public List<NgDetailRecord> NgDetailRecords { get; set; } = new List<NgDetailRecord>();
    }

    public class MachineStatus 
    {
        public string MachineName { get; set; } = "";
        public int TotalInspections { get; set; }
        public int IssuesFound { get; set; }
        public DateTime LastInspection { get; set; }
    }

    // NEW: Machine Update Status
    public class MachineUpdateStatus
    {
        public int MachineNumberId { get; set; }
        public string MachineNumber { get; set; } = "";
        public string MachineName { get; set; } = "";
        public bool IsUpdatedToday { get; set; }
        public DateTime? LastInspectionDate { get; set; }
    }

    // NEW: NG Trend Data
    public class NgTrendData
    {
        public DateTime Date { get; set; }
        public int NgCount { get; set; }
    }

    // NEW: NG Detail Record
    public class NgDetailRecord
    {
        public DateTime Date { get; set; }
        public int MachineNumberId { get; set; }
        public string MachineNumber { get; set; } = "";
        public int NgCount { get; set; }
        public string ChecklistItem { get; set; } = "";
        public string Standard { get; set; } = "";
        public string Problem { get; set; } = "";
    }
}
