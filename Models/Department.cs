using System.ComponentModel.DataAnnotations;

namespace AMRVI.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Optional: Jika departemen ini spesifik untuk Plant tertentu
        // public string? Plant { get; set; } 
    }
}
