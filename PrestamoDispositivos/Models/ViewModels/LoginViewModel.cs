using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Correo o usuario")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; } 
    }
}