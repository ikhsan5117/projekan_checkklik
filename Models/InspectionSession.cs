using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMRVI.Models.Interfaces;

namespace AMRVI.Models
{
    public class InspectionSession : IInspectionSession
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

        /// <summary>
        /// Calculated shift number for this inspection (1/2/3, etc.).
        /// Not mapped to the database – set in controller/service logic when needed.
        /// </summary>
        [NotMapped]
        public int? ShiftNumber { get; set; }

        // Navigation Properties
        [ForeignKey("MachineNumberId")]
        public virtual MachineNumber MachineNumber { get; set; } = null!;

        public virtual ICollection<InspectionResult> InspectionResults { get; set; } = new List<InspectionResult>();
    }
}
