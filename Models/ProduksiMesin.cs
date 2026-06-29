using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRVI.Models
{
    public class ProduksiMesin
    {
        [Key]
        public int Id { get; set; }
        public string? KodeMesin { get; set; }
        public string? NamaMesin { get; set; }
        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public string? Keterangan { get; set; }
    }
}
