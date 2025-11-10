using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.Models.ViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [Display(Name = "Código de autenticación")]
        public string TwoFactorCode { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        [Display(Name = "Recordar este equipo")]
        public bool RememberMachine { get; set; }

        public string ReturnUrl { get; set; } = string.Empty;
    }
}