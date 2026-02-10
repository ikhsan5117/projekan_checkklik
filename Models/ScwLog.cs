using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class ScwLog
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int? MesinId { get; set; }
        public string Jenis4M { get; set; } = string.Empty;
        public string DetailProblem { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Keterangan { get; set; } = string.Empty;
        public string? Group { get; set; }
        public string? Shift { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? ResolvedByUserId { get; set; }
    }
}
