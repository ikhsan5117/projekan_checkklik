using System.ComponentModel.DataAnnotations;
using AMRVI.Models.Interfaces;

namespace AMRVI.Models
{
    public class Machine : IMachine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<MachineNumber> MachineNumbers { get; set; } = new List<MachineNumber>();
        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();
    }
}
