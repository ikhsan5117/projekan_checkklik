using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class AndonMachine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Plant")]
        public int PlantId { get; set; }

        [Required]
        [StringLength(50)]
        public string MachineCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string? MachineName { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Plant Plant { get; set; } = null!;
        public virtual ICollection<AndonRecord> AndonRecords { get; set; } = new List<AndonRecord>();
    }
}
