namespace AMRVI.DTOs
{
    public class AndonRecordDto
    {
        public int Id { get; set; }
        public string PlantCode { get; set; } = string.Empty;
        public string MachineCode { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string FourMCode { get; set; } = string.Empty;
        public string FourMName { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public DateTime RecordedAt { get; set; }
        public bool IsResolved { get; set; }
    }

    public class CreateAndonRecordDto
    {
        public string PlantCode { get; set; } = string.Empty;
        public string MachineCode { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string FourMCode { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateAndonRecordDto
    {
        public string? StatusCode { get; set; }
        public string? FourMCode { get; set; }
        public string? Remark { get; set; }
        public bool? IsResolved { get; set; }
    }

    public class AndonStatisticsDto
    {
        public Dictionary<string, int> MachineStatus { get; set; } = new();
        public Dictionary<string, int> FourMStatus { get; set; } = new();
        public int TotalRecords { get; set; }
        public int ActiveIssues { get; set; }
    }
}
