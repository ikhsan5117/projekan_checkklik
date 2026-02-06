using System;
using System.ComponentModel.DataAnnotations;

namespace AMRVI.Models
{
    public class FourMCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ColorCode { get; set; }

        [Required]
        public int Priority { get; set; } = 0;

        // Navigation property
        public virtual ICollection<AndonRecord> AndonRecords { get; set; } = new List<AndonRecord>();
    }
}
