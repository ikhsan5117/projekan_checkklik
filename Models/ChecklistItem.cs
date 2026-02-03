using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMRVI.Models.Interfaces;

namespace AMRVI.Models
{
    public class ChecklistItem : IChecklistItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int OrderNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string DetailName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string StandardDescription { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("MachineId")]
        public virtual Machine Machine { get; set; } = null!;

        public virtual ICollection<InspectionResult> InspectionResults { get; set; } = new List<InspectionResult>();
    }
}
