using System.ComponentModel.DataAnnotations;

namespace AMRVI.ViewModels
{
    public class LoginViewModel
    {
        // Username & Password hanya dibutuhkan untuk Admin
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Plant selection is required")]
        public string? Plant { get; set; }

        // Departemen tidak lagi dipilih di login
        public string? Department { get; set; }

        public bool RememberMe { get; set; }

        // Flag apakah ini login sebagai Admin
        public bool IsAdminLogin { get; set; }
    }
}
