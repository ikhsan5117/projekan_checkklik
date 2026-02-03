using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMRVI.Models.Interfaces;

namespace AMRVI.Models
{
    public class InspectionResult : IInspectionResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InspectionSessionId { get; set; }

        [Required]
        public int ChecklistItemId { get; set; }

        [Required]
        [StringLength(2)]
        public string Judgement { get; set; } = string.Empty; // "OK" or "NG"

        [StringLength(500)]
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("InspectionSessionId")]
        public virtual InspectionSession InspectionSession { get; set; } = null!;

        [ForeignKey("ChecklistItemId")]
        public virtual ChecklistItem ChecklistItem { get; set; } = null!;
    }
}
