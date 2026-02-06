using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class AndonRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Plant")]
        public int PlantId { get; set; }

        [Required]
        [ForeignKey("AndonMachine")]
        public int MachineId { get; set; }

        [Required]
        [ForeignKey("StatusType")]
        public int StatusId { get; set; }

        [Required]
        [ForeignKey("FourMCategory")]
        public int FourMCategoryId { get; set; }

        [StringLength(500)]
        public string? Remark { get; set; }

        [Required]
        public DateTime RecordedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool IsResolved { get; set; } = false;

        public DateTime? ResolvedAt { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        // Navigation properties
        public virtual Plant Plant { get; set; } = null!;
        public virtual AndonMachine AndonMachine { get; set; } = null!;
        public virtual StatusType StatusType { get; set; } = null!;
        public virtual FourMCategory FourMCategory { get; set; } = null!;
    }
}
