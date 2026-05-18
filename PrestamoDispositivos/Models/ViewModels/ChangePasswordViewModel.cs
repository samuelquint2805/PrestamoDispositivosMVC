using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es requerida.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Confirma la nueva contraseña.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string ConfirmPassword { get; set; } = "";
    }
}
