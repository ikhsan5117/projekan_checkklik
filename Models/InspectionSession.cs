using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class InspectionSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MachineNumberId { get; set; }

        [Required]
        [StringLength(100)]
        public string InspectorName { get; set; } = string.Empty;

        public DateTime InspectionDate { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("MachineNumberId")]
        public virtual MachineNumber MachineNumber { get; set; } = null!;

        public virtual ICollection<InspectionResult> InspectionResults { get; set; } = new List<InspectionResult>();
    }
}
