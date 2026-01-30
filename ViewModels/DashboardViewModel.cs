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
    }

    public class MachineStatus 
    {
        public string MachineName { get; set; } = "";
        public int TotalInspections { get; set; }
        public int IssuesFound { get; set; }
        public DateTime LastInspection { get; set; }
    }
}
