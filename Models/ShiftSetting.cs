using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    [Table("ShiftSettings")]
    public class ShiftSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Plant { get; set; } = "RVI";

        [Required]
        public int ShiftNumber { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
