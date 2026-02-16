using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class HenkatenProblem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TanggalUpdate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Shift { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PicLeader { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NamaAreaLine { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NamaOperator { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Jenis4M { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string KeteranganProblem { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string RencanaPerbaikan { get; set; } = string.Empty;

        [Required]
        public DateTime TanggalRencanaPerbaikan { get; set; }

        [StringLength(500)]
        public string? FotoTemuan { get; set; }

        [StringLength(1000)]
        public string? AktualPerbaikan { get; set; }

        public DateTime? TanggalAktualPerbaikan { get; set; }

        [StringLength(500)]
        public string? FotoAktual { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

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
