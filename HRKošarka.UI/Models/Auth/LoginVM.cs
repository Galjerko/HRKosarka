using System.ComponentModel.DataAnnotations;

namespace HRKošarka.UI.Models.Auth
{
    public class LoginVM
    {
        [Required]
        [Display(Name = "Email or Username")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
