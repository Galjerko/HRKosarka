using System.ComponentModel.DataAnnotations;

namespace HRKošarka.UI.Models.Auth
{
    public class RegisterVM
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
