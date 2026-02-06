using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class Plant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PlantCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PlantName { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
        public virtual ICollection<AndonRecord> AndonRecords { get; set; } = new List<AndonRecord>();
    }
}
