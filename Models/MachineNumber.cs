using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class MachineNumber
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        [StringLength(50)]
        public string Number { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("MachineId")]
        public virtual Machine Machine { get; set; } = null!;

        public virtual ICollection<InspectionSession> InspectionSessions { get; set; } = new List<InspectionSession>();
    }
}
