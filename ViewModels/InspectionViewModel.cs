namespace AMRVI.ViewModels
{
    public class InspectionViewModel
    {
        public int? SelectedMachineId { get; set; }
        public int? SelectedMachineNumberId { get; set; }
        public int CurrentChecklistIndex { get; set; } = 0;
        public int TotalChecklists { get; set; }
        public int InspectionSessionId { get; set; }
        
        public string MachineName { get; set; } = string.Empty;
        public string MachineNumber { get; set; } = string.Empty;
        
        public ChecklistItemViewModel? CurrentChecklist { get; set; }
        
        public List<MachineViewModel> Machines { get; set; } = new List<MachineViewModel>();
        public List<MachineNumberViewModel> MachineNumbers { get; set; } = new List<MachineNumberViewModel>();
    }

    public class MachineViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class MachineNumberViewModel
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public int MachineId { get; set; }
    }

    public class ChecklistItemViewModel
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public string DetailName { get; set; } = string.Empty;
        public string StandardDescription { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string? CurrentJudgement { get; set; }
    }

    public class InspectionSubmitModel
    {
        public int InspectionSessionId { get; set; }
        public int ChecklistItemId { get; set; }
        public string Judgement { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}
