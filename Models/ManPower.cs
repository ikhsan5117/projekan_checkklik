using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class ManPower
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NIK { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NamaLengkap { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Jabatan { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Shift { get; set; } = string.Empty;

        [StringLength(100)]
        public string? AreaLine { get; set; }

        [StringLength(50)]
        public string? NoTelepon { get; set; }

        [StringLength(200)]
        public string? Email { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [ForeignKey("Plant")]
        public int PlantId { get; set; }

        // Navigation property
        public virtual Plant Plant { get; set; } = null!;
    }
}
